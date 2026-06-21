using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Powers.Mocks;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaMagicBookCardPool))]
public sealed class BG_KoakumaBlackPearlBook : KoakumaMagicBookCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [PowerTip<KoakumaNextReplayPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<KoakumaNextReplayPower>("Replay", 1),
        new CardsVar(1)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<KoakumaNextReplayPower>(choiceContext, Owner.Creature, Amount("Replay"), Owner.Creature, this);
        await InterpretDraw(choiceContext);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Replay", 1);
    }
}
