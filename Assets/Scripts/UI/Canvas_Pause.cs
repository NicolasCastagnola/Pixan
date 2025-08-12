using System;
using System.Collections;
using SaveHandler;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Canvas_Pause : BaseMonoSingleton<Canvas_Pause>
{
    public event Action OnResumePlaying, OnRestart, OnBackToMenu;

    [SerializeField] private AnimatedContainer mainContainer;

    [SerializeField] private Options options;

    [SerializeField] private UnityEvent onPause;
    [SerializeField] private UnityEvent onResume;

    public static bool IsPlaying = true;

    public void Initialize(Action onResumePlaying, Action onBackToMenu, Action onRestart)
    {
        IsPlaying = false;


        OnResumePlaying = onResumePlaying;
        OnResumePlaying += () => IsPlaying = true;
        OnRestart = onRestart;
        OnRestart += () => IsPlaying = true;
        OnBackToMenu = onBackToMenu;
        OnBackToMenu += () => IsPlaying = true;


        options.SetVolume(Canvas_PSXFilter.Instance.Volume.GetComponent<VolumeConfig>());
        mainContainer.Show();
        StartCoroutine(options.WaitLoadOptions());
    }
    public void Terminate()
    {
        mainContainer.Hide();

        OnResumePlaying = null;
        OnRestart = null;
        OnBackToMenu = null;
    }
    private void LateUpdate()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
            
        Resume();
    }
    public void Resume()
    {
        if (Canvas_Inventory.isOpen || Canvas_Playing.InBonfireMenu) return;

        OnResumePlaying?.Invoke();
    }
    
    public void RestartLevel() => OnRestart?.Invoke();
    public void GoToMenu()
    {
        OnBackToMenu?.Invoke();
    }
    //SaveHandler
    public void SaveOptions() => options.SaveOptions();
    public void LoadOptions() => options.LoadOptions();
    public void UpdateRealTimeVolume() => options.UpdateRealTimeVolume();
    public void UpdateRealTimeBrightness() => options.UpdateRealTimeBrightness();
    public void UpdateRealTimeSaturation() => options.UpdateRealTimeSaturation();
}