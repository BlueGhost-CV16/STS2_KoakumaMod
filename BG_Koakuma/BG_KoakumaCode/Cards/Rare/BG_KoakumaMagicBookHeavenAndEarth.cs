using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Powers;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMagicBookHeavenAndEarth : KoakumaInterpretableRareCard, IKoakumaOnInterpretResolved
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret, KoakumaHoverTips.MagicBook];

    public BG_KoakumaMagicBookHeavenAndEarth() : base(3, CardType.Attack, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState?.HittableEnemies.ToList() ?? [];
        await CreatureCmd.Damage(choiceContext, enemies, DynamicVars.Damage, Owner.Creature, this);
        await KoakumaMechanics.FillHandWithRandomMagicBooks(choiceContext, this);
        KoakumaMechanics.ConsumeInterpret(this);
    }

    public Task ResolveInterpret(PlayerChoiceContext choiceContext)
    {
        if (KoakumaMechanics.ConsumeInterpret(this))
        {
            EnergyCost.AddUntilPlayed(-1, reduceOnly: true);
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
