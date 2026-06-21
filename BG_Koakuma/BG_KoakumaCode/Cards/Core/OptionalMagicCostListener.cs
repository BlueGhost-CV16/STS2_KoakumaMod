using STS2RitsuLib.Combat.SecondaryResources;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BG_Koakuma.Cards;

internal sealed class OptionalMagicCostListener : ISecondaryResourceHookListener
{
    public static OptionalMagicCostListener Instance { get; } = new();

    public async Task AfterSecondaryResourceSpent(SecondaryResourceSpendContext context)
    {
        if (!KoakumaMagic.IsMagic(context.Definition) || context.Amount <= 0)
        {
            return;
        }

        await KoakumaMechanics.AfterMagicSpent(
            new BlockingPlayerChoiceContext(),
            context.Player,
            context.Amount,
            context.Source);
    }
}
