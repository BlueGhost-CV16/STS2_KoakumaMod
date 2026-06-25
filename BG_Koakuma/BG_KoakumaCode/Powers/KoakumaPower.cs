using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

public abstract class KoakumaPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => KoakumaPowerAssets.For(GetType());

    protected virtual IEnumerable<string> ExtraKoakumaHoverTipIds => [];

    protected virtual IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        KoakumaHoverTips.CreateMany(ExtraKoakumaHoverTipIds).Concat(ExtraKoakumaHoverTips);

    internal static IEnumerable<IHoverTip> CreateAdditionalHoverTips<TPower>() where TPower : KoakumaPower, new() =>
        new TPower().AdditionalHoverTips;

    protected static IHoverTip CardTip<TCard>() where TCard : CardModel =>
        HoverTipFactory.FromCard<TCard>();

    protected static IHoverTip PowerTip<TPower>() where TPower : PowerModel =>
        HoverTipFactory.FromPower<TPower>();

    protected static IHoverTip KeywordTip(CardKeyword keyword) =>
        HoverTipFactory.FromKeyword(keyword);
}
