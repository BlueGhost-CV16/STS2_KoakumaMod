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
public sealed class BG_KoakumaMoonlitDance : KoakumaUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [CardTip<BG_KoakumaMoonlitCycle>()];

    public BG_KoakumaMoonlitDance() : base(2, CardType.Skill)
    {
        SetMagicCost(2);
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12, ValueProp.Move),
        new BlockVar("BonusBlock", 6, ValueProp.Move),
        MagicVar("MagicCost", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        if (KoakumaMechanics.MagicPaid(cardPlay))
        {
            await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars["BonusBlock"], cardPlay);
        }
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        await base.AfterCardChangedPiles(card, oldPileType, clonedBy);
        if (card == this && oldPileType == PileType.Play && Pile?.Type != PileType.Play)
        {
            var result = await CardCmd.TransformTo<BG_KoakumaMoonlitCycle>(this);
            if (IsUpgraded && result?.cardAdded != null)
            {
                CardCmd.Upgrade(result.Value.cardAdded);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
        DynamicVars["BonusBlock"].UpgradeValueBy(2);
        AddKeyword(CardKeyword.Retain);
    }
}
