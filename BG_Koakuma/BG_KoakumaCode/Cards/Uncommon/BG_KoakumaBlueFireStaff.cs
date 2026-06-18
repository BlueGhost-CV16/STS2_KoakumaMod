using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaBlueFireStaff : KoakumaUncommonCard
{
    public override int MaxUpgradeLevel => int.MaxValue;

    public BG_KoakumaBlueFireStaff() : base(1, CardType.Attack, TargetType.AnyEnemy)
    {
        SetMagicCost(3);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1, ValueProp.Move),
        new PowerVar<MagicBurnPower>("MagicBurn", 1),
        AmountVar("HitCount", 2),
        MagicVar("MagicCost", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var hits = Amount("HitCount");

        for (var i = 0; i < hits; i++)
        {
            await PowerCmd.Apply<MagicBurnPower>(choiceContext, cardPlay.Target, Amount("MagicBurn"), Owner.Creature, this);
        }
        
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hits)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            CardCmd.Upgrade(this);
        }
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("HitCount", 1);
        //EnergyCost.UpgradeBy(-1);
    }
}
