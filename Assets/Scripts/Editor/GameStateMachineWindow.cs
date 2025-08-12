using System;
using System.Collections.Generic;
using GameStateMachineCore;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class GameStateMachineWindow : OdinMenuEditorWindow
{
    [MenuItem("GameStateMachine/Inspector",false,0)]
    public static void OpenWindow()
    {
        GameStateMachineWindow window = (GameStateMachineWindow)GetWindow(typeof(GameStateMachineWindow));
        window.titleContent = new GUIContent("Game States Inspector");
        window.Show();
    }
    
    protected override OdinMenuTree BuildMenuTree()
    {
        void AddToTree(Stack<object> states, OdinMenuTree odinMenuTree, string basePath)
        {
            foreach (Object obj in states)
            {
                var type = obj.GetType();
                odinMenuTree.Add($"{basePath}/{type.Name}", obj);
            }
        }

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            return new OdinMenuTree();
        
        var tree = new OdinMenuTree(false, new OdinMenuTreeDrawingConfig() { DrawSearchToolbar = true});

        foreach (var pair in GameStatesManager.ActiveStateMachines)
        {
            string basePath = pair.Key.GetType().ToString();
            Stack<Object> states = new Stack<Object>();
            foreach (var baseGameState in pair.Value)
                states.Push(baseGameState);

            AddToTree(states, tree, basePath);
        }

        foreach (var pair in AsyncStateMachineObserver.ActiveStateMachines)
        {
            string basePath = pair.Key.GetType().ToString();
            Stack<Object> states = new Stack<Object>();
            foreach (var baseGameState in pair.Value)
                states.Push(baseGameState);

            AddToTree(states, tree, basePath);
        }
        
        return tree;
    }

    private void Awake() => Register();

    protected override void OnDisable()
    {
        base.OnDisable();
        Unregister();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Register();
    }

    private void PlayModeStateChange(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case UnityEditor.PlayModeStateChange.EnteredEditMode:
                Rebuild();
                break;
            case UnityEditor.PlayModeStateChange.ExitingEditMode:
                break;
            case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                Rebuild();
                break;
            case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
        }
    }

    private void Register()
    {
        EditorApplication.playModeStateChanged -= PlayModeStateChange;
        EditorApplication.playModeStateChanged += PlayModeStateChange;
        BaseGameState.OnAnySwitch -= Rebuild;
        BaseGameState.OnAnySwitch += Rebuild;

        AsyncStateMachineObserver.OnUpdate -= Rebuild;
        AsyncStateMachineObserver.OnUpdate += Rebuild;
    }
    
    private void Unregister()
    {
        BaseGameState.OnAnySwitch -= Rebuild;
        EditorApplication.playModeStateChanged -= PlayModeStateChange;
        AsyncStateMachineObserver.OnUpdate -= Rebuild;

    }

    void Rebuild()
    {
        ForceMenuTreeRebuild();
        Repaint();
    }
}
