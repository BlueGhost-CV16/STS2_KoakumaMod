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
public sealed class BG_KoakumaTemporaryCatalog : KoakumaInterpretableUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic, KoakumaHoverTips.Interpret];

    public BG_KoakumaTemporaryCatalog() : base(0, CardType.Skill)
    {
        SetMagicCost(2);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("Choose", 1),
        MagicVar("MagicCost", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var count = Amount("Choose") + (KoakumaMechanics.ConsumeInterpret(this) ? 1 : 0);
        var drawPile = PileType.Draw.GetPile(Owner);
        var selected = await CardSelectCmd.FromCombatPile(choiceContext, drawPile, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, count)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        }, KoakumaMechanics.IsMagicBook);

        foreach (var card in selected)
        {
            await CardPileCmd.Add(card, KoakumaMechanics.MagicPaid(cardPlay) ? PileType.Hand : PileType.Draw, CardPilePosition.Top);
        }
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
