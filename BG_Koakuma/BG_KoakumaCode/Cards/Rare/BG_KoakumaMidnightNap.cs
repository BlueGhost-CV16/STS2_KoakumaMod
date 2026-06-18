using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMidnightNap : KoakumaRareCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    public BG_KoakumaMidnightNap() : base(1, CardType.Skill) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("Select", 1),
        new CardsVar(2),
        MagicVar("Magic", 2),
        new EnergyVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount("Select")) { Cancelable = true }, card => card != this, this)).FirstOrDefault();
        var wasGenerated = selected != null && !IsInDeck(selected);
        if (selected != null)
        {
            await CardCmd.Exhaust(choiceContext, selected);
        }
        await CardPileCmd.Draw(choiceContext, Amount("Cards"), Owner);
        await KoakumaMechanics.GainMagic(Owner, Amount("Magic"), this);
        if (wasGenerated)
        {
            await PlayerCmd.GainEnergy(Amount("Energy"), Owner);
        }
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Cards", 1);
        UpgradeAmount("Magic", 1);
        UpgradeAmount("Energy", 1);
    }

    private bool IsInDeck(CardModel card)
    {
        var deck = PileType.Deck.GetPile(Owner).Cards;
        return deck.Contains(card) || (card.DeckVersion != null && deck.Contains(card.DeckVersion));
    }
}
