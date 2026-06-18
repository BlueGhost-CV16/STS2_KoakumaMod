using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BG_Koakuma.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class ManaRefundPower : KoakumaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task Refund(PlayerChoiceContext choiceContext, int amount, AbstractModel? source)
    {
        if (Owner.Player != null)
        {
            await KoakumaMechanics.GainMagic(Owner.Player, amount, source);
        }
        await PowerCmd.Decrement(this);
    }
}
