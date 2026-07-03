using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

using BG_Koakuma.Tooltips;
namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class MagicBurnPower : KoakumaPower
{
    //protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.MagicBurn];

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    //public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    //{
    //    await TriggerBurnDamage(new ThrowingPlayerChoiceContext(), Amount, applier, cardSource);
    //}

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && amount > 0)
        {
            await TriggerBurnDamage(choiceContext, Amount, applier, cardSource);
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && dealer != null && dealer != Owner && props.IsPoweredAttack())
        {
            await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unpowered, dealer, cardSource, null);
            if (dealer.GetPower<StackedInjuryPower>() is { } stackedInjury)
            {
                await CreatureCmd.Damage(choiceContext, Owner, Amount * stackedInjury.Amount, ValueProp.Unpowered, dealer, cardSource, null);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            if (CombatState.Players.Any(player => player.Creature.GetPower<ForbiddenImmortalMagicFlamePower>() != null))
            {
                return;
            }
            await PowerCmd.ModifyAmount(choiceContext, this, -Math.Max(1, (int)Math.Ceiling(Amount / 2m)), Owner, null);
        }
    }

    private async Task TriggerBurnDamage(PlayerChoiceContext choiceContext, decimal amount, Creature? dealer, CardModel? cardSource)
    {
        await CreatureCmd.Damage(choiceContext, Owner, amount, ValueProp.Unpowered, dealer, cardSource, null);
        if (dealer?.GetPower<StackedInjuryPower>() is { } stackedInjury)
        {
            await CreatureCmd.Damage(choiceContext, Owner, amount * stackedInjury.Amount, ValueProp.Unpowered, dealer, cardSource, null);
        }
    }
}
