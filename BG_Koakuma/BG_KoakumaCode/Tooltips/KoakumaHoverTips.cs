using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using BG_Koakuma.Cards;
using STS2RitsuLib.Keywords;

namespace BG_Koakuma.Tooltips;

internal static class KoakumaHoverTips
{
    public const string Magic = "BG_KOAKUMA_SECONDARY_RESOURCE_MAGIC";
    public const string MagicCost = "BG_KOAKUMA_MAGIC_COST";
    public const string MagicBook = KoakumaCard.MagicBookKeywordId;
    public const string Read = "BG_KOAKUMA_TIP_READ";
    public const string Interpret = "BG_KOAKUMA_TIP_INTERPRET";
    public const string MagicBookCollection = "BG_KOAKUMA_TIP_MAGIC_BOOK_COLLECTION";
    public const string MagicBurn = "BG_KOAKUMA_TIP_MAGIC_BURN";

    public static IHoverTip Create(string id)
    {
        if (id == MagicBook)
        {
            return ModKeywordRegistry.CreateHoverTip(id);
        }

        return new HoverTip(
            new LocString("static_hover_tips", $"{id}.title"),
            new LocString("static_hover_tips", $"{id}.description"));
    }

    public static IReadOnlyList<IHoverTip> CreateMany(IEnumerable<string> ids)
    {
        var uniqueIds = new HashSet<string>(StringComparer.Ordinal);
        var tips = new List<IHoverTip>();

        foreach (var id in ids)
        {
            if (uniqueIds.Add(id))
            {
                tips.Add(Create(id));
            }
        }

        return tips;
    }
}
