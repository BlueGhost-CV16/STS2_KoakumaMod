using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Cards;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Relics;

[RegisterRelic(typeof(BG_KoakumaRelicPool))]
[RegisterCharacterStarterRelic(typeof(BG_KoakumaCharacter))]
[RegisterTouchOfOrobasRefinement(typeof(BG_KoakumaSevenLuminariesElementBook))]
public sealed class BG_KoakumaLittleDevilMagicBook : KoakumaRelic, IKoakumaAfterRead
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        AmountVar("Reads", 1),
        MagicVar("Magic", 1)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
        {
            return;
        }

        Flash();
        for (var i = 0; i < Amount("Reads"); i++)
        {
            await KoakumaMechanics.Read(choiceContext, Owner, this);
        }
    }

    public async Task AfterRead(PlayerChoiceContext choiceContext, CardModel readCard)
    {
        Flash();
        await GainMagic(choiceContext, Amount("Magic"));
    }
}
