using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Powers.Mocks;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.CardPools;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(TokenCardPool))]
public sealed class BG_KoakumaLibrarianContract : KoakumaDerivedCard
{
    public BG_KoakumaLibrarianContract() : base(0, CardType.Power)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<LibrarianContractPower>("Power", 1),
        AmountVar("Return", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount("Return")) { Cancelable = true }, card => card != this, this);
        foreach (var card in selected)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<BG_KoakumaLapisBook>(Owner), PileType.Hand, Owner);
        await PowerCmd.Apply<LibrarianContractPower>(choiceContext, Owner.Creature, Amount("Power"), Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
