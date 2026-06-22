using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaReread : KoakumaCommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    public BG_KoakumaReread() : base(1, CardType.Skill) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        MagicVar("Magic", 2),
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.GainMagic(Owner, Amount("Magic"), this);
        var drawPile = PileType.Draw.GetPile(Owner);
        foreach (var card in drawPile.Cards.Reverse().Take(Amount("Cards")).ToList())
        {
            await CardPileCmd.Add(card, PileType.Hand, CardPilePosition.Bottom);
        }
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Magic", 1);
    }
}
