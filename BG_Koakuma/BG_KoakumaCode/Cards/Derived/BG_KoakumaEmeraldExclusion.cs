using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Powers.Mocks;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.CardPools;
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(TokenCardPool))]
public sealed class BG_KoakumaEmeraldExclusion : KoakumaMagicBookCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];

    public BG_KoakumaEmeraldExclusion() : base(true) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>("SelfWeak", 1),
        new PowerVar<VulnerablePower>("SelfVulnerable", 1),
        new PowerVar<WeakPower>("Weak", 3),
        new PowerVar<VulnerablePower>("Vulnerable", 3),
        new BlockVar(13, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WeakPower>(choiceContext, Owner.Creature, Amount("SelfWeak"), Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, Owner.Creature, Amount("SelfVulnerable"), Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(choiceContext, CombatState.HittableEnemies, Amount("Weak"), Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, CombatState.HittableEnemies, Amount("Vulnerable"), Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await InterpretDraw(choiceContext);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Weak", 1);
        UpgradeAmount("Vulnerable", 1);
        DynamicVars.Block.UpgradeValueBy(4);
    }
}
