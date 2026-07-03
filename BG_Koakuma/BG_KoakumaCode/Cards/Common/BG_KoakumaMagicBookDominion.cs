using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMagicBookDominion : KoakumaInterpretableCommonCard, IKoakumaOnInterpretResolved
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];

    public BG_KoakumaMagicBookDominion() : base(1, CardType.Attack, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var hits = 1 + PileType.Hand.GetPile(Owner).Cards.Count(card => card.EnergyCost.GetWithModifiers(CostModifiers.Local) == 0);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hits)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        KoakumaMechanics.ConsumeInterpret(this);
    }

    public Task ResolveInterpret(PlayerChoiceContext choiceContext)
    {
        if (KoakumaMechanics.ConsumeInterpret(this))
        {
            AddKeyword(CardKeyword.Retain);
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
