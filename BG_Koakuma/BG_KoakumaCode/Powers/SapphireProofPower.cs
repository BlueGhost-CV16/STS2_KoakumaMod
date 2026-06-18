using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class SapphireProofPower : SapphireDamageReductionPower
{
    protected override int PercentPerStack => 5;

    protected override async Task AfterReductionFullyExpired(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<KoakumaRadiancePower>(choiceContext, Owner, 1, Owner, null);
    }
}
