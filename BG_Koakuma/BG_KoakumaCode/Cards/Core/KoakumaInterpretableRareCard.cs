using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

public abstract class KoakumaInterpretableRareCard : KoakumaRareCard, IKoakumaInterpretableCard
{
    protected KoakumaInterpretableRareCard(int cost, CardType type, TargetType target = TargetType.Self)
        : base(cost, type, target)
    {
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == this && oldPileType == PileType.Hand && Pile?.Type is not (PileType.Hand or PileType.Play))
        {
            KoakumaMechanics.ClearInterpret(this);
        }

        return Task.CompletedTask;
    }
}
