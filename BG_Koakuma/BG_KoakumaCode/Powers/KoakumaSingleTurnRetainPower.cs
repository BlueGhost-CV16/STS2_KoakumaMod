using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

using BG_Koakuma.Tooltips;
namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class KoakumaSingleTurnRetainPower : KoakumaPower
{
    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [KeywordTip(CardKeyword.Retain)];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public static Task Retain(PlayerChoiceContext choiceContext, Player player, CardModel source, int duration = 1)
    {
        return duration <= 0
            ? Task.CompletedTask
            : PowerCmd.Apply<KoakumaSingleTurnRetainPower>(choiceContext, player.Creature, duration, player.Creature, source);
    }

    public override Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (player != Owner.Player || combatState == null || !Hook.ShouldFlush(combatState, player))
        {
            return Task.CompletedTask;
        }

        foreach (var card in PileType.Hand.GetPile(player).Cards.Where(card => IsZeroCost(card) && !card.ShouldRetainThisTurn))
        {
            card.GiveSingleTurnRetain();
        }

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Decrement(this);
        }
    }

    private static bool IsZeroCost(CardModel card)
    {
        return card.EnergyCost.GetWithModifiers(CostModifiers.Local) == 0;
    }
}
