using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BG_Koakuma.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class JackBombPower : KoakumaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay ||
            cardPlay.Card.Owner != Owner.Player ||
            cardPlay.Card is BG_KoakumaMagicBomb ||
            cardPlay.Card.EnergyCost.GetWithModifiers(CostModifiers.Local) != 0)
        {
            return;
        }

        var enemy = CombatState.HittableEnemies.OrderBy(_ => Owner.Player.RunState.Rng.CombatCardSelection.NextInt()).FirstOrDefault();
        if (enemy == null)
        {
            return;
        }

        for (var i = 0; i < Amount; i++)
        {
            await KoakumaMechanics.AutoPlayMagicBomb(choiceContext, Owner.Player, enemy);
        }
    }
}
