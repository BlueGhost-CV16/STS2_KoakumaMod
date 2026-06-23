using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaMagicBookCardPool))]
public sealed class BG_KoakumaRhodoniteFinalCycle : KoakumaMagicBookCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic, KoakumaHoverTips.Interpret];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [PowerTip<RhodoniteFinalCyclePower>()];

    public override CardAssetProfile AssetProfile => new(PortraitPath: $"{Entry.ResPath}/images/cards/{nameof(BG_KoakumaRhodoniteBook)}.png");

    public BG_KoakumaRhodoniteFinalCycle() : base(true) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        MagicVar("Magic", RhodoniteFinalCyclePower.LockedMagicAmount),
        new PowerVar<RhodoniteFinalCyclePower>("Power", 1),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RhodoniteFinalCyclePower>(choiceContext, Owner.Creature, Amount("Power"), Owner.Creature, this);
        await KoakumaMechanics.SetMagic(Owner, Amount("Magic"), this);
        await InterpretDraw(choiceContext);
    }
}
