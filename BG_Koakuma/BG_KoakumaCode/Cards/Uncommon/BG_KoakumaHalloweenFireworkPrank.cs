using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaHalloweenFireworkPrank : KoakumaUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    public BG_KoakumaHalloweenFireworkPrank() : base(2, CardType.Attack, TargetType.AllEnemies)
    {
        SetMagicCost(3);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14, ValueProp.Move),
        new DamageVar("BonusDamage", 7, ValueProp.Move),
        new PowerVar<MagicBurnPower>("MagicBurn", 2),
        new PowerVar<MagicBurnPower>("BonusMagicBurn", 1),
        MagicVar("MagicCost", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null)
        {
            return;
        }

        var enemies = CombatState.HittableEnemies.ToList();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);
        await PowerCmd.Apply<MagicBurnPower>(choiceContext, enemies, Amount("MagicBurn"), Owner.Creature, this);
        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            await DamageCmd.Attack(DynamicVars["BonusDamage"].BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(CombatState)
                .Execute(choiceContext);
            await PowerCmd.Apply<MagicBurnPower>(choiceContext, enemies, Amount("BonusMagicBurn"), Owner.Creature, this);
        }
        else
        {
            await CardCmd.Exhaust(choiceContext, this);
            //CardCmd.ApplyKeyword(this, CardKeyword.Exhaust);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars["BonusDamage"].UpgradeValueBy(2);
        UpgradeAmount("MagicBurn", 2);
        UpgradeAmount("BonusMagicBurn", 1);
    }
}
