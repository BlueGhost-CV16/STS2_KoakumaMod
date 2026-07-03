using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaScarletDevilGarden : KoakumaUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [PowerTip<ManaOverdrivePower>(), PowerTip<StrengthPower>(), PowerTip<DexterityPower>()];

    public BG_KoakumaScarletDevilGarden() : base(1, CardType.Power) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ManaOverdrivePower>("Power", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ManaOverdrivePower>(choiceContext, Owner.Creature, Amount("Power"), Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Power", 1);
    }
}
