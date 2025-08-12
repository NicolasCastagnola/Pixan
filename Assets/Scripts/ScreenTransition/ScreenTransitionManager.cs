using System.Collections;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityRandom = UnityEngine.Random;

public class ScreenTransitionManager : BaseMonoSingleton<ScreenTransitionManager>
{
    private static IScreenTransition _activeTransition;
    private static bool _destroyTransitionAfterReplace; 
    
    public static bool InTransition { get; private set; }
    private static IScreenTransition _defaultTransition;

    protected override void Awake()
    {
        base.Awake();
        _defaultTransition = GetComponentInChildren<IScreenTransition>();
    }

    [Button]
    public static void Set(IScreenTransition transition)
    {
        if (_destroyTransitionAfterReplace && _activeTransition is MonoBehaviour monoBehaviour)
        {
            Destroy(monoBehaviour.gameObject);
            _destroyTransitionAfterReplace = false;
        }
        _activeTransition = transition;
    }

    [Button]
    public static YieldInstruction Set(AssetReferenceGameObject assetReference)
    {
        IEnumerator Process()
        {
            var op = assetReference.InstantiateAsync();
            yield return op;
            Set(op.Result.GetComponent<IScreenTransition>());
            _destroyTransitionAfterReplace = true;
        }
        return  Instance.StartCoroutine(Process());
    }
    
    [Button]
    public static YieldInstruction Show(Action callback = null)
    {
        Assert.IsFalse(InTransition);
        Assert.IsTrue(_activeTransition != null || _defaultTransition != null);

        InTransition = true;
        _activeTransition ??= _defaultTransition;
        
        return Instance.StartCoroutine(PlayShow(_activeTransition.Show(), callback));
    }

    [Button]
    public static YieldInstruction Hide(Action callback = null)
    {
        Assert.IsTrue(InTransition);
        InTransition = false;
        return Instance.StartCoroutine(PlayHide(_activeTransition.Hide(), callback));
    }

    private static IEnumerator PlayShow(YieldInstruction transitionInstruction, Action callback)
    {
        GameManager.ShowLog($"ScreenTransitionManager.Show() > Start");
        yield return transitionInstruction;
        callback?.Invoke();
        GameManager.ShowLog($"ScreenTransitionManager.Show() > End");
    }
    
    private static IEnumerator PlayHide(YieldInstruction transitionInstruction, Action callback)
    {
        GameManager.ShowLog($"ScreenTransitionManager.Hide() > Start");
        yield return transitionInstruction;
        InTransition = false;
        callback?.Invoke();
        GameManager.ShowLog($"ScreenTransitionManager.Hide() > End");
    }

    [Button]
    public static void SetProgress(float progress) => _activeTransition.SetProgress(progress);
}

public interface IScreenTransition
{
    public YieldInstruction Show();
    public YieldInstruction Hide();
    public void SetProgress(float progress);
}


