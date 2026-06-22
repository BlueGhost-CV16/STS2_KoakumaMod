using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMagicBookWriting : KoakumaInterpretableUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic, KoakumaHoverTips.Interpret, KoakumaHoverTips.MagicBookCollection];

    public BG_KoakumaMagicBookWriting() : base(0, CardType.Skill)
    {
        SetMagicCost(2);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => IsUpgraded
        ? [CardKeyword.Exhaust, CardKeyword.Retain]
        : [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        MagicVar("MagicCost", 2)
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
        AddKeyword(CardKeyword.Retain);
    }
}
