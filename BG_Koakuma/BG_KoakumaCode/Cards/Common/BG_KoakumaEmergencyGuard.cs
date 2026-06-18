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
public sealed class BG_KoakumaEmergencyGuard : KoakumaCommonCard
{
    public BG_KoakumaEmergencyGuard() : base(0, CardType.Skill)
    {
        SetMagicCost(2);
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6, ValueProp.Move),
        MagicVar("MagicCost", 2),
        new PowerVar<MagicBurnPower>("MagicBurn", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            await PowerCmd.Apply<MagicBurnPower>(choiceContext, CombatState.HittableEnemies, Amount("MagicBurn"), Owner.Creature, this);
        }
        else
        {
            await CardCmd.Exhaust(choiceContext, this);
            //CardCmd.ApplyKeyword(this, CardKeyword.Exhaust);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        UpgradeAmount("MagicBurn", 1);
    }
}
