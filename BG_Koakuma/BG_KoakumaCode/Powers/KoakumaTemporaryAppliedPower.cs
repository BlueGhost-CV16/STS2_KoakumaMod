using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Combat.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

public abstract class KoakumaTemporaryAppliedPower<TOriginModel, TPower> : ModTemporaryAppliedPowerTemplate<TOriginModel, TPower>
    where TOriginModel : AbstractModel
    where TPower : PowerModel
{
    public override PowerAssetProfile AssetProfile => KoakumaPowerAssets.For(GetType());

    protected virtual IEnumerable<string> ExtraKoakumaHoverTipIds => [];

    protected virtual IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        base.AdditionalHoverTips
            .Concat(KoakumaHoverTips.CreateMany(ExtraKoakumaHoverTipIds))
            .Concat(ExtraKoakumaHoverTips);

    protected static IHoverTip CardTip<TCard>() where TCard : CardModel =>
        HoverTipFactory.FromCard<TCard>();

    protected static IHoverTip PowerTip<TOtherPower>() where TOtherPower : PowerModel =>
        HoverTipFactory.FromPower<TOtherPower>();
}
