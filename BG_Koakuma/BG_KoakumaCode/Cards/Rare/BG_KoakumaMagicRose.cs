using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMagicRose : KoakumaRareCard
{
    public BG_KoakumaMagicRose() : base(1, CardType.Skill) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [MagicVar("Magic", 12)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.GainMagic(Owner, Amount("Magic"), this);
        await PowerCmd.Apply<LoseMagicNextTurnPower>(choiceContext, Owner.Creature, Amount("Magic"), Owner.Creature, this);
    }

    protected override void OnUpgrade() => UpgradeAmount("Magic", 4);
}
