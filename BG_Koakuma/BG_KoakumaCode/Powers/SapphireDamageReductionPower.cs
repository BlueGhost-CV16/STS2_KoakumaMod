using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

public abstract class SapphireDamageReductionPower : KoakumaPower, IPowerExtraIconAmountLabelSpecsProvider
{
    private const string DamageReductionVarName = "DamageReduction";
    private const string ReductionStacksVarName = "ReductionStacks";
    private const string TurnsVarName = "Turns";
    private const int InitialTurns = 2;
    private bool _ignoreNextPowerAmountChanged;

    protected abstract int PercentPerStack { get; }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override int DisplayAmount => DamageReductionPercent;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(DamageReductionVarName, 1m),
        new IntVar(ReductionStacksVarName, 0),
        new IntVar(TurnsVarName, InitialTurns)
    ];

    private int RemainingReductionStacks
    {
        get => DynamicVars[ReductionStacksVarName].IntValue;
        set
        {
            var stacks = Math.Max(0, value);
            if (DynamicVars[ReductionStacksVarName].IntValue == stacks)
            {
                return;
            }

            DynamicVars[ReductionStacksVarName].BaseValue = stacks;
            InvokeDisplayAmountChanged();
        }
    }

    private int RemainingTurns
    {
        get => DynamicVars[TurnsVarName].IntValue;
        set
        {
            var turns = Math.Max(0, value);
            if (DynamicVars[TurnsVarName].IntValue == turns)
            {
                return;
            }

            DynamicVars[TurnsVarName].BaseValue = turns;
            InvokeDisplayAmountChanged();
        }
    }

    private int DamageReductionPercent => Math.Clamp(RemainingReductionStacks * PercentPerStack, 0, 100);
    private decimal DamageMultiplier => Math.Max(0m, 1m - DamageReductionPercent / 100m);

    public IReadOnlyList<ExtraIconAmountLabelSpec> GetPowerExtraIconAmountLabelSpecs()
    {
        return
        [
            ExtraIconAmountLabelSpec.Plain(ExtraIconAmountLabelCorner.BottomRight, RemainingTurns.ToString()),
            ExtraIconAmountLabelSpec.Plain(ExtraIconAmountLabelCorner.TopRight, DamageReductionPercent + "%")
        ];
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return target == Owner || dealer == Owner ? DamageMultiplier : 1m;
    }

    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        AddReductionStacks((int)amount);
        RefreshDuration();
        SyncDynamicVars();
        _ignoreNextPowerAmountChanged = true;
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power != this)
        {
            return Task.CompletedTask;
        }

        if (_ignoreNextPowerAmountChanged)
        {
            _ignoreNextPowerAmountChanged = false;
            return Task.CompletedTask;
        }

        if (amount != 0)
        {
            AddReductionStacks((int)amount);
        }

        if (amount > 0)
        {
            RefreshDuration();
        }

        SyncDynamicVars();
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack() && result.UnblockedDamage + result.OverkillDamage > 0)
        {
            ReduceReductionStacks(1);
            SyncDynamicVars();
            if (RemainingReductionStacks <= 0)
            {
                await AfterReductionFullyExpired(choiceContext);
                await PowerCmd.Remove(this);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
        {
            return;
        }

        RemainingTurns--;
        SyncDynamicVars();
        if (RemainingTurns <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }

    private void RefreshDuration()
    {
        RemainingTurns = InitialTurns;
    }

    private void AddReductionStacks(int stacks)
    {
        RemainingReductionStacks += stacks;
    }

    private void ReduceReductionStacks(int stacks)
    {
        RemainingReductionStacks -= stacks;
    }

    protected virtual Task AfterReductionFullyExpired(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    private void SyncDynamicVars()
    {
        DynamicVars[DamageReductionVarName].BaseValue = DamageMultiplier;
        DynamicVars[ReductionStacksVarName].BaseValue = RemainingReductionStacks;
        DynamicVars[TurnsVarName].BaseValue = RemainingTurns;
    }
}
