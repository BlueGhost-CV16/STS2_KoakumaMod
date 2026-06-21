using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.RunData;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace BG_Koakuma.Settings;

public static class KoakumaSettingsPage
{
    private const string DataKey = "settings";
    private const string RunSettingsKey = "host_settings";

    private static readonly RunSavedData<KoakumaSettings> HostRunSettings = RunSavedDataStore
        .For(Entry.ModId)
        .Register<KoakumaSettings>(RunSettingsKey);

    private static KoakumaSettings? CurrentRunSettings;

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

    public static bool EasyMagicBookCollectionEnabled => ReadSettings().EasyMagicBookCollection;

    public static bool EasyReadLookEnabled => ReadSettings().EasyReadLook;

    public static bool EasyReadPutEnabled => ReadSettings().EasyReadPut;

    public static void Register()
    {
        ModDataStore.For(Entry.ModId).Register<KoakumaSettings>(
            key: DataKey,
            fileName: "settings.json",
            scope: SaveScope.Global,
            defaultFactory: static () => new KoakumaSettings(),
            autoCreateIfMissing: true);

        RitsuLibFramework.RegisterModSettings(Entry.ModId, page => page
            .WithTitle(KoakumaSettingsText.Text("settings.page.title", "设置"))
            .WithModDisplayName(KoakumaSettingsText.Text("settings.mod_display_name", "BG_Koakuma"))
            .AddSection("easy_mode", section => section
                .WithTitle(KoakumaSettingsText.Text("settings.easy_mode.title", "简易模式"))
                .AddToggle(
                    "easy_magic_book_collection",
                    KoakumaSettingsText.Text("settings.easy_magic_book_collection.label", "简易魔法书收集"),
                    EasyMagicBookCollectionBinding,
                    KoakumaSettingsText.Text("settings.easy_magic_book_collection.description", "开启后，魔法书收集自动移除最低稀有度随机牌，并随机生成魔法书洗入抽牌堆。"))
                .AddToggle(
                    "easy_read_look",
                    KoakumaSettingsText.Text("settings.easy_read_look.label", "简易阅读-展示"),
                    EasyReadLookBinding,
                    KoakumaSettingsText.Text("settings.easy_read_look.description", "开启后，阅读不再展示抽牌堆顶至多3张牌，而是默认选择抽牌堆顶的牌。"))
                .AddToggle(
                    "easy_read_put",
                    KoakumaSettingsText.Text("settings.easy_read_put.label", "简易阅读-放回"),
                    EasyReadPutBinding,
                    KoakumaSettingsText.Text("settings.easy_read_put.description", "开启后，阅读后放回抽牌堆底时不再选择，默认放回手牌中最左侧的可选牌。"))));

        RitsuLibFramework.SubscribeLifecycle<RunSavedDataLobbyStagingEvent>(StageHostSettings, replayCurrentState: false);
        RitsuLibFramework.SubscribeLifecycle<RunSavedDataPreparingEvent>(UsePreparedRunSettings, replayCurrentState: false);
        RitsuLibFramework.SubscribeLifecycle<RunEndedEvent>(_ => CurrentRunSettings = null, replayCurrentState: false);
    }

    private static KoakumaSettings ReadSettings()
    {
        return CurrentRunSettings ?? ReadLocalSettings();
    }

    private static KoakumaSettings ReadLocalSettings()
    {
        return new KoakumaSettings
        {
            EasyMagicBookCollection = EasyMagicBookCollectionBinding.Read(),
            EasyReadLook = EasyReadLookBinding.Read(),
            EasyReadPut = EasyReadPutBinding.Read()
        };
    }

    private static void StageHostSettings(RunSavedDataLobbyStagingEvent evt)
    {
        if (!evt.IsMultiplayer || !evt.IsHost)
        {
            return;
        }

        HostRunSettings.Lobby.Set(evt.Lobby, ReadLocalSettings());
    }

    private static void UsePreparedRunSettings(RunSavedDataPreparingEvent evt)
    {
        CurrentRunSettings = evt.IsMultiplayer
            ? HostRunSettings.Get(evt.RunState)
            : null;
    }
}
