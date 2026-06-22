using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

using BG_Koakuma.Tooltips;
namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaMagicBookCardPool))]
public sealed class BG_KoakumaMysteryMagicBook : KoakumaCard, IKoakumaInterpretableCard, IKoakumaMagicBookCard, IKoakumaOnInterpretResolved
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];

    private const int BaseEnergyCost = 1;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRarityValue = CardRarity.Token;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust, ModKeywordRegistry.GetCardKeyword(MagicBookKeywordId)];

    protected override bool ShouldGlowGoldInternal => KoakumaHandOutlines.ShouldGlow(this);

    public BG_KoakumaMysteryMagicBook() : base(BaseEnergyCost, CardKind, CardRarityValue, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selected = await KoakumaMechanics.ChooseGeneratedMagicBook(choiceContext, this, 1);
        if (selected != null)
        {
            await KoakumaMechanics.AddGeneratedCardToHand(choiceContext, selected, Owner);
        }
    }

    public async Task ResolveInterpret(PlayerChoiceContext choiceContext)
    {
        if (!KoakumaMechanics.ConsumeInterpret(this))
        {
            return;
        }
        await CardCmd.Exhaust(choiceContext, this);
        var selected = await KoakumaMechanics.ChooseGeneratedMagicBook(choiceContext, this, 3);
        if (selected != null)
        {
            await KoakumaMechanics.AddGeneratedCardToHand(choiceContext, selected, Owner);
        }
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
