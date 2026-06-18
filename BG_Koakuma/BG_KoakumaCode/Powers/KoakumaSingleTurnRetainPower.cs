using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

    public override bool ShouldFlush(Player player)
    {
        return player != Owner.Player || !HasZeroCostCardInHand(player);
    }

    public override async Task AfterFlush(PlayerChoiceContext choiceContext, Player player, IReadOnlyCollection<CardModel> flushedCards, IReadOnlyCollection<CardModel> retainedCards)
    {
        if (player != Owner.Player || retainedCards.Count == 0)
        {
            return;
        }

        var cardsToDiscard = retainedCards
            .Where(card => !IsZeroCost(card) && !card.ShouldRetainThisTurn)
            .ToList();
        if (cardsToDiscard.Count > 0)
        {
            await CardPileCmd.Add(cardsToDiscard, PileType.Discard);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Decrement(this);
        }
    }

    private static bool HasZeroCostCardInHand(Player player)
    {
        return PileType.Hand.GetPile(player).Cards.Any(IsZeroCost);
    }

    private static bool IsZeroCost(CardModel card)
    {
        return card.EnergyCost.GetWithModifiers(CostModifiers.Local) == 0;
    }
}
