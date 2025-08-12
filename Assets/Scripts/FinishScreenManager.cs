using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class FinishScreenManager : BaseMonoSingleton<FinishScreenManager>
{
    public event Action OnLoadNext;
    
    public float fadeDuration = 1.0f;
    public TextMeshProUGUI[] TextScreens; 
    private int currentScreenIndex = 0;

    public void Initialize(Action onLoadNext)
    {
        OnLoadNext = onLoadNext;
    }
    
    public void ChangeText()
    {
        StartCoroutine(ChangeTextWithFade());
    }

    private IEnumerator ChangeTextWithFade()
    {
        if (TextScreens == null || TextScreens.Length == 0) yield break;

        StartCoroutine(FadeOut(TextScreens[currentScreenIndex], fadeDuration));
        yield return new WaitForSeconds(fadeDuration);

        currentScreenIndex++;
        if (currentScreenIndex >= TextScreens.Length)
        {
            OnTextSequenceCompleted();
            yield break;
        }
        StartCoroutine(FadeIn(TextScreens[currentScreenIndex], fadeDuration));
    }

    private IEnumerator FadeIn(TextMeshProUGUI textMeshPro, float duration)
    {
        textMeshPro.alpha = 0.0f;
        textMeshPro.gameObject.SetActive(true);

        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            textMeshPro.alpha = Mathf.Lerp(0.0f, 1.0f, t);
            yield return null;
        }

        textMeshPro.alpha = 1.0f;
    }

    private IEnumerator FadeOut(TextMeshProUGUI textMeshPro, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            textMeshPro.alpha = Mathf.Lerp(1.0f, 0.0f, t);
            yield return null;
        }

        textMeshPro.alpha = 0.0f;
        textMeshPro.gameObject.SetActive(false);
    }


    private void OnTextSequenceCompleted()
    {
       Cursor.lockState = CursorLockMode.Confined;
       
       OnLoadNext?.Invoke();
    }
}
