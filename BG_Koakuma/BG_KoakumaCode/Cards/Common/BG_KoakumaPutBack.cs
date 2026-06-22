using MegaCrit.Sts2.Core.CardSelection;
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
public sealed class BG_KoakumaPutBack : KoakumaCommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    public BG_KoakumaPutBack() : base(1, CardType.Skill) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9, ValueProp.Move),
        AmountVar("Return", 1),
        MagicVar("Magic", 1),
        MagicVar("MagicBonus", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount("Return")) { Cancelable = true }, card => card != this, this)).FirstOrDefault();
        if (selected != null)
        {
            await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
            var cost = Math.Max(0, selected.EnergyCost.GetWithModifiers(CostModifiers.Local));
            await KoakumaMechanics.GainMagic(Owner, cost + Amount("MagicBonus"), this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}
