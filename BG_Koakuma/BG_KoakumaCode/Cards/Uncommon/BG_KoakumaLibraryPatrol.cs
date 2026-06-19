using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BG_Koakuma.Characters;
using BG_Koakuma.Tooltips;
using MegaCrit.Sts2.Core.Entities.Creatures;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
public sealed class BG_KoakumaLibraryPatrol : KoakumaUncommonCard
{
    protected override IEnumerable<string> ExtraKoakumaHoverTipIds => [KoakumaHoverTips.MagicBook];

    public BG_KoakumaLibraryPatrol() : base(1, CardType.Skill) { }
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
            where c != null && c.IsAlive && c.IsPlayer
            select c;
        foreach (var item in enumerable)
        {
            if (item.Player == null)
            {
                continue;
            }

            foreach (var card in KoakumaMechanics.CreateRandomMagicBooks(item.Player, Amount("Cards")))
            {
                await KoakumaMechanics.AddGeneratedCardToHand(choiceContext, card, item.Player);
            }
        }
    }

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}
