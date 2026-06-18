using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaEmergencyEnergy : KoakumaUncommonCard
{
    public BG_KoakumaEmergencyEnergy() : base(0, CardType.Skill) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("SelectMode", 0),
        MagicVar("Magic", 2),
        new CardsVar(1),
        AmountVar("Select", 1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(Owner).Cards.Where(card => card != this).ToList();
        var target = Amount("SelectMode") > 0
            ? (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1, Amount("Select")) { Cancelable = true }, card => card != this, this)).FirstOrDefault()
            : hand.OrderBy(_ => Owner.RunState.Rng.CombatCardSelection.NextInt()).FirstOrDefault();
        if (target != null)
        {
            await CardCmd.Exhaust(choiceContext, target);
        }
        await KoakumaMechanics.GainMagic(Owner, Amount("Magic"), this);
        await CardPileCmd.Draw(choiceContext, Amount("Cards"), Owner);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("SelectMode", 1);
        UpgradeAmount("Magic", 1);
    }
}
