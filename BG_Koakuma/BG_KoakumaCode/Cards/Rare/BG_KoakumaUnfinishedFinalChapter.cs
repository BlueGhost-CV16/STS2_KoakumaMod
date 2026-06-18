using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaUnfinishedFinalChapter : KoakumaInterpretableRareCard
{
    public BG_KoakumaUnfinishedFinalChapter() : base(1, CardType.Skill) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [AmountVar("Choose", 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.ChooseGeneratedMagicBooksToHand(choiceContext, this, Amount("Choose"), KoakumaMechanics.ConsumeInterpret(this));
    }

    protected override void OnUpgrade() => UpgradeAmount("Choose", 1);
}
