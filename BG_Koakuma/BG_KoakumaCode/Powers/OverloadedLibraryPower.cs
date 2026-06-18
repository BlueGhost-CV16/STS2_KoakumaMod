using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BG_Koakuma.Cards;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class OverloadedLibraryPower : KoakumaPower, ISecondaryResourceHookListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == Owner.Player)
        {
            await KoakumaMechanics.GainMagic(player, Amount, this);
        }
    }

    public async Task AfterSecondaryResourceSpent(SecondaryResourceSpendContext context)
    {
        if (context.Player == Owner.Player && KoakumaMagic.IsMagic(context.Definition))
        {
            await PowerCmd.Apply<MagicBurnPower>(new ThrowingPlayerChoiceContext(), Owner.CombatState.HittableEnemies, 1, Owner, null);
        }
    }
}
