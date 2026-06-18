using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaTemporaryBinding : KoakumaInterpretableCommonCard
{
    public BG_KoakumaTemporaryBinding() : base(1, CardType.Skill) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7, ValueProp.Move),
        AmountVar("Choose", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount("Choose"))
        {
            Cancelable = true
        }, card => card != this, this)).FirstOrDefault();
        if (selected == null)
        {
            return;
        }

        selected.GiveSingleTurnRetain();
        if (KoakumaMechanics.ConsumeInterpret(this))
        {
            selected.EnergyCost.AddUntilPlayed(-1, reduceOnly: true);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}
