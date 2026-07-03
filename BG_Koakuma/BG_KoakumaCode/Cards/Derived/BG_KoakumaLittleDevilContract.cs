using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Powers.Mocks;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.CardPools;
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(TokenCardPool))]
public sealed class BG_KoakumaLittleDevilContract : KoakumaDerivedCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic, KoakumaHoverTips.Interpret];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [KeywordTip(CardKeyword.Retain)];

    public BG_KoakumaLittleDevilContract() : base(0, CardType.Power)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        AmountVar("Return", 1),
        MagicVar("MagicCost", 3),
        new EnergyVar("Energy", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1, 1), card => card != this, this);
        foreach (var card in selected)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        await CardPileCmd.Draw(choiceContext, Amount("Cards"), Owner);
        await PowerCmd.Apply<KoakumaContractPower>(choiceContext, Owner.Creature, Amount("Energy"), Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
