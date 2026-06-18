using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using BG_Koakuma.Cards;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Characters;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Potions;

[RegisterPotion(typeof(BG_KoakumaPotionPool), StableEntryStem = "BG_KOAKUMA_MAGIC_POTION")]
public sealed class BG_KoakumaMagicPotion : KoakumaPotion
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        SecondaryResourceVars.For("Magic", KoakumaMagic.MagicId, 5)
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        NCombatRoom.Instance?.PlaySplashVfx(target, new Color("ff9acb"));
        await KoakumaMagic.Gain(target.Player!, DynamicVars["Magic"].IntValue, this);
    }
}
