using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaMagicChant : KoakumaUncommonCard
{
    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [KeywordTip(CardKeyword.Retain), .. PowerExtraTips<GainMagicNextTurnPower>()];

    public BG_KoakumaMagicChant() : base(1, CardType.Skill) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        MagicVar("NextTurnMagic", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaSingleTurnRetainPower.Retain(choiceContext, Owner, this);
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, Amount("Energy"), Owner.Creature, this);
        await PowerCmd.Apply<GainMagicNextTurnPower>(choiceContext, Owner.Creature, Amount("NextTurnMagic"), Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Energy", 1);
        UpgradeAmount("NextTurnMagic", 1);
    }
}
