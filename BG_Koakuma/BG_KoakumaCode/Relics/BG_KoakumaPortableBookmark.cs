using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Cards;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Relics;

[RegisterRelic(typeof(BG_KoakumaRelicPool))]
public sealed class BG_KoakumaPortableBookmark : KoakumaRelic, IKoakumaAfterRead, IKoakumaAfterReadCardReturned
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, ValueProp.Unpowered)
    ];

    public async Task AfterRead(PlayerChoiceContext choiceContext, CardModel? readCard)
    {
        if (readCard != null)
        {
            await DamageForCardCost(choiceContext, readCard);
        }
    }

    public async Task AfterReadCardReturned(PlayerChoiceContext choiceContext, CardModel returned, AbstractModel source)
    {
        await DamageForCardCost(choiceContext, returned);
    }

    private async Task DamageForCardCost(PlayerChoiceContext choiceContext, CardModel card)
    {
        var cost = Math.Max(1, card.EnergyCost.GetWithModifiers(CostModifiers.Local));

        var enemies = Owner.Creature.CombatState?.HittableEnemies.ToList();
        if (enemies == null || enemies.Count == 0)
        {
            return;
        }

        Flash();
        for (var i = 0; i < cost; i++)
        {
            await CreatureCmd.Damage(choiceContext, enemies, DynamicVars.Damage, Owner.Creature, null, null);
        }
    }
}
