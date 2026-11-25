using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

internal static class RealisticTerrainTrees
{
    private const string OriginalFolder = "Assets/Realistic Tree/Prefabs/";
    private const string TerrainFolder = "Assets/Realistic Tree/Prefabs/Terrain URP/";

    [MenuItem("Tools/Realistic Trees/Repair Terrain Tree Slots")]
    private static void Repair()
    {
        int fixedSlots = 0;
        var changedTerrains = new List<TerrainData>();
        foreach (string guid in AssetDatabase.FindAssets("t:TerrainData", new[] { "Assets" }))
        {
            var data = AssetDatabase.LoadAssetAtPath<TerrainData>(AssetDatabase.GUIDToAssetPath(guid));
            if (data == null)
                continue;

            var prototypes = data.treePrototypes;
            int changes = 0;
            foreach (var prototype in prototypes)
            {
                string original = AssetDatabase.GetAssetPath(prototype.prefab);
                if (!original.StartsWith(OriginalFolder, System.StringComparison.Ordinal) ||
                    original.StartsWith(TerrainFolder, System.StringComparison.Ordinal))
                    continue;

                string name = Path.GetFileNameWithoutExtension(original);
                var replacement = AssetDatabase.LoadAssetAtPath<GameObject>(TerrainFolder + name + " (Terrain).prefab");
                if (replacement == null)
                    continue;

                prototype.prefab = replacement;
                changes++;
            }

            if (changes == 0)
                continue;

            Undo.RegisterCompleteObjectUndo(data, "Repair Terrain Tree Slots");
            data.treePrototypes = prototypes;
            data.RefreshPrototypes();
            EditorUtility.SetDirty(data);
            changedTerrains.Add(data);
            fixedSlots += changes;
        }

        foreach (var data in changedTerrains)
            AssetDatabase.SaveAssetIfDirty(data);

        Debug.Log($"[Realistic Trees] Replaced {fixedSlots} tree slots on {changedTerrains.Count} terrain assets.");
    }
}



