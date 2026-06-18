using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaNamelessMagicBook : KoakumaInterpretableUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret, KoakumaHoverTips.MagicBookCollection, KoakumaHoverTips.Read];

    public BG_KoakumaNamelessMagicBook() : base(0, CardType.Skill) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new CardsVar("InterpretCards", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, Amount("Cards"), Owner);
        await KoakumaMechanics.CollectMagicBook(choiceContext, this);
        await KoakumaMechanics.Read(choiceContext, this);
        await KoakumaMechanics.DrawIfInterpreted(choiceContext, this, Amount("InterpretCards"));
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Cards", 1);
    }
}
