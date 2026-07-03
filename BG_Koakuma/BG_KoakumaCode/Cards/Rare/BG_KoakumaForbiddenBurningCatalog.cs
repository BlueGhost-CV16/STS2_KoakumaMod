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
public sealed class BG_KoakumaForbiddenBurningCatalog : KoakumaRareCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.MagicBookCollection];

    public BG_KoakumaForbiddenBurningCatalog() : base(1, CardType.Attack, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null)
        {
            return;
        }

        var attack = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);

        var hitCount = attack.Results
            .SelectMany(static results => results)
            .Count(static result => result.TotalDamage + result.OverkillDamage > 0);
        for (var i = 0; i < hitCount; i++)
        {
            await KoakumaMechanics.CollectMagicBook(choiceContext, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}
