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
public sealed class BG_KoakumaSevenLuminariesElementBook : KoakumaRelic, IKoakumaAfterRead
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Magic, KoakumaHoverTips.Read, KoakumaHoverTips.MagicBook];

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        AmountVar("Reads", 1),
        MagicVar("Magic", 1)
    ];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner)
        {
            return;
        }

        Flash();
        if (Owner.PlayerCombatState?.TurnNumber == 1)
        {
            await ChooseContractToHand(choiceContext, combatState);
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
        {
            return;
        }

        Flash();
        for (var i = 0; i < Amount("Reads"); i++)
        {
            await KoakumaMechanics.Read(choiceContext, Owner, this);
        }
    }

    public async Task AfterRead(PlayerChoiceContext choiceContext, CardModel readCard)
    {
        Flash();
        await GainMagic(choiceContext, Amount("Magic"));
    }

    private async Task ChooseContractToHand(PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        var cards = new CardModel[]
        {
            combatState.CreateCard<BG_KoakumaLittleDevilContract>(Owner),
            combatState.CreateCard<BG_KoakumaLibrarianContract>(Owner)
        };

        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner);
        if (selected != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, Owner);
        }
    }
}
