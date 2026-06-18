using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

public static class KoakumaPowerAssets
{
    public static PowerAssetProfile For(Type powerType)
    {
        var path = $"{Entry.ResPath}/images/powers/{powerType.Name}.png";
        return new PowerAssetProfile(path, path);
    }
}
