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
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaMagicBookCardPool))]
public sealed class BG_KoakumaAlexandriteBook : KoakumaMagicBookCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("Show", 3),
        AmountVar("Choose", 1),
        new CardsVar(1)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var discardPile = PileType.Discard.GetPile(Owner);
        var shown = discardPile.Cards.OrderBy(_ => Owner.RunState.Rng.CombatCardSelection.NextInt()).Take(Amount("Show")).ToList();
        var selected = shown.Count == 0
            ? []
            : (await KoakumaMechanics.ChooseCardsWithGeneratedScreenWhenPossible(choiceContext, shown, Owner, SelectionScreenPrompt, Amount("Choose"))).ToList();
        foreach (var card in selected)
        {
            await CardPileCmd.Add(card, PileType.Hand, CardPilePosition.Bottom);
        }
        foreach (var card in shown.Except(selected).ToList())
        {
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Random);
        }
        await InterpretDraw(choiceContext);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Show", 1);
    }
}
