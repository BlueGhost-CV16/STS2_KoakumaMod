using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaFinalPageEcho : KoakumaInterpretableRareCard
{
    public BG_KoakumaFinalPageEcho() : base(1, CardType.Skill) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var card in PileType.Hand.GetPile(Owner).Cards.Where(card => card != this).ToList())
        {
            await KoakumaMechanics.MarkInterpretedAndResolve(choiceContext, card);
        }

        if (KoakumaMechanics.ConsumeInterpret(this))
        {
            await PlayerCmd.GainEnergy(Amount("Energy"), Owner);
        }
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
