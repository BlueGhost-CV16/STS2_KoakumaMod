using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class ManaRecycleEnginePower : KoakumaPower
{
    private int _spent;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public async Task RecordSpent(PlayerChoiceContext choiceContext, int spent, AbstractModel? source)
    {
        _spent += spent;
        while (_spent >= 5)
        {
            _spent -= 5;
            await PlayerCmd.GainEnergy(1, Owner.Player);
        }
    }
}
