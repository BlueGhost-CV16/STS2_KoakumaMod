using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BG_Koakuma.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class ManaOverdrivePower : KoakumaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == Owner.Player && await KoakumaMechanics.SpendMagic(choiceContext, player, this, 1))
        {
            await PowerCmd.Apply<ManaOverdriveTemporaryStrengthPower>(choiceContext, Owner, Amount, Owner, null);
            await PowerCmd.Apply<ManaOverdriveTemporaryDexterityPower>(choiceContext, Owner, Amount, Owner, null);
        }
    }
}
