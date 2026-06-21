using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaMagicBookCardPool))]
public sealed class BG_KoakumaLapisFantasyLibrary : KoakumaDerivedCard
{
    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [PowerTip<LapisFantasyLibraryPower>()];

    public BG_KoakumaLapisFantasyLibrary() : base(0, CardType.Power, showInCardLibrary: true) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<LapisFantasyLibraryPower>("Power", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LapisFantasyLibraryPower>(choiceContext, Owner.Creature, Amount("Power"), Owner.Creature, this);
    }
}
