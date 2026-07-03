using System.Collections.Concurrent;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace BG_Koakuma.Vfx;

internal static class KoakumaVfx
{
    public const string JackBombThrowScenePath = $"{Entry.ResPath}/scenes/vfx/vfx_jack_bomb_throw.tscn";

    private static readonly ConcurrentDictionary<string, PackedScene> ModSceneCache = new();

    public static void PreloadScenes()
    {
        LoadScene(JackBombThrowScenePath);
    }

    public static async Task PlayMagicBombThrowAsync(Creature owner, Creature target)
    {
        if (TestMode.IsOn)
        {
            return;
        }

        var combatRoom = NCombatRoom.Instance;
        var ownerNode = combatRoom?.GetCreatureNode(owner);
        var targetNode = combatRoom?.GetCreatureNode(target);
        var parent = combatRoom?.CombatVfxContainer;
        if (ownerNode == null || targetNode == null || parent == null)
        {
            return;
        }

        var direction = targetNode.VfxSpawnPosition.X >= ownerNode.GlobalPosition.X ? 1.0f : -1.0f;
        var start = ownerNode.GlobalPosition + new Vector2(90.0f * direction, -170.0f);
        var end = targetNode.VfxSpawnPosition + new Vector2(0.0f, -20.0f);

        var vfx = GenVfxNode<NJackBombThrowVfx>(JackBombThrowScenePath);
        parent.AddChildSafely(vfx);
        await vfx.PlayUntilImpactAsync(start, end, new Color(1.0f, 0.52f, 0.18f));
    }

    public static T GenVfxNode<T>(string scenePath) where T : Node2D
    {
        if (ModSceneCache.TryGetValue(scenePath, out var modScene))
        {
            return modScene.Instantiate<T>(PackedScene.GenEditState.Disabled);
        }

        return PreloadManager.Cache.GetScene(scenePath).Instantiate<T>(PackedScene.GenEditState.Disabled);
    }

    private static void LoadScene(string scenePath)
    {
        if (ModSceneCache.ContainsKey(scenePath))
        {
            return;
        }

        var scene = ResourceLoader.Load<PackedScene>(scenePath, string.Empty, ResourceLoader.CacheMode.Reuse);
        if (scene != null)
        {
            ModSceneCache[scenePath] = scene;
        }
    }
}
