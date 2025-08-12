using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
public static class AsyncUtils
{
    public static Task WaitAsync(float waitTime) => WaitAsync(waitTime, CancellationToken.None);
	
    public static async Task WaitAsync(float waitTime, CancellationToken cancellationToken)
    {
        float endTime = Time.time + waitTime;
        while (Time.time < endTime)
        {
            if(cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            else
                await Task.Yield();
        }
    }
	
    public static async Task WaitRealtimeAsync(float duration) => await Task.Delay((int)(duration * 1000));
    
    public static ManualAwaiter GetAwaiter(this YieldInstruction instructuion) => ExtensionsHelper.GetAwaiterForInstuction(instructuion);
}

public static class ExtensionsHelper
{
    internal static ManualAwaiter GetAwaiterForInstuction(object instruction)
    {
        var awaiter = new ManualAwaiter();

        if (ContextHelper.IsMainThread)
            RoutineHelper.Instance.StartCoroutine(WaitForInstructionAndRunContinuation(instruction, awaiter));
        else
            ContextHelper.UnitySynchronizationContext.Post((state) =>
            {
                RoutineHelper.Instance.StartCoroutine(WaitForInstructionAndRunContinuation(instruction, awaiter));
            }, null);

        return awaiter;
    }
    
    private static IEnumerator WaitForInstructionAndRunContinuation(object instruction, ManualAwaiter awaiter)
    {
        yield return instruction;
        awaiter.RunContinuation();
    }

}

public class RoutineHelper : MonoBehaviour
{
    /// <summary>
    /// Return instance of this class.
    /// </summary>
    public static RoutineHelper Instance { get; private set; }

    /// <summary>
    /// Create and save one instance of this class (singleton pattern). <br/>
    /// Created object will not be visible in hierarchy and do not destroyed between scenes.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstance()
    {
        Instance = new GameObject("RoutineHelper (Awaiters)").AddComponent<RoutineHelper>();
        Instance.gameObject.hideFlags = HideFlags.HideInHierarchy;

        DontDestroyOnLoad(Instance.gameObject);
    }
}

internal static class ContextHelper 
{
    /// <summary>
    /// Main thread ID.
    /// </summary>
    internal static int MainThreadID { get; private set; }

    /// <summary>
    /// Synchronization context which created by Unity for main thread.
    /// </summary>
    internal static SynchronizationContext UnitySynchronizationContext { get; private set; }

    /// <summary>
    /// Is the current thread is main?
    /// </summary>
    internal static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == MainThreadID;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void SaveContext()
    {
        MainThreadID = Thread.CurrentThread.ManagedThreadId;
        UnitySynchronizationContext = SynchronizationContext.Current;
    }
}

