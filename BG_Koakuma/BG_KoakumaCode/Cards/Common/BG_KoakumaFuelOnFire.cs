using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaFuelOnFire : KoakumaCommonCard
{
    public BG_KoakumaFuelOnFire() : base(1, CardType.Skill, TargetType.AllEnemies) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5, ValueProp.Move),
        new PowerVar<WeakPower>("Power", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var amount = Amount("Power");
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var burningEnemies = CombatState.HittableEnemies.Where(enemy => enemy.GetPower<MagicBurnPower>() != null).ToList();
        await PowerCmd.Apply<WeakPower>(choiceContext, burningEnemies, amount, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, burningEnemies, amount, Owner.Creature, this);
        await PowerCmd.Apply<MagicBurnPower>(choiceContext, CombatState.HittableEnemies, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Power", 1);
    }
}

