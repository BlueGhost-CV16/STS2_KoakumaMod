using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using BG_Koakuma.Powers;
using STS2RitsuLib;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Scaffolding.Cards.HandOutline;

namespace BG_Koakuma.Cards;

internal static class KoakumaHandOutlines
{
    private static readonly Color InterpretedColor = new("B66DFF");
    private static readonly Color MagicSatisfiedColor = new("FF9A2E");

    public static void Register()
    {
        RitsuLibFramework.CreateContentPack(Entry.ModId)
            .CardHandOutline<KoakumaCard>(ModCardHandOutlineSwitchRule<KoakumaCard>.Switch(
                ResolveColor,
                visibleWhenUnplayable: true))
            .CardHandOutline<BG_KoakumaMysteryMagicBook>(ModCardHandOutlineSwitchRule<BG_KoakumaMysteryMagicBook>.Switch(
                ResolveColor,
                visibleWhenUnplayable: true))
            .Apply();
    }

    public static bool ShouldGlow(CardModel card)
    {
        return ResolveColor(card).HasValue;
    }

    private static Color? ResolveColor(KoakumaCard card)
    {
        return ResolveColor((CardModel)card);
    }

    private static Color? ResolveColor(BG_KoakumaMysteryMagicBook card)
    {
        return ResolveColor((CardModel)card);
    }

    private static Color? ResolveColor(CardModel card)
    {
        if (KoakumaMechanics.IsInterpreted(card))
        {
            return InterpretedColor;
        }

        if (HasSatisfiedCardCondition(card))
        {
            return MagicSatisfiedColor;
        }

        return HasSatisfiedMagicCost(card) ? MagicSatisfiedColor : null;
    }

    private static bool HasSatisfiedCardCondition(CardModel card)
    {
        return card switch
        {
            BG_KoakumaChainBomb => KoakumaMechanics.HasPlayedMagicBombThisTurn(card.Owner),
            BG_KoakumaFuelOnFire => card.Owner.Creature.CombatState?.HittableEnemies.Any(enemy => enemy.GetPower<MagicBurnPower>() != null) == true,
            BG_KoakumaMispageBurst => PileType.Draw.GetPile(card.Owner).Cards.LastOrDefault()?.EnergyCost.GetWithModifiers(CostModifiers.Local) == 0,
            _ => false
        };
    }

    private static bool HasSatisfiedMagicCost(CardModel card)
    {
        if (!card.TryGetSecondaryResourceUses(out var uses))
        {
            return false;
        }

        return uses.Snapshot()
            .Where(static use => use.Kind == SecondaryResourceUseKind.OptionalSpend)
            .Where(static use => string.Equals(use.ResourceId, KoakumaMagic.MagicId, StringComparison.OrdinalIgnoreCase))
            .Any(use => KoakumaMechanics.GetMagic(card.Owner) >= (use.Cost.CostsX ? 1 : use.Cost.Amount));
    }
}
