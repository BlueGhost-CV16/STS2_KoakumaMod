using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaMagicBookCardPool))]
public sealed class BG_KoakumaLapisBook : KoakumaMagicBookCard
{
    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [CardTip<BG_KoakumaLapisFantasyLibrary>()];

    public BG_KoakumaLapisBook() : base(true) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        ModKeywordRegistry.GetCardKeyword(MagicBookKeywordId)
    ];

    protected override bool IsPlayable => false;

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        await base.AfterCardChangedPiles(card, oldPileType, clonedBy);

        if (card == this
            || card.Owner != Owner
            || Pile?.Type != PileType.Hand
            || oldPileType == PileType.Hand
            || card.Pile?.Type != PileType.Hand
            || !KoakumaMechanics.IsMagicBook(card))
        {
            return;
        }

        await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Bottom);
        await KoakumaMechanics.TransformToTrueName(card);
    }
}
