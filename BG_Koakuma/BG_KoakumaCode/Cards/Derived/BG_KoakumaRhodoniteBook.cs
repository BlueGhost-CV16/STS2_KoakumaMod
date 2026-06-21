using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

using BG_Koakuma.Tooltips;
namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaMagicBookCardPool))]
public sealed class BG_KoakumaRhodoniteBook : KoakumaMagicBookCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic, KoakumaHoverTips.Interpret];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        MagicVar("Magic", 4),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.GainMagic(Owner, Amount("Magic"), this);
        await InterpretDraw(choiceContext);
    }

    protected override void OnUpgrade() => UpgradeAmount("Magic", 2);
}
