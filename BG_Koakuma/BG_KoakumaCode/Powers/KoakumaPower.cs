using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

public abstract class KoakumaPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => KoakumaPowerAssets.For(GetType());
}
