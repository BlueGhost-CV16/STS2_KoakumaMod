using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaWasteHeatReturn : KoakumaRareCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.MagicBurn, KoakumaHoverTips.Block];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [KeywordTip(CardKeyword.Retain)];

    public BG_KoakumaWasteHeatReturn() : base(1, CardType.Power) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WasteHeatReturnPower>("Power", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WasteHeatReturnPower>(choiceContext, Owner.Creature, Amount("Power"), Owner.Creature, this);
    }

    protected override void OnUpgrade() => UpgradeAmount("Power", 1);
}
