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
public sealed class BG_KoakumaMispageBurst : KoakumaCommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.MagicBurn];

    public BG_KoakumaMispageBurst() : base(1, CardType.Attack, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, ValueProp.Move),
        new PowerVar<MagicBurnPower>("MagicBurn", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage, Owner.Creature, this, cardPlay);
        if (PileType.Draw.GetPile(Owner).Cards.LastOrDefault()?.EnergyCost.GetWithModifiers(CostModifiers.Local) == 0)
        {
            await PowerCmd.Apply<MagicBurnPower>(choiceContext, cardPlay.Target, Amount("MagicBurn"), Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}
