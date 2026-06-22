using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaWorldCatalogMagicBook : KoakumaAncientCard, IKoakumaInterpretableCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.MagicBook, KoakumaHoverTips.Read];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars => [AmountVar("Reads", 2)];

    public BG_KoakumaWorldCatalogMagicBook() : base(0, CardType.Skill) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var interpreted = KoakumaMechanics.ConsumeInterpret(this);
        await KoakumaMechanics.ChooseGeneratedMagicBookToDrawPile(choiceContext, this, interpreted, IsUpgraded);
        for (var i = 0; i < Amount("Reads"); i++)
        {
            await KoakumaMechanics.Read(choiceContext, this);
        }
    }

    protected override void OnUpgrade() { }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == this && oldPileType == PileType.Hand && Pile?.Type is not (PileType.Hand or PileType.Play))
        {
            KoakumaMechanics.ClearInterpret(this);
        }

        return Task.CompletedTask;
    }
}
