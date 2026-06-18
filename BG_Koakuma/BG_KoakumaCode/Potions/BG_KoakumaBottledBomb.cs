using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using BG_Koakuma.Cards;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Potions;

[RegisterPotion(typeof(BG_KoakumaPotionPool), StableEntryStem = "BG_KOAKUMA_BOTTLED_BOMB")]
public sealed class BG_KoakumaBottledBomb : ModPotionTemplate
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override PotionAssetProfile AssetProfile => new(
        $"{Entry.ResPath}/images/potions/BG_KoakumaBottledBomb.png",
        $"{Entry.ResPath}/images/potions/outline/BG_KoakumaBottledBomb.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var bomb = combatState.CreateCard<BG_KoakumaMagicBomb>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(bomb, PileType.Hand, Owner);
        }
    }
}
