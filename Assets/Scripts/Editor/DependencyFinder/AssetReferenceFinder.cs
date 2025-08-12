using UnityEditor;
using UnityEngine;

public class AssetReferenceFinder : EditorWindow
{
    private string[] dependencies;
    private string[] referencedBy;
    private Vector2 scrollPosition;
    private bool isSearchInProgress = false;

    [MenuItem("Assets/Show Asset Usage", false, 100)]
    private static void ShowAssetUsage()
    {
        Object selectedObject = Selection.activeObject;

        if (selectedObject != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedObject);

            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            System.Collections.Generic.List<string> referencingAssets = new System.Collections.Generic.List<string>();

            AssetReferenceFinder window = GetWindow<AssetReferenceFinder>("Asset Usage");

            window.isSearchInProgress = true;
            EditorUtility.DisplayCancelableProgressBar("Searching Asset References", "Searching...", 0f);

            for (int i = 0; i < allAssets.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Searching Asset References", "Searching...", (float)i / allAssets.Length))
                {
                    // Cancel button pressed
                    EditorUtility.ClearProgressBar();
                    window.isSearchInProgress = false;
                    return;
                }

                string[] dependencies = AssetDatabase.GetDependencies(allAssets[i], false);
                if (System.Array.Exists(dependencies, element => element == assetPath))
                {
                    referencingAssets.Add(allAssets[i]);
                }
            }

            EditorUtility.ClearProgressBar();

            window.referencedBy = referencingAssets.ToArray();
            window.isSearchInProgress = false;
            window.Show();
        }
    }

    private void OnGUI()
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 16;
        headerStyle.margin = new RectOffset(5, 5, 5, 0);

        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.margin = new RectOffset(5, 5, 0, 0);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.margin = new RectOffset(0, 0, 0, 0);
        buttonStyle.fixedWidth = 100;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (isSearchInProgress)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Searching asset references...", headerStyle);
            EditorGUILayout.Space(5);
        }
        else if (referencedBy != null && referencedBy.Length > 0)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Referenced By", headerStyle);
            EditorGUILayout.Space(5);

            foreach (string reference in referencedBy)
            {
                string referenceName = System.IO.Path.GetFileName(reference);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name:", labelStyle, GUILayout.Width(50));
                EditorGUILayout.LabelField(referenceName, labelStyle);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Path:", labelStyle, GUILayout.Width(50));
                EditorGUILayout.LabelField(reference, labelStyle);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(50);
                if (GUILayout.Button("Select", buttonStyle))
                {
                    Object asset = AssetDatabase.LoadAssetAtPath(reference, typeof(Object));
                    Selection.activeObject = asset;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);
            }
        }
        else
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("No assets reference the selected asset.", labelStyle);
            EditorGUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView();
    }
}
