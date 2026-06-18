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
public sealed class BG_KoakumaGhostCrystalBook : KoakumaMagicBookCard
{

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("Show", 3),
        AmountVar("Choose", 1),
        new CardsVar(1)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.ExhaustHandCardAndChooseReplacement(choiceContext, this, false, Amount("Show"), Amount("Choose"), useGeneratedChoiceScreen: true);
        await InterpretDraw(choiceContext);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Show", 1);
    }
}
