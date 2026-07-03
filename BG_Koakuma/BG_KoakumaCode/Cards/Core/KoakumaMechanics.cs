using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using BG_Koakuma.Powers;
using BG_Koakuma.Settings;
using STS2RitsuLib;
using STS2RitsuLib.Combat.SecondaryResources;

namespace BG_Koakuma.Cards;

internal static class KoakumaMechanics
{
    private static readonly HashSet<CardModel> InterpretedCards = new();
    private static readonly Dictionary<Player, int> MagicBookCollections = new();
    private static readonly Dictionary<Player, int> Reads = new();
    private static readonly Dictionary<Player, HashSet<Type>> CollectedIdentifiedMagicBookTypes = new();

    public static void Register()
    {
        RitsuLibFramework.SubscribeLifecycle<CombatStartingEvent>(_ => ClearCombatState(), replayCurrentState: false);
        RitsuLibFramework.SubscribeLifecycle<CombatEndedEvent>(_ => ClearCombatState(), replayCurrentState: false);
    }

    private static void ClearCombatState()
    {
        InterpretedCards.Clear();
        MagicBookCollections.Clear();
        Reads.Clear();
        CollectedIdentifiedMagicBookTypes.Clear();
    }

    public static async Task Read(PlayerChoiceContext choiceContext, CardModel sourceCard)
    {
        await Read(choiceContext, sourceCard.Owner, sourceCard, sourceCard);
    }

    public static async Task Read(PlayerChoiceContext choiceContext, Player owner, AbstractModel source)
    {
        await Read(choiceContext, owner, source, null);
    }

    private static async Task Read(PlayerChoiceContext choiceContext, Player owner, AbstractModel source, CardModel? sourceCard)
    {
        var drawPile = PileType.Draw.GetPile(owner);
        var topCards = drawPile.Cards.Take(3).ToList();
        if (topCards.Count == 0)
        {
            return;
        }

        var selected = await ChooseReadCard(choiceContext, topCards, owner);

        if (selected == null)
        {
            await CompleteRead(choiceContext, owner, null);
            return;
        }

        await CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Bottom);
        await MarkInterpretedAndResolveAfterHandEntry(choiceContext, selected);

        var handCards = PileType.Hand.GetPile(owner).Cards
            //.Where(card => card != selected && card != sourceCard)
            .ToList();
        var returned = await ChooseReadReturnCard(choiceContext, owner, handCards.Contains, source);

        if (returned != null)
        {
            await CardPileCmd.Add(returned, PileType.Draw, CardPilePosition.Bottom);
            ClearInterpret(returned);
            await TriggerAfterReadCardReturned(choiceContext, owner, returned, source);
        }

