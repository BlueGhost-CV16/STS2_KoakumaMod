using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Cards;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class OtherworldGrandSummonPower : KoakumaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var options = KoakumaMechanics.RandomOtherCharacterCards(player, 3, false);
        var selected = await KoakumaMechanics.ChooseGeneratedCards(choiceContext, options, player, Amount);
        foreach (var card in selected)
        {
            card.SetToFreeThisTurn();
            CardCmd.ApplyKeyword(card, CardKeyword.Exhaust);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        }
    }
}
