using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using BG_Koakuma.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class ReverseIndexPower : KoakumaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var candidates = PileType.Draw.GetPile(player).Cards.TakeLast(3).ToList();
        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, candidates, player, canSkip: true);
        if (selected != null)
        {
            await CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Bottom);
        }
    }
}
