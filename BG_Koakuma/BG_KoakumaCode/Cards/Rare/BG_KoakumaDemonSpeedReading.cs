using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Powers;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaDemonSpeedReading : KoakumaRareCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic, KoakumaHoverTips.Interpret];

    public BG_KoakumaDemonSpeedReading() : base(1, CardType.Skill)
    {
        SetMagicCost(3);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        MagicVar("MagicCost", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var before = PileType.Hand.GetPile(Owner).Cards.ToHashSet();
        await CardPileCmd.Draw(choiceContext, Amount("Cards"), Owner);
        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            foreach (var card in PileType.Hand.GetPile(Owner).Cards.Where(card => !before.Contains(card)))
            {
                await KoakumaMechanics.MarkInterpretedAndResolve(choiceContext, card);
            }
        }
    }

    protected override void OnUpgrade() => UpgradeAmount("Cards", 1);
}
