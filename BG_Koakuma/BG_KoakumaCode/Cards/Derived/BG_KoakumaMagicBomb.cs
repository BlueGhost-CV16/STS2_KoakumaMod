using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Powers.Mocks;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.CardPools;
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(TokenCardPool))]
public sealed class BG_KoakumaMagicBomb : KoakumaDerivedCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [PowerTip<MagicBurnPower>()];

    public BG_KoakumaMagicBomb() : base(0, CardType.Attack, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3, ValueProp.Move),
        new PowerVar<MagicBurnPower>("MagicBurn", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await PowerCmd.Apply<MagicBurnPower>(choiceContext, cardPlay.Target, Amount("MagicBurn"), Owner.Creature, this);
        if (Owner.GetRelic<BG_KoakumaImprovedMagicBomb>() is { } improvedMagicBomb)
        {
            await improvedMagicBomb.ApplyBonusMagicBurn(choiceContext, cardPlay.Target, this);
        }
    }

    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        PileType pileType,
        CardPilePosition position)
    {
        return card == this && isAutoPlay
            ? (PileType.None, position)
            : base.ModifyCardPlayResultPileTypeAndPosition(card, isAutoPlay, resources, pileType, position);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        UpgradeAmount("MagicBurn", 1);
        AddKeyword(CardKeyword.Retain);
    }
}
