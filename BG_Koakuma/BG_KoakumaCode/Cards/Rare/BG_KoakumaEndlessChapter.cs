using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaEndlessChapter : KoakumaInterpretableRareCard, IKoakumaOnInterpretResolved
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];

    public BG_KoakumaEndlessChapter() : base(4, CardType.Skill) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new EnergyVar(2)
    ];

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        await base.AfterCardChangedPiles(card, oldPileType, clonedBy);
        if (card == this && Pile?.Type == PileType.Hand && oldPileType != PileType.Hand)
        {
            await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), Amount("Cards"), Owner);
            await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Bottom);
        }
    }

    public async Task ResolveInterpret(PlayerChoiceContext choiceContext)
    {
        if (KoakumaMechanics.ConsumeInterpret(this))
        {
            await PlayerCmd.GainEnergy(Amount("Energy"), Owner);
        }
    }

    protected override void OnUpgrade()
    {
        UpgradeAmount("Energy", 1);
        EnergyCost.UpgradeBy(1);
    }
}
