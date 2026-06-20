using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace BG_Koakuma.Settings;

public static class KoakumaSettingsPage
{
    private const string DataKey = "settings";

    private static readonly ModSettingsValueBinding<KoakumaSettings, bool> EasyMagicBookCollectionBinding = new(
        Entry.ModId,
        DataKey,
        SaveScope.Global,
        static settings => settings.EasyMagicBookCollection,
        static (settings, value) => settings.EasyMagicBookCollection = value);

    private static readonly ModSettingsValueBinding<KoakumaSettings, bool> EasyReadLookBinding = new(
        Entry.ModId,
        DataKey,
        SaveScope.Global,
        static settings => settings.EasyReadLook,
        static (settings, value) => settings.EasyReadLook = value);

    private static readonly ModSettingsValueBinding<KoakumaSettings, bool> EasyReadPutBinding = new(
        Entry.ModId,
        DataKey,
        SaveScope.Global,
        static settings => settings.EasyReadPut,
        static (settings, value) => settings.EasyReadPut = value);

    public static bool EasyMagicBookCollectionEnabled => EasyMagicBookCollectionBinding.Read();

    public static bool EasyReadLookEnabled => EasyReadLookBinding.Read();

    public static bool EasyReadPutEnabled => EasyReadPutBinding.Read();

    public static void Register()
    {
        ModDataStore.For(Entry.ModId).Register<KoakumaSettings>(
            key: DataKey,
            fileName: "settings.json",
            scope: SaveScope.Global,
            defaultFactory: static () => new KoakumaSettings(),
            autoCreateIfMissing: true);

        RitsuLibFramework.RegisterModSettings(Entry.ModId, page => page
            .WithTitle(ModSettingsText.Literal("设置"))
            .WithModDisplayName(ModSettingsText.Literal("BG_Koakuma"))
            .AddSection("easy_mode", section => section
                .WithTitle(ModSettingsText.Literal("简易模式"))
                .AddToggle(
                    "easy_magic_book_collection",
                    ModSettingsText.Literal("简易魔法书收集"),
                    EasyMagicBookCollectionBinding,
                    ModSettingsText.Literal("开启后，魔法书收集自动移除最低稀有度随机牌，并随机生成魔法书洗入抽牌堆。"))
                .AddToggle(
                    "easy_read_look",
                    ModSettingsText.Literal("简易阅读-展示"),
                    EasyReadLookBinding,
                    ModSettingsText.Literal("开启后，阅读不再展示抽牌堆顶至多3张牌，而是默认选择抽牌堆顶的牌。"))
                .AddToggle(
                    "easy_read_put",
                    ModSettingsText.Literal("简易阅读-放回"),
                    EasyReadPutBinding,
                    ModSettingsText.Literal("开启后，阅读后放回抽牌堆底时不再选择，默认放回手牌中最左侧的可选牌。"))));
    }
}
