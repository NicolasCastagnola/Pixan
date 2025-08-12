using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ComponentSelectionWindow : EditorWindow
{
    private string[] stringList;
    private List<int> selectedIndices = new List<int>();
    private Action<int[]> selectionAction;

    private string searchFilter = "";
    private Vector2 scrollPosition = Vector2.zero;

    public static void OpenWindow(string[] strings, Action<int[]> selectionCallback)
    {
        ComponentSelectionWindow window = GetWindow<ComponentSelectionWindow>();
        window.titleContent = new GUIContent("Example Editor Window");
        window.stringList = strings;
        window.selectionAction = selectionCallback;
        window.minSize = new Vector2(200, CalculateWindowHeight(strings.Length));
        window.Show();
    }

    private static float CalculateWindowHeight(int itemCount)
    {
        // Calculate the required height of the window based on the number of items and the height of each item
        float itemHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        return itemCount * itemHeight + 60; // add some extra space for padding and buttons
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        EditorGUILayout.Space(20);

        // Search bar
        GUILayout.BeginHorizontal();
        searchFilter = EditorGUILayout.TextField(searchFilter, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        {
            // Clear the search filter
            searchFilter = "";
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Display the list of strings with checkboxes for selection
        int visibleItemCount = 0;
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < stringList.Length; i++)
        {
            if (!string.IsNullOrEmpty(searchFilter) && !stringList[i].ToLower().Contains(searchFilter.ToLower()))
            {
                // Skip items that don't match the search filter
                continue;
            }

            visibleItemCount++;

            bool selected = selectedIndices.Contains(i);
            selected = EditorGUILayout.ToggleLeft(stringList[i], selected);
            if (selected)
            {
                if (!selectedIndices.Contains(i))
                {
                    selectedIndices.Add(i);
                }
            }
            else
            {
                selectedIndices.Remove(i);
            }
        }
        GUILayout.EndScrollView();

        if (visibleItemCount == 0)
        {
            EditorGUILayout.HelpBox("No items match the search filter.", MessageType.Info);
        }

        EditorGUILayout.Space(20);

        // Button to clear the selection
        if (GUILayout.Button("Clear Selection"))
        {
            selectedIndices.Clear();
        }
        
        // Button to clear the selection
        if (GUILayout.Button("Select All"))
        {
            selectedIndices.Clear();
            for (int i = 0; i < stringList.Length; i++)
                selectedIndices.Add(i);
        }

        // Button to execute the selection action with the selected indices
        if (GUILayout.Button("Execute Selection Action") && selectedIndices.Count > 0)
        {
            selectionAction?.Invoke(selectedIndices.ToArray());
            Close();
        }

        GUILayout.EndVertical();
    }
}

