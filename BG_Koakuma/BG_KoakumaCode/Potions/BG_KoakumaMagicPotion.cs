using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using BG_Koakuma.Cards;
using BG_Koakuma.Characters;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Potions;

[RegisterPotion(typeof(BG_KoakumaPotionPool), StableEntryStem = "BG_KOAKUMA_MAGIC_POTION")]
public sealed class BG_KoakumaMagicPotion : ModPotionTemplate
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;

    public override PotionAssetProfile AssetProfile => new(
        $"{Entry.ResPath}/images/potions/BG_KoakumaMagicPotion.png",
        $"{Entry.ResPath}/images/potions/outline/BG_KoakumaMagicPotion.png");

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
