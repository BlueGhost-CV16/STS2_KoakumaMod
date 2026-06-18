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
public sealed class BG_KoakumaForcedSpellIntervention : KoakumaRareCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic, KoakumaHoverTips.Interpret, KoakumaHoverTips.MagicBook];

    public BG_KoakumaForcedSpellIntervention() : base(0, CardType.Skill)
    {
        SetMagicCost(3);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        MagicVar("MagicCost", 3),
        AmountVar("RequireInterpreted", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, 1) { Cancelable = true }, card => card != this && KoakumaMechanics.CanInterpret(card), this)).FirstOrDefault();
        if (selected != null)
        {
            var alreadyInterpreted = KoakumaMechanics.IsInterpreted(selected);
            await KoakumaMechanics.MarkInterpretedAndResolve(choiceContext, selected);
            if (KoakumaMechanics.IsMagicBook(selected) && (alreadyInterpreted || Amount("RequireInterpreted") == 0))
            {
                await KoakumaMechanics.TransformToTrueName(selected);
            }
        }

        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Bottom);
        }
    }

    protected override void OnUpgrade() => UpgradeAmount("RequireInterpreted", -1);
}
