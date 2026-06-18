using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using BG_Koakuma.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

[RegisterCard(typeof(BG_KoakumaCardPool))]
[RegisterCharacterStarterCard(typeof(BG_KoakumaCharacter), 1, Order = 4)]
public sealed class BG_KoakumaMagicBookOrganize : KoakumaCard, IKoakumaInterpretableCard
{
    private const int BaseEnergyCost = 0;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRarityValue = CardRarity.Basic;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    public BG_KoakumaMagicBookOrganize() : base(BaseEnergyCost, CardKind, CardRarityValue, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await KoakumaMechanics.Read(choiceContext, this);

        if (KoakumaMechanics.ConsumeInterpret(this))
        {
            await KoakumaMechanics.Read(choiceContext, this);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == this && oldPileType == PileType.Hand && Pile?.Type is not (PileType.Hand or PileType.Play))
        {
            KoakumaMechanics.ClearInterpret(this);
        }
        return Task.CompletedTask;
    }
}
