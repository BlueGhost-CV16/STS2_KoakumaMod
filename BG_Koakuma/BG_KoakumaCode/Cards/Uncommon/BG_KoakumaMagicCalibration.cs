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
public sealed class BG_KoakumaMagicCalibration : KoakumaUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    public BG_KoakumaMagicCalibration() : base(0, CardType.Skill)
    {
        SetMagicCost(3);
    }

    //public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        MagicVar("MagicCost", 3),
        MagicVar("Magic", 3),
        AmountVar("Choose", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount("Choose"))
            {
                Cancelable = true
            }, card => card != this, this)).FirstOrDefault();
            selected?.EnergyCost.SetUntilPlayed(0);
        }

        await KoakumaMechanics.GainMagic(Owner, Amount("Magic"), this);
    }

    protected override void OnUpgrade() => UpgradeAmount("Magic", 2);
}
