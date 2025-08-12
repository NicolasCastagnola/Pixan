using System;
using UnityEditor;
using UnityEngine;
public class SourceSelectionWindow : EditorWindow
{
    private GameObject[] _selection;
    private Action<GameObject> _selectionAction;
    private Vector2 _scrollPosition;

    public static SourceSelectionWindow OpenWindow(GameObject[] selection, Action<GameObject> selectionCallback)
    {
        SourceSelectionWindow window = GetWindow<SourceSelectionWindow>();
        window.titleContent = new GUIContent("Select Source");
        window._selection = selection;
        window._selectionAction = selectionCallback;
        window.minSize = new Vector2(200, CalculateWindowHeight(selection.Length));
        window.maxSize = new Vector2(1000, CalculateWindowHeight(selection.Length));
        window.ShowModal();
        return window;
    }

    private static float CalculateWindowHeight(int itemCount)
    {
        // Calculate the required height of the window based on the number of items and the height of each item
        float itemHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        return itemCount * itemHeight + 60; // add some extra space for padding and buttons
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(20);
        
        // Create a scroll view for the list
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
        EditorGUI.indentLevel++;

        // Display the list of selected GameObjects with buttons for selection
        for (int i = 0; i < _selection.Length; i++)
        {
            if (GUILayout.Button(_selection[i].name))
            {
                _selectionAction?.Invoke(_selection[i]);
                Close();
                break;
            }
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(20);
    }
}
