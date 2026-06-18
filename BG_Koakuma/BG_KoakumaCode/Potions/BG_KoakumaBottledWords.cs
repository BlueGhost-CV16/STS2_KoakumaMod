using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BG_Koakuma.Cards;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Potions;

[RegisterPotion(typeof(BG_KoakumaPotionPool), StableEntryStem = "BG_KOAKUMA_BOTTLED_WORDS")]
public sealed class BG_KoakumaBottledWords : ModPotionTemplate
{
    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override PotionAssetProfile AssetProfile => new(
        $"{Entry.ResPath}/images/potions/BG_KoakumaBottledWords.png",
        $"{Entry.ResPath}/images/potions/outline/BG_KoakumaBottledWords.png");

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var owner = Owner;
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        while ((owner.PlayerCombatState?.Hand.Cards.Count ?? CardPile.MaxCardsInHand) < CardPile.MaxCardsInHand)
        {
            var books = KoakumaMechanics.CreateRandomMagicBooks(owner, 1);
            if (books.Count == 0)
            {
                return;
            }

            await KoakumaMechanics.AddGeneratedCardToHand(choiceContext, books[0], owner);
        }
    }
}
