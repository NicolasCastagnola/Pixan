using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender.Examples;

public class DebugUtilitiesWindow : OdinEditorWindow
{
    [MenuItem("Window/Debug Utilities")]
    public static void Init()
    {
        var window = GetWindow<DebugUtilitiesWindow>();
        window.titleContent.text = $"Utilities";
        window.Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        SceneSwitcher.OnScenesLoaded -= SetScenes;
        SceneSwitcher.OnScenesLoaded += SetScenes;
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SceneSwitcher.OnScenesLoaded -= SetScenes;
    }

    [System.Serializable]    
    public struct SceneButtons
    {
        private string _label;
        [ShowInInspector, LabelText("$_label"), ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, Expanded = true)] public QuickButton[] Buttons;

        public SceneButtons(string label, QuickButton[] buttons)
        {
            _label = label;
            Buttons = buttons;
        }
    }
    
    
    [System.Serializable]
    public struct QuickButton
    {
        private Action _callback;
        private string _label;

        public QuickButton(string label, Action callback)
        {
            _label = label;
            _callback = callback;
        }

        [Button(Name = "$_label")]
        void Play() => _callback.Invoke();
    }

    [ShowInInspector, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, Expanded = true)] private List<SceneButtons> _buttons = new List<SceneButtons>();
    public bool centerOnSelection = false;

    public static void SetScenes(Scene[] scenes)
    {
        if(!HasOpenInstances<DebugUtilitiesWindow>())
            return;
        
        var window = GetWindow<DebugUtilitiesWindow>();
        window._buttons.Clear();

        /*
        void AddToButtonsList(Component component, List<QuickButton> list)
        {
            if (component)
                list.Add(new QuickButton(component.GetType().Name, () => SelectObject(component.gameObject)));
        }
        */

        foreach (Scene scene in scenes)
        {
            List<QuickButton> quickButtons = new List<QuickButton>();
            
            /*AddToButtonsList(scene.FindObjectInScene<SceneController>(), quickButtons);
            
            AddToButtonsList(scene.FindObjectInScene<BaseController>(), quickButtons);

            AddToButtonsList(scene.FindObjectInScene<EnemyDirector>(), quickButtons);

            AddToButtonsList(scene.FindObjectInScene<MiningArea>(), quickButtons);*/

            if (quickButtons.Count > 0)
            {
                window._buttons.Add(new SceneButtons(scene.name, quickButtons.ToArray()));
            }
        }
    }
    
    private static void SelectObject(GameObject go)
    {
        Selection.activeGameObject = go;
        SceneView.lastActiveSceneView.LookAt(go.transform.position);
    }

    [Button(Name = "Camera"), ButtonGroup("Quick Selection")]
    public void SelectMainCamera()
    {
        Selection.activeObject = Camera.main;
        if (centerOnSelection && Selection.activeObject) SceneView.lastActiveSceneView.FrameSelected();
    }
}
