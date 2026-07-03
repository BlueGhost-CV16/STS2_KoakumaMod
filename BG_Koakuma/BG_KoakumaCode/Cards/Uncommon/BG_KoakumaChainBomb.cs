using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaChainBomb : KoakumaUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.MagicBurn];

    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [CardTip<BG_KoakumaMagicBomb>()];

    public BG_KoakumaChainBomb() : base(0, CardType.Attack, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3, ValueProp.Move),
        new PowerVar<MagicBurnPower>("MagicBurn", 1),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var hadPlayedMagicBomb = KoakumaMechanics.HasPlayedMagicBombThisTurn(Owner);
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage, Owner.Creature, this, cardPlay);
        await PowerCmd.Apply<MagicBurnPower>(choiceContext, cardPlay.Target, Amount("MagicBurn"), Owner.Creature, this);
        if (hadPlayedMagicBomb)
        {
            await CardPileCmd.Draw(choiceContext, Amount("Cards"), Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        UpgradeAmount("MagicBurn", 1);
    }
}
