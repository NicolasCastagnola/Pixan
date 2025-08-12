using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using SaveHandler;

public class PauseMenuComponent : BaseMonoSingleton<PauseMenuComponent>
{
    // public event Action<bool> OnChangePauseStatus;
    // public event Action OnRestart, OnBackToMenu;
    //
    //
    // [SerializeField] Options options;
    //
    // [SerializeField] UIFader background;
    //
    //
    // [SerializeField] UnityEvent onPause;
    // [SerializeField] UnityEvent onResume;
    //
    // GameObject canvas;
    //
    // public static bool isPaused;
    //
    // public void Initialize(Action<bool> onChangePauseStatus, Action onBackToMenu, Action onRestart)
    // {
    //     OnChangePauseStatus = onChangePauseStatus;
    //     OnRestart = onRestart;
    //     OnBackToMenu = onBackToMenu;
    //     
    //     StartCoroutine(options.WaitLoadOptions());
    //     canvas = transform.GetChild(0).gameObject;
    //     
    // }
    // public void PauseMenu(InputAction.CallbackContext ctx)
    // {
    //     if (ctx.started)
    //     {
    //         if (InventoryStatsMenu.isOpen) return;
    //         Pause();
    //     }
    // }
    //
    // public void RestartLevel() => OnRestart?.Invoke();
    //
    // public void Pause()
    // {
    //     if (isPaused)
    //     {
    //         onResume?.Invoke();
    //         canvas.SetActive(false);
    //         OnChangePauseStatus?.Invoke(true);
    //         GameManager.instance.LockCursor(true);
    //     }
    //     else
    //     {
    //         onPause?.Invoke();
    //         canvas.SetActive(true);
    //         OnChangePauseStatus?.Invoke(false);
    //         GameManager.instance.LockCursor(false);
    //     }
    //
    //     isPaused = !isPaused;
    // }
    //
    // public void GoToMenu()
    // {
    //     StartCoroutine(GoToMenuCO());
    // }
    //
    // IEnumerator GoToMenuCO()
    // {
    //     background.FadeIn(0.2f);
    //     Time.timeScale = 1;
    //     yield return new WaitForSeconds(0.22f);
    //     
    //     OnBackToMenu?.Invoke();
    //     // UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    // }
    //
    // //SaveHandler
    // public void SaveOptions() => options.SaveOptions();
    // public void LoadOptions() => options.LoadOptions();
    // public void UpdateRealTimeVolume() => options.UpdateRealTimeVolume();
    // public void UpdateRealTimeBrightness() => options.UpdateRealTimeBrightness();
    // public void UpdateRealTimeSaturation() => options.UpdateRealTimeSaturation();
}
