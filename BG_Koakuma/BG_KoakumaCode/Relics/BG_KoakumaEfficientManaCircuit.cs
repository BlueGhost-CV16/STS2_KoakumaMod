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
using BG_Koakuma.Tooltips;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Relics;

[RegisterRelic(typeof(BG_KoakumaRelicPool))]
public sealed class BG_KoakumaEfficientManaCircuit : KoakumaRelic, IKoakumaAfterMagicSpent
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic];

    private int _spent;

    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        MagicVar("MagicCost", 8),
        new BlockVar(8, ValueProp.Unpowered)
    ];

    public async Task AfterMagicSpent(PlayerChoiceContext choiceContext, int amount, AbstractModel? source)
    {
        _spent += amount;
        while (_spent >= Amount("MagicCost"))
        {
            _spent -= Amount("MagicCost");
            Flash();
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
        }
    }
}
