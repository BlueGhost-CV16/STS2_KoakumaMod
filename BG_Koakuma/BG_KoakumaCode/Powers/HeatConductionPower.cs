using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BG_Koakuma.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class HeatConductionPower : KoakumaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay ||
            cardPlay.Card.Owner.Creature != Owner ||
            cardPlay.Card is BG_KoakumaMagicBomb ||
            cardPlay.Card.EnergyCost.GetWithModifiers(CostModifiers.Local) != 0 ||
            !cardPlay.Card.Keywords.Contains(CardKeyword.Exhaust))
        {
            return;
        }

        var player = Owner.Player;
        var combatState = Owner.CombatState;
        if (player == null || combatState == null)
        {
            return;
        }

        for (var i = 0; i < Amount; i++)
        {
            await CardPileCmd.AddGeneratedCardToCombat(combatState.CreateCard<BG_KoakumaMagicBomb>(player), PileType.Hand, player);
        }
    }
}
