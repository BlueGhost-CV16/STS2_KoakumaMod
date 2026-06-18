using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaBlankChapter : KoakumaInterpretableUncommonCard
{
    public BG_KoakumaBlankChapter() : base(0, CardType.Skill)
    {
        SetMagicCost(2);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        MagicVar("MagicCost", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, Amount("Cards"), Owner);
        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            await CardPileCmd.Draw(choiceContext, Amount("Cards"), Owner);
        }
        await KoakumaMechanics.DrawIfInterpreted(choiceContext, this);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
