using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Combat.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

public abstract class KoakumaTemporaryAppliedPower<TOriginModel, TPower> : ModTemporaryAppliedPowerTemplate<TOriginModel, TPower>
    where TOriginModel : AbstractModel
    where TPower : PowerModel
{
    public override PowerAssetProfile AssetProfile => KoakumaPowerAssets.For(GetType());
}
