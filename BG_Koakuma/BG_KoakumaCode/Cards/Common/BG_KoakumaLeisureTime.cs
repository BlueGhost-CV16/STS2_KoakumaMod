using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaLeisureTime : KoakumaCommonCard
{
    public BG_KoakumaLeisureTime() : base(1, CardType.Skill) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5, ValueProp.Move),
        AmountVar("Recover", 1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var discardPile = PileType.Discard.GetPile(Owner);
        var selected = (await CardSelectCmd.FromCombatPile(
            choiceContext,
            discardPile,
            Owner,
            new CardSelectorPrefs(new LocString("cards", "BG_KOAKUMA_MECHANIC_RECOVER_DISCARD_ZERO"), 0, Amount("Recover")) { Cancelable = true },
            card => card.EnergyCost.GetWithModifiers(CostModifiers.Local) == 0)).FirstOrDefault();
        if (selected != null)
        {
            await CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Bottom);
        }

        await KoakumaSingleTurnRetainPower.Retain(choiceContext, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
