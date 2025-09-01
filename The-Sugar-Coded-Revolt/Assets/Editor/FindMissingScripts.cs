using UnityEngine;
using UnityEditor;

public class FindMissingScripts
{
    [MenuItem("Tools/Find Missing Scripts in Project")]
    private static void FindMissing()
    {
        int sceneObjectsChecked = 0;
        int missingScriptsFound = 0;

        // ------------------------
        // Scan all GameObjects in the scene
        // ------------------------
        GameObject[] allSceneObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in allSceneObjects)
        {
            sceneObjectsChecked++;
            Component[] components = go.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.Log(string.Format("Missing script on GameObject in scene: {0}", go.name), go);
                    missingScriptsFound++;
                }
            }
        }

        // ------------------------
        // Scan all prefabs in the project
        // ------------------------
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            Component[] components = prefab.GetComponentsInChildren<Component>(true);

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.Log(string.Format("Missing script in prefab: {0} at path: {1}", prefab.name, path), prefab);
                    missingScriptsFound++;
                }
            }
        }

        Debug.Log(string.Format("Checked {0} scene objects. Found {1} missing scripts in total.", sceneObjectsChecked, missingScriptsFound));
    }
}
