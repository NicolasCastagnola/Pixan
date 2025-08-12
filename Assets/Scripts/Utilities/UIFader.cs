using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

    /// <summary>
    /// Script fades a UI Canvas Group with given methods. Good for gradually showing/hiding a UI.
    /// </summary>
    public class UIFader : MonoBehaviour
    {
        [SerializeField] private bool disableOnAlpha0 = true;
        [SerializeField] private bool enableOnPlay;
        private CanvasGroup canvasGroup;

        private bool fading = false;
        private bool visible = false;
        public bool IsFading => fading;
        public bool Visible => visible;

        private Tween curTween = null;
        [SerializeField] TextMeshProUGUI _text;
        float _fadeInTime = 1f;
        float _displayTime = 1f;
        float _fadeOutTime = 1f;


        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = enableOnPlay?1:0;
        }

        public void FadeIn(float time)
        {
            if (curTween.IsActive())
                curTween.Kill();

            if (disableOnAlpha0)
            {
                gameObject.SetActive(true);
            }
            visible = true;
            fading = true;
            curTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, time).OnComplete(() =>
            {
                fading = false;
            });
        }

        public void FadeOut(float time)
        {
            if(curTween.IsActive())
                curTween.Kill();

            visible = false;
            fading = true;
            curTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, time).OnComplete(() =>
            {
                fading = false;
                if (disableOnAlpha0)
                {
                    gameObject.SetActive(false);
                }
            });
        }

        public void ShowText(string text = "", float fadeInTime = 1f, float displayTime = 1f, float fadeOutTime = 1f){
            if (text != "") { SetText(text); }
            FadeIn(fadeInTime);
            GameManager.instance.WaitForXSeconds(displayTime, () => {
                FadeOut(fadeOutTime);
            });
        GameManager.ShowLog("Txt: " + text + "fit: " + fadeInTime + " - dt: " + displayTime + " - fot: " + fadeOutTime); ;

        }
        public void ShowText(string text = "") { 
            if (text != "") { SetText(text); }
            FadeIn(_fadeInTime);
            GameManager.instance.WaitForXSeconds(_displayTime, () => {
                FadeOut(_fadeOutTime);
            });
        }

        public void SetText(string text = ""){
            _text.SetText(text);
        }

        public void SetFadeInTime(float time = 1f){
            _fadeInTime = time;
        }
        public void SetDisplayTime(float time = 1f){
            _displayTime = time;
        }
        public void SetFadeOutTime(float time = 1f){
            _fadeOutTime = time;
        }
    }