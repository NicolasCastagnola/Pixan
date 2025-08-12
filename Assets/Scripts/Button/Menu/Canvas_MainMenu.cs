using System;
using SaveHandler;
using UnityEngine;
using System.Collections;
using Audio;
using TMPro;

public class Canvas_MainMenu : BaseMonoSingleton<Canvas_MainMenu>
{
    public event Action OnPlayButtonPressed, OnResetProgressButtonPressed, OnExitGamePressed;
    public event Action<int> OnLoadCustomLevel;

    [SerializeField] private AudioConfigurationData mainMenuTheme;

    //[SerializeField] Options options;

    [SerializeField] private TMP_Text version;

    [SerializeField] UIFader background;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            PlayerPrefs.SetInt("SavedLevel", Mathf.Clamp(PlayerPrefs.GetInt("SavedLevel",1)+1,1,3));
    }
    public void Initialize(Action continueFromSaved, Action<int> loadTestingLevel, Action exitGame, Action onResetProgressButtonPressed)
    {
        OnPlayButtonPressed = continueFromSaved;
        OnLoadCustomLevel = loadTestingLevel;
        OnExitGamePressed = exitGame;
        OnResetProgressButtonPressed = onResetProgressButtonPressed;

        version.text = $"Patch: {Application.version}";

        mainMenuTheme.Play2D();

        //StartCoroutine(options.WaitLoadOptions());
        background.FadeOut(0.5f);
    }
    public void Terminate()
    {
        OnPlayButtonPressed = null;
        OnExitGamePressed = null;
        OnLoadCustomLevel = null;

        mainMenuTheme.Stop();
    }
    public void Play() => OnPlayButtonPressed?.Invoke();
    public void ResetProgress() => OnResetProgressButtonPressed?.Invoke();
    public void LoadCustomLevel(int value)
    {
        if(PlayerPrefs.GetInt("SavedLevel", 1)>= value)/*Bypass limitation only level 1*/
            OnLoadCustomLevel?.Invoke(value);
    }
    public void Exit() => OnExitGamePressed?.Invoke();

    public void DeleteProgress() => PlayerPrefs.DeleteAll();

    #region SaveHandler
    public void SaveOptions() { }//options.SaveOptions() ;}
    public void LoadOptions() {}//options.LoadOptions();}
    public void UpdateRealTimeVolume() {}//options.UpdateRealTimeVolume();}
    public void UpdateRealTimeBrightness() {}//options.UpdateRealTimeBrightness();}
    public void UpdateRealTimeSaturation() {}//options.UpdateRealTimeSaturation();}
  #endregion
}
