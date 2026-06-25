using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMagicBookHeavenAndEarth : KoakumaInterpretableRareCard, IKoakumaOnInterpretResolved
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret, KoakumaHoverTips.MagicBook];

    public BG_KoakumaMagicBookHeavenAndEarth() : base(4, CardType.Skill) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.FillHandWithRandomMagicBooks(choiceContext, this, IsUpgraded);
        KoakumaMechanics.ConsumeInterpret(this);
    }

    public Task ResolveInterpret(PlayerChoiceContext choiceContext)
    {
        if (KoakumaMechanics.ConsumeInterpret(this))
        {
            EnergyCost.AddUntilPlayed(-1, reduceOnly: true);
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade() { }
}
