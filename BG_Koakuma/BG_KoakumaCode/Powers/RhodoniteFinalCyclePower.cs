using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using BG_Koakuma.Cards;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class RhodoniteFinalCyclePower : KoakumaPower, IKoakumaAfterMagicSpent, ISecondaryResourceHookListener
{
    public const int LockedMagicAmount = 999;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerAssetProfile AssetProfile => KoakumaPowerAssets.For(typeof(ManaRefundPower));

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        SecondaryResourceVars.For("Magic", KoakumaMagic.MagicId, LockedMagicAmount)
    ];

    public decimal ModifySecondaryResourceGain(SecondaryResourceContext context, decimal amount)
    {
        return IsLockedMagicChange(context.Player, context.Definition)
            ? 0
            : amount;
    }

    public async Task AfterSecondaryResourceChanged(SecondaryResourceChangeContext context)
    {
        if (!IsLockedMagicChange(context.Player, context.Definition)
            || context.Source == this
            || context.NewAmount == LockedMagicAmount)
        {
            return;
        }

        await KoakumaMechanics.SetMagic(context.Player, LockedMagicAmount, this);
    }

    public async Task AfterMagicSpent(PlayerChoiceContext choiceContext, int amount, AbstractModel? source)
    {
        if (Owner.Player != null)
        {
            await KoakumaMechanics.SetMagic(Owner.Player, LockedMagicAmount, this);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Owner.Player == null || side != Owner.Side || !participants.Contains(Owner))
        {
            return;
        }

        await KoakumaMechanics.SetMagic(Owner.Player, 0, this);
        await PowerCmd.Remove(this);
    }

    private bool IsLockedMagicChange(Player player, SecondaryResourceDefinition definition)
    {
        return Owner.Player == player && KoakumaMagic.IsMagic(definition);
    }
}
