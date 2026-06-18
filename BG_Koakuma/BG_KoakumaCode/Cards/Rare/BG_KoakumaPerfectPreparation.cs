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
public sealed class BG_KoakumaPerfectPreparation : KoakumaRareCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic, KoakumaHoverTips.MagicBookCollection, KoakumaHoverTips.Read];

    public BG_KoakumaPerfectPreparation() : base(0, CardType.Skill) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        MagicVar("Magic", 1),
        new EnergyVar(1),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.GainMagic(Owner, Amount("Magic"), this);
        await PlayerCmd.GainEnergy(Amount("Energy"), Owner);
        await KoakumaMechanics.CollectMagicBook(choiceContext, this);
        await KoakumaMechanics.Read(choiceContext, this);
        await CardPileCmd.Draw(choiceContext, Amount("Cards"), Owner);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}
