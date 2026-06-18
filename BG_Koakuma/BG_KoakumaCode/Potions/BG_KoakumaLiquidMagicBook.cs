using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BG_Koakuma.Cards;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Potions;

[RegisterPotion(typeof(BG_KoakumaPotionPool), StableEntryStem = "BG_KOAKUMA_LIQUID_MAGIC_BOOK")]
public sealed class BG_KoakumaLiquidMagicBook : KoakumaPotion
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.MagicBook];

    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

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
