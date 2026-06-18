using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaLibraryLibrarian : KoakumaUncommonCard
{
    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [PowerTip<LibraryLibrarianPower>()];

    public BG_KoakumaLibraryLibrarian() : base(1, CardType.Power) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<LibraryLibrarianPower>("Power", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LibraryLibrarianPower>(choiceContext, Owner.Creature, Amount("Power"), Owner.Creature, this);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}