        await CompleteRead(choiceContext, owner, selected);
    }

    public static async Task CollectMagicBook(PlayerChoiceContext choiceContext, CardModel sourceCard)
    {
        await CollectMagicBook(choiceContext, sourceCard.Owner, sourceCard);
    }

    public static async Task CollectMagicBook(PlayerChoiceContext choiceContext, Player owner, AbstractModel source)
    {
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (KoakumaSettingsPage.EasyMagicBookCollectionEnabled)
        {
            await CollectMagicBookAutomatically(choiceContext, owner, combatState, source);
            return;
        }

        var exhaustPile = PileType.Exhaust.GetPile(owner);
        var candidates = exhaustPile.Cards
            .OrderBy(_ => owner.RunState.Rng.CombatCardSelection.NextInt())
            .Take(7)
            .ToList();

        var selected = candidates.Count == 0
            ? []
            : (await CardSelectCmd.FromCombatPile(
                choiceContext,
                exhaustPile,
                owner,
                new CardSelectorPrefs(new LocString("cards", "BG_KOAKUMA_MECHANIC_COLLECT_EXILE"), 0, Math.Min(3, candidates.Count))
                {
                    Cancelable = true,
                    RequireManualConfirmation = true
                },
                card => candidates.Contains(card))).ToList();

        if (selected.Count == 0)
        {
            var card = combatState.CreateCard(ModelDb.Card<BG_KoakumaMysteryMagicBook>(), owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, owner));
            await ResolveMagicBookCollection(choiceContext, owner, combatState, source);
            return;
        }
        
        await CardPileCmd.RemoveFromCombat(selected);
        var offerCount = Math.Max(1, selected.Count);
        var offeredBooks = CreateRandomMagicBooks(owner, offerCount).ToList();
        if (offeredBooks.Count == 0)
        {
            return;
        }

        var chosenBook = offeredBooks.Count == 1
            ? offeredBooks[0]
            : await CardSelectCmd.FromChooseACardScreen(choiceContext, offeredBooks, owner);

        chosenBook ??= offeredBooks[0];
        RecordCollectedMagicBook(owner, chosenBook.GetType());
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(chosenBook, PileType.Draw, owner, CardPilePosition.Random));
        if (owner.Creature.GetPower<MagicCopierPower>() != null)
        {
            var copiedBook = CreateCardOfType(combatState, chosenBook.GetType(), owner);
            await AddGeneratedCardToHand(choiceContext, copiedBook, owner);
        }

        await ResolveMagicBookCollection(choiceContext, owner, combatState, source);
    }

    private static async Task CollectMagicBookAutomatically(PlayerChoiceContext choiceContext, Player owner, ICombatState combatState, AbstractModel source)
    {
        var removed = ChooseLowestRarityExhaustCard(owner);
        if (removed != null)
        {
            await CardPileCmd.RemoveFromCombat(removed);
        }

        var generatedBook = removed == null
            ? combatState.CreateCard<BG_KoakumaMysteryMagicBook>(owner)
            : CreateRandomMagicBooks(owner, 1).FirstOrDefault()
              ?? combatState.CreateCard<BG_KoakumaMysteryMagicBook>(owner);

        RecordCollectedMagicBook(owner, generatedBook.GetType());
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(generatedBook, PileType.Draw, owner, CardPilePosition.Random));

        if (owner.Creature.GetPower<MagicCopierPower>() != null)
        {
            var copiedBook = CreateCardOfType(combatState, generatedBook.GetType(), owner);
            await AddGeneratedCardToHand(choiceContext, copiedBook, owner);
        }

        await ResolveMagicBookCollection(choiceContext, owner, combatState, source);
    }

    private static CardModel? ChooseLowestRarityExhaustCard(Player owner)
    {
        return PileType.Exhaust.GetPile(owner).Cards
            .GroupBy(card => MagicBookCollectionRarityRank(card.Rarity))
            .OrderBy(group => group.Key)
            .FirstOrDefault()
            ?.OrderBy(_ => owner.RunState.Rng.CombatCardSelection.NextInt())
            .FirstOrDefault();
    }

    private static int MagicBookCollectionRarityRank(CardRarity rarity)
    {
        return rarity switch
        {
            CardRarity.Basic => 1,
            CardRarity.Common => 2,
            CardRarity.Uncommon => 3,
            CardRarity.Rare => 4,
            CardRarity.Ancient => 5,
            _ => 0
        };
    }

    public static int GetMagicBookCollectionCount(Player owner)
    {
        return MagicBookCollections.GetValueOrDefault(owner);
    }

    public static int GetReadCount(Player owner)
    {
        return Reads.GetValueOrDefault(owner);
    }

    public static async Task<bool> SpendMagic(PlayerChoiceContext choiceContext, CardModel sourceCard, int amount)
    {
        return await SpendMagic(choiceContext, sourceCard.Owner, sourceCard, amount, sourceCard);
    }

    public static async Task<bool> SpendMagic(PlayerChoiceContext choiceContext, Player owner, AbstractModel source, int amount)
    {
        return await SpendMagic(choiceContext, owner, source, amount, null);
    }

    private static async Task<bool> SpendMagic(PlayerChoiceContext choiceContext, Player owner, AbstractModel source, int amount, CardModel? sourceCard)
    {
        var spent = await KoakumaMagic.Spend(owner, amount, sourceCard, source);
        return spent;
    }

    public static async Task AfterMagicSpent(PlayerChoiceContext choiceContext, Player owner, int amount, AbstractModel? source)
    {
        foreach (var refund in owner.Creature.GetPowerInstances<ManaRefundPower>().ToList())
        {
            await refund.Refund(choiceContext, amount, source);
        }
        foreach (var recycle in owner.Creature.GetPowerInstances<ManaRecycleEnginePower>().ToList())
        {
            await recycle.RecordSpent(choiceContext, amount, source);
        }
        foreach (var relic in owner.Relics.OfType<IKoakumaAfterMagicSpent>().ToList())
        {
            await relic.AfterMagicSpent(choiceContext, amount, source);
        }
        foreach (var power in owner.Creature.Powers.OfType<IKoakumaAfterMagicSpent>().ToList())
        {
            await power.AfterMagicSpent(choiceContext, amount, source);
        }
    }

    public static bool MagicPaid(CardPlay cardPlay)
    {
        return cardPlay.SecondaryResources().Value(KoakumaMagic.MagicId) > 0;
    }

    public static int GetMagic(Player owner)
    {
        return KoakumaMagic.Get(owner);
    }

    public static Task<int> GainMagic(Player owner, int amount, AbstractModel? source = null)
    {
        if (IsMagicLockedByFinalCycle(owner))
        {
            return Task.FromResult(GetMagic(owner));
        }

        return KoakumaMagic.Gain(owner, amount, source);
    }

    public static Task<int> LoseMagic(Player owner, int amount, AbstractModel? source = null)
    {
        if (IsMagicLockedByFinalCycle(owner))
        {
            return Task.FromResult(GetMagic(owner));
        }

        return KoakumaMagic.Lose(owner, amount, source);
    }

    public static Task<int> SetMagic(Player owner, int amount, AbstractModel? source = null)
    {
        if (IsMagicLockedByFinalCycle(owner))
        {
            if (CanSetMagicWhileFinalCycleLocked(amount, source))
            {
                return SecondaryResourceCmd.Set(owner, KoakumaMagic.MagicId, amount, source);
            }

            return EnsureFinalCycleMagicLock(owner);
        }

        var current = GetMagic(owner);
        return amount >= current
            ? GainMagic(owner, amount - current, source)
            : LoseMagic(owner, current - amount, source);
    }

    private static Task<int> EnsureFinalCycleMagicLock(Player owner)
    {
        return GetMagic(owner) == RhodoniteFinalCyclePower.LockedMagicAmount
            ? Task.FromResult(RhodoniteFinalCyclePower.LockedMagicAmount)
            : SecondaryResourceCmd.Set(owner, KoakumaMagic.MagicId, RhodoniteFinalCyclePower.LockedMagicAmount, owner.Creature.GetPower<RhodoniteFinalCyclePower>());
    }

    private static bool IsMagicLockedByFinalCycle(Player owner)
    {
        return owner.Creature.GetPower<RhodoniteFinalCyclePower>() != null;
    }

    private static bool CanSetMagicWhileFinalCycleLocked(int amount, AbstractModel? source)
    {
        return amount == RhodoniteFinalCyclePower.LockedMagicAmount
            || source is RhodoniteFinalCyclePower;
    }

    public static bool ConsumeInterpret(CardModel card)
    {
        return InterpretedCards.Remove(card);
    }

    public static bool IsInterpreted(CardModel card)
    {
        return InterpretedCards.Contains(card);
    }

    public static bool CanInterpret(CardModel card)
    {
        return card is IKoakumaInterpretableCard;
    }

    public static void MarkInterpreted(CardModel card)
    {
        MarkInterpretedIfSupported(card);
    }

    public static async Task MarkInterpretedAndResolve(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (IsInterpreted(card))
        {
            return;
        }

        if (MarkInterpretedIfSupported(card))
        {
            await TriggerAfterInterpreted(choiceContext, card);
        }
        await ResolveInterpretIfNeeded(choiceContext, card);
    }

    public static Task MarkInterpretedAndResolveAfterHandEntry(PlayerChoiceContext choiceContext, CardModel card)
    {
        return MagicBookCorridorWillHandleHandEntry(card)
            ? Task.CompletedTask
            : MarkInterpretedAndResolve(choiceContext, card);
    }

    public static bool IsMagicBook(CardModel card)
    {
        return card is IKoakumaMagicBookCard || card is BG_KoakumaMysteryMagicBook;
    }

    private static bool MagicBookCorridorWillHandleHandEntry(CardModel card)
    {
        return card.Pile?.Type == PileType.Hand
            && IsMagicBook(card)
            && card.Owner.Creature.GetPower<MagicBookCorridorEtoilePower>() != null;
    }

    public static bool HasCollectedAllIdentifiedMagicBooks(Player owner)
    {
        return CollectedIdentifiedMagicBookTypes.TryGetValue(owner, out var collected)
            && IdentifiedMagicBookTypes.All(collected.Contains);
    }

    public static bool IsBookSpell(CardModel card)
    {
        return BookSpellTypes.Contains(card.GetType());
    }

    public static bool HasPlayedMagicBombThisTurn(Player owner)
    {
        return CombatManager.Instance.History.CardPlaysFinished.Any(entry =>
            entry.HappenedThisTurn(owner.Creature.CombatState) &&
            entry.CardPlay.Card.Owner == owner &&
            entry.CardPlay.Card is BG_KoakumaMagicBomb);
    }

    public static void ClearInterpret(CardModel card)
    {
        InterpretedCards.Remove(card);
    }

    public static async Task DrawIfInterpreted(PlayerChoiceContext choiceContext, CardModel card, int count = 1)
    {
        if (ConsumeInterpret(card))
        {
            await CardPileCmd.Draw(choiceContext, count, card.Owner);
        }
    }

    public static IReadOnlyList<CardModel> CreateRandomMagicBooks(CardModel sourceCard, int count)
    {
        return CreateRandomMagicBooks(sourceCard.Owner, count);
    }

    public static IReadOnlyList<CardModel> CreateRandomMagicBooks(Player owner, int count)
    {
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return [];
        }

        var bookTypes = IdentifiedMagicBookTypes
            .OrderBy(_ => owner.RunState.Rng.CombatCardSelection.NextInt())
            .Take(count)
            .ToList();
        return bookTypes.Select(type => CreateCardOfType(combatState, type, owner)).ToList();
    }

    public static CardModel CreateCardOfType(ICombatState combatState, Type type, Player owner)
    {
        var canonical = ModelDb.GetById<CardModel>(ModelDb.GetId(type));
        return combatState.CreateCard(canonical, owner);
    }

    public static async Task<CardModel?> ChooseGeneratedMagicBook(PlayerChoiceContext choiceContext, CardModel sourceCard, int count)
    {
        var owner = sourceCard.Owner;
        var books = CreateRandomMagicBooks(sourceCard, count);
        if (books.Count == 0)
        {
            return null;
        }

        return books.Count == 1 ? books[0] : await CardSelectCmd.FromChooseACardScreen(choiceContext, books, owner);
    }

    public static async Task ChooseGeneratedMagicBooksToHand(PlayerChoiceContext choiceContext, CardModel sourceCard, int chooseCount, bool markInterpreted)
    {
        var owner = sourceCard.Owner;
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var books = IdentifiedMagicBookTypes.Select(type => CreateCardOfType(combatState, type, owner)).ToList();
        var selected = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            books,
            owner,
            new CardSelectorPrefs(sourceCard.SelectionScreenPrompt, 0, chooseCount)
            {
                Cancelable = true,
                RequireManualConfirmation = true
            });

        foreach (var card in selected)
        {
            await AddGeneratedCardToHand(choiceContext, card, owner, markInterpreted);
        }
    }

    public static async Task ChooseGeneratedMagicBookOffersToHand(PlayerChoiceContext choiceContext, CardModel sourceCard, int offerCount, int offerSize, bool markInterpreted)
    {
        var owner = sourceCard.Owner;
        for (var i = 0; i < offerCount; i++)
        {
            var books = CreateRandomMagicBooks(sourceCard, offerSize);
            if (books.Count == 0)
            {
                return;
            }

            var chosen = books.Count == 1
                ? books[0]
                : await CardSelectCmd.FromChooseACardScreen(choiceContext, books, owner, canSkip: false);
            chosen ??= books[0];

            await AddGeneratedCardToHand(choiceContext, chosen, owner, markInterpreted);
        }
    }

    public static async Task AddGeneratedCardToHand(PlayerChoiceContext choiceContext, CardModel card, Player owner, bool markInterpretedAfterHandEntry = false)
    {
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, owner);
        if (markInterpretedAfterHandEntry)
        {
            await MarkInterpretedAndResolveAfterHandEntry(choiceContext, card);
        }
        if (CanTransformToTrueName(card) && owner.Creature.GetPower<LapisFantasyLibraryPower>() != null)
        {
            await TransformToTrueName(card);
        }
    }

    public static async Task AutoPlayMagicBomb(PlayerChoiceContext choiceContext, Player owner, Creature target, bool skipCardPileVisuals = false)
    {
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var bomb = combatState.CreateCard<BG_KoakumaMagicBomb>(owner);
        await CardCmd.AutoPlay(choiceContext, bomb, target, skipCardPileVisuals: skipCardPileVisuals);
    }

    public static async Task AutoPlayMagicBombsOnAllEnemies(PlayerChoiceContext choiceContext, Player owner, int count)
    {
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }
        
        bool flag = false;
        for (var i = 0; i < count; i++)
        {
            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                await AutoPlayMagicBomb(choiceContext, owner, enemy, flag);
                flag = true;
            }
        }
    }

    public static async Task TransformToTrueName(CardModel original)
    {
        if (original is BG_KoakumaLapisBook)
        {
            await TransformLapisBookToFantasyLibraryIfComplete(original);
            return;
        }

        if (!TrueNameMap.TryGetValue(original.GetType(), out var trueNameType))
        {
            return;
        }

        var wasInterpreted = IsInterpreted(original);
        var replacement = original.Owner.Creature.CombatState == null
            ? null
            : CreateCardOfType(original.Owner.Creature.CombatState, trueNameType, original.Owner);
        if (replacement == null)
        {
            return;
        }

        var result = await CardCmd.Transform(original, replacement);
        if (wasInterpreted && result?.cardAdded != null)
        {
            MarkInterpreted(result.Value.cardAdded);
        }
    }

    public static async Task TransformLapisBookToFantasyLibraryIfComplete(CardModel original)
    {
        if (original is not BG_KoakumaLapisBook || !HasCollectedAllIdentifiedMagicBooks(original.Owner))
        {
            return;
        }

        var replacement = original.Owner.Creature.CombatState == null
            ? null
            : CreateCardOfType(original.Owner.Creature.CombatState, typeof(BG_KoakumaLapisFantasyLibrary), original.Owner);
        if (replacement == null)
        {
            return;
        }

        await CardCmd.Transform(original, replacement);
    }

    public static async Task RestoreTrueNameToNormalIfNeeded(CardModel original)
    {
        if (!NormalNameMap.TryGetValue(original.GetType(), out var normalNameType))
        {
            return;
        }

        var replacement = original.Owner.Creature.CombatState == null
            ? null
            : CreateCardOfType(original.Owner.Creature.CombatState, normalNameType, original.Owner);
        if (replacement == null)
        {
            return;
        }

        if (original.IsUpgraded)
        {
            CardCmd.Upgrade(replacement);
        }

        await CardCmd.Transform(original, replacement);
    }

    public static async Task TransformToTrueName(CardModel original, bool requireInterpreted)
    {
        if (!requireInterpreted || IsInterpreted(original))
        {
            await TransformToTrueName(original);
        }
    }

    public static bool CanTransformToTrueName(CardModel card)
    {
        return TrueNameMap.ContainsKey(card.GetType())
            || card is BG_KoakumaLapisBook && HasCollectedAllIdentifiedMagicBooks(card.Owner);
    }

    public static async Task ChooseGeneratedMagicBookToDrawPile(PlayerChoiceContext choiceContext, CardModel sourceCard, bool addInterpretedCopyToTop, bool upgraded)
    {
        var owner = sourceCard.Owner;
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var books = IdentifiedMagicBookTypes.Select(type => CreateCardOfType(combatState, type, owner)).ToList();
        if (upgraded)
        {
            foreach (var book in books.Where(book => !book.IsUpgraded))
            {
                CardCmd.Upgrade(book);
            }
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            books,
            owner,
            new CardSelectorPrefs(sourceCard.SelectionScreenPrompt, 1)
            {
                Cancelable = true
            })).FirstOrDefault();
        if (selected == null)
        {
            return;
        }

        await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Draw, owner, CardPilePosition.Bottom);
        if (!addInterpretedCopyToTop)
        {
            return;
        }

        var topCopy = CreateCardOfType(combatState, selected.GetType(), owner);
        if (selected.IsUpgraded && !topCopy.IsUpgraded)
        {
            CardCmd.Upgrade(topCopy);
        }

        await CardPileCmd.AddGeneratedCardToCombat(topCopy, PileType.Draw, owner, CardPilePosition.Top);
    }

    public static async Task PutDrawPileMagicBooksOnTop(CardModel sourceCard)
    {
        var owner = sourceCard.Owner;
        foreach (var card in PileType.Draw.GetPile(owner).Cards.Where(IsMagicBook).ToList())
        {
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
        }
    }

    public static async Task SelectZeroCostDrawCardsToHand(PlayerChoiceContext choiceContext, CardModel sourceCard, int count, bool markInterpreted)
    {
        var owner = sourceCard.Owner;
        var drawPile = PileType.Draw.GetPile(owner);
        var selected = await CardSelectCmd.FromCombatPile(
            choiceContext,
            drawPile,
            owner,
            new CardSelectorPrefs(sourceCard.SelectionScreenPrompt, 0, count)
            {
                Cancelable = true,
                RequireManualConfirmation = true
            },
            card => card.EnergyCost.GetWithModifiers(CostModifiers.Local) == 0);

        foreach (var card in selected)
        {
            await CardPileCmd.Add(card, PileType.Hand, CardPilePosition.Bottom);
            if (markInterpreted)
            {
                await MarkInterpretedAndResolveAfterHandEntry(choiceContext, card);
            }
        }
    }

    public static async Task FillHandWithRandomMagicBooks(PlayerChoiceContext choiceContext, CardModel sourceCard, bool upgraded = false)
    {
        var owner = sourceCard.Owner;
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        while ((owner.PlayerCombatState?.Hand.Cards.Count ?? CardPile.MaxCardsInHand) < CardPile.MaxCardsInHand)
        {
            var type = IdentifiedMagicBookTypes[owner.RunState.Rng.CombatCardSelection.NextInt(IdentifiedMagicBookTypes.Length)];
            var card = CreateCardOfType(combatState, type, owner);
            if (upgraded)
            {
                CardCmd.Upgrade(card);
            }

            await AddGeneratedCardToHand(choiceContext, card, owner);
        }
    }

    public static IReadOnlyList<CardModel> RandomCardsOfType(CardModel sourceCard, CardType type, int count, bool upgraded)
    {
        var owner = sourceCard.Owner;
        var pools = owner.UnlockState.CharacterCardPools;
        var candidates = pools
            .Where(pool => pool != owner.Character.CardPool)
            .SelectMany(pool => pool.GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint))
            .Where(card => card.Type == type && card.CanBeGeneratedInCombat && card.Rarity is not CardRarity.Basic and not CardRarity.Token)
            .ToList();

        if (candidates.Count == 0)
        {
            candidates = owner.Character.CardPool.GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
                .Where(card => card.Type == type && card.CanBeGeneratedInCombat && card.Rarity is not CardRarity.Basic and not CardRarity.Token)
                .ToList();
        }

        var generated = CardFactory.GetDistinctForCombat(owner, candidates, count, owner.RunState.Rng.CombatCardGeneration).ToList();
        if (upgraded)
        {
            foreach (var card in generated)
            {
                CardCmd.Upgrade(card);
            }
        }

        return generated;
    }

    public static async Task ExhaustHandCardAndChooseReplacement(
        PlayerChoiceContext choiceContext,
        CardModel sourceCard,
        bool upgradedGenerated,
        int showCount,
        int chooseCount,
        bool useGeneratedChoiceScreen)
    {
        var owner = sourceCard.Owner;
        var exhausted = (await CardSelectCmd.FromHand(
            choiceContext,
            owner,
            new CardSelectorPrefs(sourceCard.SelectionScreenPrompt, 1),
            card => card != sourceCard,
            sourceCard)).FirstOrDefault();
        if (exhausted == null)
        {
            return;
        }

        var type = exhausted.Type;
        await CardCmd.Exhaust(choiceContext, exhausted);

        var options = RandomCardsOfType(sourceCard, type, showCount, upgradedGenerated);
        if (options.Count == 0)
        {
            return;
        }

        var selected = useGeneratedChoiceScreen
            ? await ChooseGeneratedCards(choiceContext, options, owner, Math.Min(chooseCount, options.Count))
            : await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                options,
                owner,
                new CardSelectorPrefs(sourceCard.SelectionScreenPrompt, Math.Min(chooseCount, options.Count)));
        foreach (var card in selected)
        {
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, owner);
        }
    }

    public static IReadOnlyList<CardModel> RandomOtherCharacterCards(CardModel sourceCard, int count, bool upgraded)
    {
        return RandomOtherCharacterCards(sourceCard.Owner, count, upgraded);
    }

    public static IReadOnlyList<CardModel> RandomOtherCharacterCards(Player owner, int count, bool upgraded)
    {
        var candidates = owner.UnlockState.CharacterCardPools
            .Where(pool => pool != owner.Character.CardPool)
            .SelectMany(pool => pool.GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint))
            .Where(card => card.CanBeGeneratedInCombat && card.Rarity is not CardRarity.Basic and not CardRarity.Token)
            .ToList();
        if (candidates.Count == 0)
        {
            candidates = owner.Character.CardPool.GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
                .Where(card => card.CanBeGeneratedInCombat && card.Rarity is not CardRarity.Basic and not CardRarity.Token)
                .ToList();
        }

        var generated = CardFactory.GetDistinctForCombat(owner, candidates, count, owner.RunState.Rng.CombatCardGeneration).ToList();
        if (upgraded)
        {
            foreach (var card in generated)
            {
                CardCmd.Upgrade(card);
            }
        }

        return generated;
    }

    public static async Task<IReadOnlyList<CardModel>> ChooseGeneratedCards(
        PlayerChoiceContext choiceContext,
        IReadOnlyList<CardModel> options,
        Player owner,
        int chooseCount,
        bool canSkip = true)
    {
        var selected = new List<CardModel>();
        var remaining = options.ToList();
        var offerStart = 0;
        while (selected.Count < chooseCount && remaining.Count > 0 && offerStart < remaining.Count)
        {
            var offered = remaining
                .Skip(offerStart)
                .Take(3)
                .ToList();
            var chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, offered, owner, canSkip: canSkip);
            if (chosen == null)
            {
                offerStart += canSkip ? offered.Count : 0;
                continue;
            }

            selected.Add(chosen);
            remaining.Remove(chosen);
            offerStart = 0;
        }

        return selected;
    }

    public static async Task<IReadOnlyList<CardModel>> ChooseCardsWithGeneratedScreenWhenPossible(
        PlayerChoiceContext choiceContext,
        IReadOnlyList<CardModel> options,
        Player owner,
        LocString prompt,
        int chooseCount,
        bool requireManualConfirmation = false)
    {
        if (chooseCount == 1 && options.Count is > 0 and <= 3)
        {
            var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, owner, canSkip: true);
            return selected == null ? [] : [selected];
        }

        var cappedChooseCount = Math.Min(chooseCount, options.Count);
        var prefs = requireManualConfirmation
            ? new CardSelectorPrefs(prompt, 0, cappedChooseCount) { Cancelable = true, RequireManualConfirmation = true }
            : new CardSelectorPrefs(prompt, cappedChooseCount) { Cancelable = true };

        return (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            options,
            owner,
            prefs)).ToList();
    }

    public static async Task MoveRandomExhaustCardsToHand(PlayerChoiceContext choiceContext, CardModel sourceCard, int showCount, int chooseCount, Func<CardModel, bool>? filter = null)
    {
        var owner = sourceCard.Owner;
        var exhaustPile = PileType.Exhaust.GetPile(owner);
        var candidates = exhaustPile.Cards
            .Where(card => card != sourceCard && (filter?.Invoke(card) ?? true))
            .OrderBy(_ => owner.RunState.Rng.CombatCardSelection.NextInt())
            .Take(showCount)
            .ToList();
        if (candidates.Count == 0)
        {
            return;
        }

        var selected = await ChooseCardsWithGeneratedScreenWhenPossible(
            choiceContext,
            candidates,
            owner,
            new LocString("cards", "BG_KOAKUMA_MECHANIC_RECOVER_EXHAUST"),
            Math.Min(chooseCount, candidates.Count),
            requireManualConfirmation: true);

        foreach (var card in selected)
        {
            await CardPileCmd.Add(card, PileType.Hand, CardPilePosition.Bottom);
        }
    }

    public static async Task SpecialReadAndUpgrade(PlayerChoiceContext choiceContext, CardModel sourceCard, bool upgradeReturned)
    {
        var owner = sourceCard.Owner;
        var drawPile = PileType.Draw.GetPile(owner);
        var topCards = drawPile.Cards.Take(3).ToList();
        if (topCards.Count == 0)
        {
            return;
        }

        var selected = await ChooseReadCard(choiceContext, topCards, owner);

        if (selected == null)
        {
            await CompleteRead(choiceContext, owner, null);
            return;
        }

        CardCmd.Upgrade(selected);
        await CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Bottom);
        await MarkInterpretedAndResolveAfterHandEntry(choiceContext, selected);

        var handCards = PileType.Hand.GetPile(owner).Cards
            //.Where(card => card != selected && card != sourceCard)
            .ToList();
        var returned = await ChooseReadReturnCard(choiceContext, owner, handCards.Contains, sourceCard);

        if (returned != null)
        {
            if (upgradeReturned)
            {
                CardCmd.Upgrade(returned);
            }
            await CardPileCmd.Add(returned, PileType.Draw, CardPilePosition.Bottom);
            ClearInterpret(returned);
            await TriggerAfterReadCardReturned(choiceContext, owner, returned, sourceCard);
        }

        await CompleteRead(choiceContext, owner, selected);
    }

    public static async Task ReadAndRewardReturnedCost(PlayerChoiceContext choiceContext, CardModel sourceCard, int bonusAmount)
    {
        var owner = sourceCard.Owner;
        var drawPile = PileType.Draw.GetPile(owner);
        var topCards = drawPile.Cards.Take(3).ToList();
        if (topCards.Count == 0)
        {
            return;
        }

        var selected = await ChooseReadCard(choiceContext, topCards, owner);
        if (selected == null)
        {
            await CompleteRead(choiceContext, owner, null);
            return;
        }

        await CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Bottom);
        await MarkInterpretedAndResolveAfterHandEntry(choiceContext, selected);

        var returned = await ChooseReadReturnCard(choiceContext, owner, card => card != sourceCard, sourceCard);

        if (returned != null)
        {
            await CardPileCmd.Add(returned, PileType.Draw, CardPilePosition.Bottom);
            ClearInterpret(returned);
            var bonus = Math.Max(0, returned.EnergyCost.GetWithModifiers(CostModifiers.Local)) + bonusAmount;
            await PlayerCmd.GainEnergy(bonus, owner);
            await GainMagic(owner, bonus, sourceCard);
        }

        await CompleteRead(choiceContext, owner, selected);
    }

    public static async Task AddRandomBookSpellToHand(PlayerChoiceContext choiceContext, CardModel sourceCard, bool retainAndExhaust)
    {
        var owner = sourceCard.Owner;
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var type = BookSpellTypes[owner.RunState.Rng.CombatCardSelection.NextInt(BookSpellTypes.Length)];
        var card = CreateCardOfType(combatState, type, owner);
        if (retainAndExhaust)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Retain, CardKeyword.Exhaust);
        }

        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, owner);
    }

    private static Task<CardModel?> ChooseReadCard(PlayerChoiceContext choiceContext, IReadOnlyList<CardModel> topCards, Player owner)
    {
        if (KoakumaSettingsPage.EasyReadLookEnabled)
        {
            return Task.FromResult(topCards.FirstOrDefault());
        }

        return CardSelectCmd.FromChooseACardScreen(choiceContext, topCards, owner, canSkip: true);
    }

    private static async Task<CardModel?> ChooseReadReturnCard(PlayerChoiceContext choiceContext, Player owner, Func<CardModel, bool> predicate, AbstractModel source)
    {
        if (KoakumaSettingsPage.EasyReadPutEnabled)
        {
            return PileType.Hand.GetPile(owner).Cards.FirstOrDefault(predicate);
        }

        return (await CardSelectCmd.FromHand(
            choiceContext,
            owner,
            new CardSelectorPrefs(new LocString("cards", "BG_KOAKUMA_MECHANIC_READ_RETURN"), 1, 1)
            {
                Cancelable = true,
                RequireManualConfirmation = true
            },
            predicate,
            source)).FirstOrDefault();
    }

    private static async Task CompleteRead(PlayerChoiceContext choiceContext, Player owner, CardModel? readCard)
    {
        await TriggerAfterRead(choiceContext, owner, readCard);
        Reads[owner] = GetReadCount(owner) + 1;
    }

    private static async Task TriggerAfterRead(PlayerChoiceContext choiceContext, Player owner, CardModel? readCard)
    {
        var pages = owner.Creature.GetPower<MagicPagesPower>();
        if (pages != null)
        {
            await pages.AfterRead(choiceContext, readCard);
        }
        foreach (var relic in owner.Relics.OfType<IKoakumaAfterRead>().ToList())
        {
            await relic.AfterRead(choiceContext, readCard);
        }
    }

    private static async Task TriggerAfterReadCardReturned(PlayerChoiceContext choiceContext, Player owner, CardModel returned, AbstractModel source)
    {
        foreach (var relic in owner.Relics.OfType<IKoakumaAfterReadCardReturned>().ToList())
        {
            await relic.AfterReadCardReturned(choiceContext, returned, source);
        }
    }

    private static bool MarkInterpretedIfSupported(CardModel card)
    {
        if (card is IKoakumaInterpretableCard)
        {
            if (!InterpretedCards.Add(card))
            {
                return false;
            }

            if (card is IKoakumaOnInterpreted onInterpreted)
            {
                onInterpreted.OnInterpreted();
            }

            return true;
        }

        return false;
    }

    public static async Task TriggerMagicBookCorridorIfNeeded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (IsMagicBook(card) && card.Owner.Creature.GetPower<MagicBookCorridorEtoilePower>() != null)
        {
            await MarkInterpretedAndResolve(choiceContext, card);
        }
    }

    private static async Task TriggerAfterInterpreted(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner.Creature.GetPower<ManaAnnotationPower>() is { } manaAnnotation)
        {
            await GainMagic(card.Owner, manaAnnotation.Amount, manaAnnotation);
        }
    }

    private static async Task ResolveInterpretIfNeeded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card is IKoakumaOnInterpretResolved resolved)
        {
            await resolved.ResolveInterpret(choiceContext);
        }
    }

    private static void ReduceMagicBookHeavyStrikeCosts(Player owner)
    {
        foreach (var card in owner.Piles
                     .Where(pile => pile.IsCombatPile)
                     .SelectMany(pile => pile.Cards)
                     .OfType<BG_KoakumaMagicBookHeavyStrike>())
        {
            card.EnergyCost.AddUntilPlayed(-1, reduceOnly: true);
        }
    }

    private static async Task ResolveMagicBookCollection(PlayerChoiceContext choiceContext, Player owner, ICombatState combatState, AbstractModel source)
    {
        MagicBookCollections[owner] = GetMagicBookCollectionCount(owner) + 1;
        ReduceMagicBookHeavyStrikeCosts(owner);

        if (owner.Creature.GetPower<OverreadSyndromePower>() is { } overreadSyndrome)
        {
            await PowerCmd.Apply<MagicBurnPower>(choiceContext, combatState.HittableEnemies, overreadSyndrome.Amount, owner.Creature, source as CardModel);
        }
    }

    private static void RecordCollectedMagicBook(Player owner, Type bookType)
    {
        if (!IdentifiedMagicBookTypes.Contains(bookType))
        {
            return;
        }

        if (!CollectedIdentifiedMagicBookTypes.TryGetValue(owner, out var collected))
        {
            collected = [];
            CollectedIdentifiedMagicBookTypes[owner] = collected;
        }

        collected.Add(bookType);
    }

    private static readonly Type[] IdentifiedMagicBookTypes =
    [
        typeof(BG_KoakumaEmeraldBook),
        typeof(BG_KoakumaRubyBook),
        typeof(BG_KoakumaSapphireBook),
        typeof(BG_KoakumaAmethystBook),
        typeof(BG_KoakumaApatiteBook),
        typeof(BG_KoakumaRhodoniteBook),
        typeof(BG_KoakumaBlackPearlBook),
        typeof(BG_KoakumaFluoriteBook),
        typeof(BG_KoakumaWhitePearlBook),
        typeof(BG_KoakumaGhostCrystalBook),
        typeof(BG_KoakumaObsidianBook),
        typeof(BG_KoakumaOnyxBook),
        typeof(BG_KoakumaAlexandriteBook)
    ];

    private static readonly Dictionary<Type, Type> TrueNameMap = new()
    {
        [typeof(BG_KoakumaEmeraldBook)] = typeof(BG_KoakumaEmeraldExclusion),
        [typeof(BG_KoakumaRubyBook)] = typeof(BG_KoakumaRubyMatch),
        [typeof(BG_KoakumaSapphireBook)] = typeof(BG_KoakumaSapphireProof),
        [typeof(BG_KoakumaAmethystBook)] = typeof(BG_KoakumaAmethystLegend),
        [typeof(BG_KoakumaApatiteBook)] = typeof(BG_KoakumaApatiteSloth),
        [typeof(BG_KoakumaRhodoniteBook)] = typeof(BG_KoakumaRhodoniteIsolation),
        [typeof(BG_KoakumaRhodoniteIsolation)] = typeof(BG_KoakumaRhodoniteFinalCycle),
        [typeof(BG_KoakumaBlackPearlBook)] = typeof(BG_KoakumaBlackPearlCourtship),
        [typeof(BG_KoakumaFluoriteBook)] = typeof(BG_KoakumaFluoriteSloth),
        [typeof(BG_KoakumaFluoriteSloth)] = typeof(BG_KoakumaFluoriteAfterimage),
        [typeof(BG_KoakumaWhitePearlBook)] = typeof(BG_KoakumaWhitePearlFoam),
        [typeof(BG_KoakumaGhostCrystalBook)] = typeof(BG_KoakumaGhostCrystalChain),
        [typeof(BG_KoakumaObsidianBook)] = typeof(BG_KoakumaObsidianCatalog),
        [typeof(BG_KoakumaOnyxBook)] = typeof(BG_KoakumaOnyxAbsence),
        [typeof(BG_KoakumaAlexandriteBook)] = typeof(BG_KoakumaBrilliantAlexandrite)
    };

    private static readonly Dictionary<Type, Type> NormalNameMap = new()
    {
        [typeof(BG_KoakumaEmeraldExclusion)] = typeof(BG_KoakumaEmeraldBook),
        [typeof(BG_KoakumaRubyMatch)] = typeof(BG_KoakumaRubyBook),
        [typeof(BG_KoakumaSapphireProof)] = typeof(BG_KoakumaSapphireBook),
        [typeof(BG_KoakumaAmethystLegend)] = typeof(BG_KoakumaAmethystBook),
        [typeof(BG_KoakumaApatiteSloth)] = typeof(BG_KoakumaApatiteBook),
        [typeof(BG_KoakumaRhodoniteIsolation)] = typeof(BG_KoakumaRhodoniteBook),
        [typeof(BG_KoakumaRhodoniteFinalCycle)] = typeof(BG_KoakumaRhodoniteBook),
        [typeof(BG_KoakumaBlackPearlCourtship)] = typeof(BG_KoakumaBlackPearlBook),
        [typeof(BG_KoakumaFluoriteSloth)] = typeof(BG_KoakumaFluoriteBook),
        [typeof(BG_KoakumaFluoriteAfterimage)] = typeof(BG_KoakumaFluoriteBook),
        [typeof(BG_KoakumaWhitePearlFoam)] = typeof(BG_KoakumaWhitePearlBook),
        [typeof(BG_KoakumaGhostCrystalChain)] = typeof(BG_KoakumaGhostCrystalBook),
        [typeof(BG_KoakumaObsidianCatalog)] = typeof(BG_KoakumaObsidianBook),
        [typeof(BG_KoakumaOnyxAbsence)] = typeof(BG_KoakumaOnyxBook),
        [typeof(BG_KoakumaBrilliantAlexandrite)] = typeof(BG_KoakumaAlexandriteBook)
    };

    private static readonly Type[] BookSpellTypes =
    [
        typeof(BG_KoakumaMagicBookCollect),
        typeof(BG_KoakumaMagicBookOrganize),
        typeof(BG_KoakumaMagicBookWriting),
        typeof(BG_KoakumaMagicBookCompose),
        typeof(BG_KoakumaMagicBookCarry),
        typeof(BG_KoakumaPageBarrier),
        typeof(BG_KoakumaMagicBookOverview)
    ];
}

internal interface IKoakumaInterpretableCard;

internal interface IKoakumaMagicBookCard;

internal interface IKoakumaAfterRead
{
    Task AfterRead(PlayerChoiceContext choiceContext, CardModel? readCard);
}

internal interface IKoakumaAfterReadCardReturned
{
    Task AfterReadCardReturned(PlayerChoiceContext choiceContext, CardModel returned, AbstractModel source);
}

internal interface IKoakumaAfterMagicSpent
{
    Task AfterMagicSpent(PlayerChoiceContext choiceContext, int amount, AbstractModel? source);
}

internal interface IKoakumaOnInterpreted
{
    void OnInterpreted();
}

internal interface IKoakumaOnInterpretResolved
{
    Task ResolveInterpret(PlayerChoiceContext choiceContext);
}
