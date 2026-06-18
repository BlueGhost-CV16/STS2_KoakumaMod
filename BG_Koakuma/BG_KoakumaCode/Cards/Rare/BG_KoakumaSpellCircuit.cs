using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaSpellCircuit : KoakumaRareCard
{
    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [PowerTip<SpellCircuitPower>()];

    public BG_KoakumaSpellCircuit() : base(1, CardType.Power) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SpellCircuitPower>("Block", 3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SpellCircuitPower>(choiceContext, Owner.Creature, Amount("Block"), Owner.Creature, this);
    }

    protected override void OnUpgrade() => UpgradeAmount("Block", 1);
}
