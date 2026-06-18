using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class SapphireProtectionPower : SapphireDamageReductionPower
{
    protected override int PercentPerStack => 10;
}
