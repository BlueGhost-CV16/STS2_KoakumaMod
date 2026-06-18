using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMagicBookWriting : KoakumaInterpretableCommonCard
{
    public BG_KoakumaMagicBookWriting() : base(0, CardType.Skill)
    {
        SetMagicCost(1);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        MagicVar("MagicCost", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.CollectMagicBook(choiceContext, this);
        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            await KoakumaMechanics.CollectMagicBook(choiceContext, this);
        }
        if (KoakumaMechanics.ConsumeInterpret(this))
        {
            await KoakumaMechanics.CollectMagicBook(choiceContext, this);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
