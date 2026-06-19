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
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(TokenCardPool))]
public sealed class BG_KoakumaFluoriteAfterimage : KoakumaMagicBookCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];

    public BG_KoakumaFluoriteAfterimage() : base(true) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FluoriteAfterimagePower>("ExtraTurn", 1),
        AmountVar("ExhaustSelf", 1),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FluoriteAfterimagePower>(choiceContext, Owner.Creature, Amount("ExtraTurn"), Owner.Creature, this);
        if (Amount("ExhaustSelf") > 0)
        {
            PlayerCmd.EndTurn(Owner, canBackOut: false);
        }
        await InterpretDraw(choiceContext);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("ExhaustSelf", -1);
    }
}
