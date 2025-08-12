using UnityEditor;
using UnityEngine;

public class SearchAssetByGUID : EditorWindow
{
    string guid = "";

    [MenuItem("Tools/Search Asset By GUID")]
    static void Init()
    {
        SearchAssetByGUID window = (SearchAssetByGUID)EditorWindow.GetWindow(typeof(SearchAssetByGUID));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Enter GUID of asset to search for:");
        guid = EditorGUILayout.TextField(guid);

        if (GUILayout.Button("Search"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (!string.IsNullOrEmpty(path))
            {
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
            }
            else
            {
                if (!GameManager.disableLogs) Debug.LogError("Asset with GUID " + guid + " not found.");
            }
        }
    }
}