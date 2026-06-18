using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaFamiliarSupport : KoakumaUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Read];

    public BG_KoakumaFamiliarSupport() : base(0, CardType.Skill) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("Bonus", 0)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.ReadAndRewardReturnedCost(choiceContext, this, Amount("Bonus"));
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Bonus", 1);
    }
}

