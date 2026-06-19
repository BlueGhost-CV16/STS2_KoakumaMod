using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMoonlitCycle : KoakumaUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [CardTip<BG_KoakumaMoonlitDance>()];

    public BG_KoakumaMoonlitCycle() : base(2, CardType.Attack, TargetType.AnyEnemy)
    {
        SetMagicCost(2);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(16, ValueProp.Move),
        new DamageVar("BonusDamage", 8, ValueProp.Move),
        MagicVar("MagicCost", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage, Owner.Creature, this);
        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, (DamageVar)DynamicVars["BonusDamage"], Owner.Creature, this);
        }
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        await base.AfterCardChangedPiles(card, oldPileType, clonedBy);
        if (card == this && oldPileType == PileType.Play && Pile?.Type != PileType.Play)
        {
            var result = await CardCmd.TransformTo<BG_KoakumaMoonlitDance>(this);
            if (IsUpgraded && result?.cardAdded != null)
            {
                CardCmd.Upgrade(result.Value.cardAdded);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars["BonusDamage"].UpgradeValueBy(2);
        AddKeyword(CardKeyword.Retain);
    }
}
