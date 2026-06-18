using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaDeliberateReading : KoakumaCommonCard
{
    public BG_KoakumaDeliberateReading() : base(0, CardType.Skill) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("UpgradeReturned", 0)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.SpecialReadAndUpgrade(choiceContext, this, Amount("UpgradeReturned") > 0);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("UpgradeReturned", 1);
    }
}
