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
public sealed class BG_KoakumaObsidianCatalog : KoakumaMagicBookCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];

    public BG_KoakumaObsidianCatalog() : base(true) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("IncludeDrawPile", 0),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var card in PileType.Hand.GetPile(Owner).Cards.Where(card => card != this && card is IKoakumaMagicBookCard).ToList())
        {
            await KoakumaMechanics.TransformToTrueName(card);
        }
        if (Amount("IncludeDrawPile") > 0)
        {
            foreach (var card in PileType.Draw.GetPile(Owner).Cards.Where(card => card is IKoakumaMagicBookCard).ToList())
            {
                await KoakumaMechanics.TransformToTrueName(card);
            }
        }
        await InterpretDraw(choiceContext);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("IncludeDrawPile", 1);
    }
}
