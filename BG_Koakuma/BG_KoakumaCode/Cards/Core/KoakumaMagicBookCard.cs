using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Powers.Mocks;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

public abstract class KoakumaMagicBookCard : KoakumaDerivedCard, IKoakumaMagicBookCard
{
    protected KoakumaMagicBookCard(
        bool baseRetain = false,
        CardType cardType = CardType.Skill,
        TargetType targetType = TargetType.Self)
        : base(0, cardType, targetType, baseRetain)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        base.CanonicalKeywords.Concat([ModKeywordRegistry.GetCardKeyword(MagicBookKeywordId)]);
}
