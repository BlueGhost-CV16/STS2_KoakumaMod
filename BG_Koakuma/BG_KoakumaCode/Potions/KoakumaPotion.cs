using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Potions;

public abstract class KoakumaPotion : ModPotionTemplate
{
#pragma warning disable RITSU013
    public override PotionAssetProfile AssetProfile => new(
        $"{Entry.ResPath}/images/potions/{GetType().Name}.png",
        $"{Entry.ResPath}/images/potions/outline/{GetType().Name}.png");
#pragma warning restore RITSU013

    protected virtual IEnumerable<string> ExtraKoakumaHoverTipIds => [];

    protected virtual IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        KoakumaHoverTips.CreateMany(ExtraKoakumaHoverTipIds).Concat(ExtraKoakumaHoverTips);

    protected static IHoverTip CardTip<TCard>() where TCard : CardModel =>
        HoverTipFactory.FromCard<TCard>();
}
