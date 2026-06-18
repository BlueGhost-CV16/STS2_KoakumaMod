using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMagicBookCombination : KoakumaRareCard
{
    public BG_KoakumaMagicBookCombination() : base(0, CardType.Attack, TargetType.AnyEnemy) { }

    protected override bool IsPlayable => PileType.Hand.GetPile(Owner).Cards
        .Where(card => card != this)
        .All(card => !card.CanPlay() || card.EnergyCost.GetWithModifiers(CostModifiers.Local) == 0);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(30, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(10);
}
