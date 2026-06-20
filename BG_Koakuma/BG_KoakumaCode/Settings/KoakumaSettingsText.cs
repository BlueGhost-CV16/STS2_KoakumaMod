using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils;

namespace BG_Koakuma.Settings;

public static class KoakumaSettingsText
{
    private static readonly I18N Localization = RitsuLibFramework.CreateModLocalization(
        Entry.ModId,
        "settings_ui",
        pckFolders: [$"{Entry.ResPath}/i18n/settings_ui"]);

    public static ModSettingsText Text(string key, string fallback)
    {
        return ModSettingsText.I18N(Localization, key, fallback);
    }
}
