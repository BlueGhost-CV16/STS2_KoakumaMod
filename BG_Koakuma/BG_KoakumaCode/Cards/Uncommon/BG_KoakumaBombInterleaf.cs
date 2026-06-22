using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaBombInterleaf : KoakumaUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [CardTip<BG_KoakumaMagicBomb>()];

    public BG_KoakumaBombInterleaf() : base(1, CardType.Skill)
    {
        SetMagicCost(2);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("Count", 3),
        MagicVar("MagicCost", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = CombatState;
        ArgumentNullException.ThrowIfNull(combatState);

        for (var i = 0; i < Amount("Count"); i++)
        {
            var bomb = combatState.CreateCard<BG_KoakumaMagicBomb>(Owner);
            if (KoakumaMechanics.MagicPaid(cardPlay))
            {
                CardCmd.Upgrade(bomb);
            }
            await CardPileCmd.AddGeneratedCardToCombat(bomb, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade() => UpgradeAmount("Count", 1);
}
