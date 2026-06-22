using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class ManaRecycleEnginePower : KoakumaPower, IPowerExtraIconAmountLabelSpecsProvider
{
    private int _spent;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override int DisplayAmount => RemainingMagic;

    private int RequiredMagic => Math.Max(1, Amount);
    private int RemainingMagic => RequiredMagic - _spent % RequiredMagic;

    public IReadOnlyList<ExtraIconAmountLabelSpec> GetPowerExtraIconAmountLabelSpecs()
    {
        return
        [
            ExtraIconAmountLabelSpec.Plain(ExtraIconAmountLabelCorner.BottomRight, RemainingMagic.ToString()),
            ExtraIconAmountLabelSpec.Plain(ExtraIconAmountLabelCorner.TopRight, RequiredMagic.ToString())
        ];
    }

    public async Task RecordSpent(PlayerChoiceContext choiceContext, int spent, AbstractModel? source)
    {
        Player? player = Owner.Player;
        if (spent <= 0 || player is null)
        {
            return;
        }

        _spent += spent;
        while (_spent >= RequiredMagic)
        {
            _spent -= RequiredMagic;
            await PlayerCmd.GainEnergy(1, player);
        }

        InvokeDisplayAmountChanged();
    }
}
