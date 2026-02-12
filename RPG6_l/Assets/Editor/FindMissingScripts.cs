#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

// Editor utility to find and optionally remove missing script references on GameObjects
public static class FindMissingScripts
{
    [MenuItem("Tools/Missing Scripts/Find in Open Scenes")]
    public static void FindInOpenScenes()
    {
        int sceneCount = EditorSceneManager.sceneCount;
        int found = 0;
        for (int i = 0; i < sceneCount; i++)
        {
            var scene = EditorSceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
                found += ProcessGameObject(root, log:true);
        }
        Debug.Log($"FindMissingScripts: Found {found} missing script references in open scenes.");
    }

    [MenuItem("Tools/Missing Scripts/Find in Project Prefabs")]
    public static void FindInProjectPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:prefab");
        int found = 0;
        foreach (var g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            found += ProcessPrefab(prefab, path);
        }
        Debug.Log($"FindMissingScripts: Found {found} missing script references in project prefabs.");
    }

    [MenuItem("Tools/Missing Scripts/Remove in Open Scenes (Undoable)")]
    public static void RemoveInOpenScenes()
    {
        int sceneCount = EditorSceneManager.sceneCount;
        int removed = 0;
        for (int i = 0; i < sceneCount; i++)
        {
            var scene = EditorSceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
                removed += RemoveMissingOnGameObject(root);
            EditorSceneManager.MarkSceneDirty(scene);
        }
        Debug.Log($"FindMissingScripts: Removed {removed} missing script references in open scenes.");
    }

    [MenuItem("Tools/Missing Scripts/Remove in Project Prefabs (Non-Undoable)")]
    public static void RemoveInProjectPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:prefab");
        int removed = 0;
        foreach (var g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            // editing prefab asset
            GameObject instance = PrefabUtility.LoadPrefabContents(path);
            removed += RemoveMissingOnGameObject(instance);
            PrefabUtility.SaveAsPrefabAsset(instance, path);
            PrefabUtility.UnloadPrefabContents(instance);
        }
        Debug.Log($"FindMissingScripts: Removed {removed} missing script references in project prefabs.");
    }

    private static int ProcessPrefab(GameObject prefab, string assetPath)
    {
        int found = 0;
        // traverse prefab hierarchy
        foreach (Transform t in prefab.GetComponentsInChildren<Transform>(true))
        {
            var go = t.gameObject;
            var comps = go.GetComponents<Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i] == null)
                {
                    Debug.LogWarning($"Missing script in prefab: {assetPath} -> {GetHierarchy(go)}", go);
                    found++;
                }
            }
        }
        return found;
    }

    private static int ProcessGameObject(GameObject root, bool log = false)
    {
        int found = 0;
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            var go = t.gameObject;
            var comps = go.GetComponents<Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i] == null)
                {
                    if (log) Debug.LogWarning($"Missing script on GameObject: {GetHierarchy(go)}", go);
                    found++;
                }
            }
        }
        return found;
    }

    private static int RemoveMissingOnGameObject(GameObject root)
    {
        int removed = 0;
        // iterate all gameobjects
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            var go = t.gameObject;
            // use SerializedObject to remove missing scripts safely for asset prefabs as well
            var so = new SerializedObject(go);
            var prop = so.FindProperty("m_Component");
            if (prop == null) continue;
            for (int i = prop.arraySize - 1; i >= 0; i--)
            {
                var compRef = prop.GetArrayElementAtIndex(i);
                var component = compRef.FindPropertyRelative("component").objectReferenceValue;
                if (component == null)
                {
                    prop.DeleteArrayElementAtIndex(i);
                    removed++;
                }
            }
            so.ApplyModifiedProperties();
        }
        return removed;
    }

    private static string GetHierarchy(GameObject go)
    {
        string path = go.name;
        var t = go.transform;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
#endif
