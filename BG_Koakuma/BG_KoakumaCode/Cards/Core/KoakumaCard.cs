using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

public abstract class KoakumaCard : ModCardTemplate
{
    internal const string MagicBookKeywordId = "BG_KOAKUMA_KEYWORD_MAGIC_BOOK";

    protected KoakumaCard(int cost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary)
        : base(cost, type, rarity, target, showInCardLibrary)
    {
    }

    protected static DynamicVar AmountVar(string name, decimal value)
    {
        var dynamicVar = new DynamicVar(name, value);
        return name is "MagicCost" or "MagicX"
            ? dynamicVar.WithSharedTooltip("BG_KOAKUMA_MAGIC_COST")
            : dynamicVar;
    }

    protected static DynamicVar MagicVar(string name, decimal value)
    {
        var dynamicVar = SecondaryResourceVars.For(name, KoakumaMagic.MagicId, value);
        return name is "MagicCost" or "MagicX"
            ? dynamicVar.WithSharedTooltip("BG_KOAKUMA_MAGIC_COST")
            : dynamicVar;
    }

    protected int Amount(string name) => DynamicVars[name].IntValue;

    protected decimal Value(string name) => DynamicVars[name].BaseValue;

    protected virtual IEnumerable<string> ExtraKoakumaHoverTipIds => [];

    protected virtual IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        KoakumaHoverTips.CreateMany(ExtraKoakumaHoverTipIds).Concat(ExtraKoakumaHoverTips);

    protected static IHoverTip CardTip<TCard>() where TCard : CardModel =>
        HoverTipFactory.FromCard<TCard>();

    protected static IHoverTip PowerTip<TPower>() where TPower : PowerModel =>
        HoverTipFactory.FromPower<TPower>();

    protected static IHoverTip KeywordTip(CardKeyword keyword) =>
        HoverTipFactory.FromKeyword(keyword);

    protected override bool ShouldGlowGoldInternal => KoakumaHandOutlines.ShouldGlow(this);

    protected void UpgradeAmount(string name, decimal addend) => DynamicVars[name].UpgradeValueBy(addend);

    protected void SetMagicCost(int amount)
    {
        this.SecondaryResourceUses().SpendIfAvailable(KoakumaMagic.MagicId, KoakumaMagic.MagicId, amount);
    }
}
