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
public sealed class BG_KoakumaOtherworldSummon : KoakumaUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    public BG_KoakumaOtherworldSummon() : base(1, CardType.Skill)
    {
        SetMagicCost(3);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("Options", 3),
        AmountVar("Choose", 1),
        AmountVar("UpgradeGenerated", 0),
        MagicVar("MagicCost", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var options = KoakumaMechanics.RandomOtherCharacterCards(this, Amount("Options"), Amount("UpgradeGenerated") > 0);
        var selected = await KoakumaMechanics.ChooseGeneratedCards(choiceContext, options, Owner, Amount("Choose"));
        var free = KoakumaMechanics.MagicPaid(cardPlay);
        foreach (var card in selected)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Exhaust);
            if (free)
            {
                card.SetToFreeThisTurn();
            }
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("UpgradeGenerated", 1);
    }
}
