using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaBombCarnival : KoakumaCommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [CardTip<BG_KoakumaMagicBomb>()];

    public BG_KoakumaBombCarnival() : base(1, CardType.Attack, TargetType.AllEnemies)
    {
        SetMagicCost(3);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("Count", 1),
        MagicVar("MagicCost", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null)
        {
            return;
        }

        var count = Amount("Count");
        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            count++;
        }

        await KoakumaMechanics.AutoPlayMagicBombsOnAllEnemies(choiceContext, Owner, count);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Count", 1);
    }
}
