using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaReverseReading : KoakumaInterpretableUncommonCard, IKoakumaOnInterpretResolved
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.Interpret];

    public BG_KoakumaReverseReading() : base(1, CardType.Skill) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [AmountVar("Show", 3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var candidates = PileType.Draw.GetPile(Owner).Cards.TakeLast(Amount("Show")).ToList();
        if (candidates.Count == 0)
        {
            return;
        }

        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, candidates, Owner, canSkip: true);
        if (selected != null)
        {
            await CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Bottom);
            await KoakumaMechanics.MarkInterpretedAndResolve(choiceContext, selected);
        }
    }

    public Task ResolveInterpret(PlayerChoiceContext choiceContext)
    {
        if (KoakumaMechanics.ConsumeInterpret(this))
        {
            EnergyCost.AddUntilPlayed(-1, reduceOnly: true);
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade() => UpgradeAmount("Show", 1);
}
