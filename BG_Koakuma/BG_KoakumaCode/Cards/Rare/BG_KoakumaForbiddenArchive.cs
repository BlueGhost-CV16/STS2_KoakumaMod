using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaForbiddenArchive : KoakumaRareCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    public BG_KoakumaForbiddenArchive() : base(0, CardType.Skill)
    {
        SetMagicCost(4);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("Choose", 1),
        MagicVar("MagicCost", 4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        var selected = await CardSelectCmd.FromCombatPile(choiceContext, exhaustPile, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount("Choose"))
        {
            Cancelable = true,
            RequireManualConfirmation = true
        }, card => card != this);

        foreach (var card in selected)
        {
            if (KoakumaMechanics.MagicPaid(cardPlay))
            {
                card.SetToFreeThisTurn();
                await CardPileCmd.Add(card, PileType.Hand, CardPilePosition.Bottom);
            }
            else
            {
                await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Random);
            }
        }
    }

    protected override void OnUpgrade() => UpgradeAmount("Choose", 1);
}
