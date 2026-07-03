using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Powers;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMidnightMagicCarnival : KoakumaRareCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.MagicCost, KoakumaHoverTips.MagicBurn];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [CardTip<BG_KoakumaMagicBomb>()];

    public BG_KoakumaMidnightMagicCarnival() : base(2, CardType.Attack, TargetType.AllEnemies)
    {
        this.SecondaryResourceUses().SpendIfAvailable(
            KoakumaMagic.MagicId,
            KoakumaMagic.MagicId,
            SecondaryResourceCost.X());
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move),
        new PowerVar<MagicBurnPower>("MagicBurn", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null)
        {
            return;
        }

        var enemies = CombatState.HittableEnemies.ToList();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);
        await PowerCmd.Apply<MagicBurnPower>(choiceContext, enemies, Amount("MagicBurn"), Owner.Creature, this);

        var magic = cardPlay.SecondaryResources().Value(KoakumaMagic.MagicId);
        if (magic > 0)
        {
            await KoakumaMechanics.AutoPlayMagicBombsOnAllEnemies(choiceContext, Owner, magic);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        UpgradeAmount("MagicBurn", 1);
    }
}
