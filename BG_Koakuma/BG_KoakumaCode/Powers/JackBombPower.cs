using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using BG_Koakuma.Cards;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class JackBombPower : KoakumaPower
{
    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [CardTip<BG_KoakumaMagicBomb>()];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay ||
            cardPlay.Card.Owner != Owner.Player ||
            //cardPlay.Card is BG_KoakumaMagicBomb ||
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

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
