using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using UnityEditor;

public static class ComponentCopier
{
    private static GameObject _source;
    private static GameObject[] _selection;
    private static Component[] _componentsToCopy;

    private static EditorWindow _activeWindow;
    
    
    [MenuItem("Tools/CopyAndPaste Components", true)]
    static bool CopyAndPasteComponentsValidation() => Selection.gameObjects.Length >= 2;
    
    [MenuItem("Tools/CopyAndPaste Components")]
    static void CopyAndPasteComponents()
    {
        if(_activeWindow) return;
        
        if(Selection.gameObjects.Length <= 1)
            return;

        List<GameObject> gos = new List<GameObject>();
        foreach (var o in Selection.objects)
        {
            if(o is GameObject gameObject)
                gos.Add(gameObject);
        }

        _selection = gos.ToArray();

        if (!GameManager.disableLogs) Debug.LogWarning($"{Selection.activeObject.name}");
        // Open the window for selecting the _source GameObject
        _activeWindow = SourceSelectionWindow.OpenWindow(_selection, OnSourceSelected);
    }

    private static void OnComponentsSelection(int[] selectedIndices)
    {
        for (int i = 0; i < _selection.Length; i++)
        {
            if(_selection[i] == _source) continue;
            foreach (int index in selectedIndices)
            {
                Component componentToCopy = _componentsToCopy[index];
                UnityEditorInternal.ComponentUtility.CopyComponent(componentToCopy);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(_selection[i]);
            } 
        }
        _activeWindow = null;
    }
    
    private static void OnSourceSelected(GameObject source)
    {
        if (source == null)
            return;

        _source = source;
        _componentsToCopy = _source.GetComponents<Component>();
        if (_componentsToCopy.Length == 0)
            return;

        string[] componentTypeNames = new string[_componentsToCopy.Length];
        for (int i = 0; i < _componentsToCopy.Length; i++)
            componentTypeNames[i] = _componentsToCopy[i].GetType().Name;

        if (componentTypeNames.Length == 0)
            return;

        // Open the window for selecting the components to copy
        ComponentSelectionWindow.OpenWindow(componentTypeNames, OnComponentsSelection);
    }
}