using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;

using BG_Koakuma.Tooltips;
namespace BG_Koakuma.Cards;

[RegisterCard(typeof(TokenCardPool))]
public sealed class BG_KoakumaRhodoniteIsolation : KoakumaMagicBookCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic, KoakumaHoverTips.Interpret];

    public BG_KoakumaRhodoniteIsolation() : base(true) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        MagicVar("Magic", 3),
        //AmountVar("Choose", 1),
        //new EnergyVar("CostReduction", 1),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.GainMagic(Owner, Amount("Magic"), this);
        
        await KoakumaMechanics.GainMagic(Owner, SecondaryResourceCmd.Get(Owner, KoakumaMagic.MagicId), this);

        //var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount("Choose"))
        //{
        //    Cancelable = true
        //}, card => card != this, this)).FirstOrDefault();
        //if (selected != null)
        //{
        //    selected.EnergyCost.AddUntilPlayed(-Amount("CostReduction"), reduceOnly: true);
        //}

        await InterpretDraw(choiceContext);
    }

    protected override void OnUpgrade() => UpgradeAmount("Magic", 1);
}
