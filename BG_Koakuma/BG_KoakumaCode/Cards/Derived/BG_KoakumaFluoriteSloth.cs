using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaMagicBookCardPool))]
public sealed class BG_KoakumaFluoriteSloth : KoakumaMagicBookCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];

    public BG_KoakumaFluoriteSloth() : base(true) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FluoriteSlothPower>("Power", 1),
        new EnergyVar("TurnEnergy", 1),
        new EnergyVar(0),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FluoriteSlothPower>(choiceContext, Owner.Creature, Amount("Power"), Owner.Creature, this);
        if (Amount("Energy") > 0)
        {
            await PlayerCmd.GainEnergy(Amount("Energy"), Owner);
        }
        await InterpretDraw(choiceContext);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Energy", 1);
    }
}
