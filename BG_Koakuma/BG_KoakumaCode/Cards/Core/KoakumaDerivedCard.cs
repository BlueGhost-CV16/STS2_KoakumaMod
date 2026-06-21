using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Powers.Mocks;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

public abstract class KoakumaDerivedCard : KoakumaCard, IKoakumaInterpretableCard
{
    private readonly bool _baseRetain;

    protected KoakumaDerivedCard(
        int cost,
        CardType type,
        TargetType target = TargetType.Self,
        bool baseRetain = false,
        bool showInCardLibrary = false)
        : base(cost, type, CardRarity.Token, target, showInCardLibrary)
    {
        _baseRetain = baseRetain;
    }

    public override CardAssetProfile AssetProfile => new(PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    public override IEnumerable<CardKeyword> CanonicalKeywords => Type switch
    {
        CardType.Power when _baseRetain => [CardKeyword.Retain],
        CardType.Power => [],
        _ when _baseRetain => [CardKeyword.Exhaust, CardKeyword.Retain],
        _ => [CardKeyword.Exhaust]
    };

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == this && Pile?.Type == PileType.Hand && oldPileType != PileType.Hand
            && Owner.Creature.GetPower<MagicBookCorridorEtoilePower>() != null)
        {
            KoakumaMechanics.MarkInterpreted(this);
        }

        if (card == this && oldPileType == PileType.Hand && Pile?.Type is not (PileType.Hand or PileType.Play))
        {
            KoakumaMechanics.ClearInterpret(this);
        }

        return Task.CompletedTask;
    }

    protected Task InterpretDraw(PlayerChoiceContext choiceContext)
    {
        return KoakumaMechanics.DrawIfInterpreted(choiceContext, this);
    }
}
