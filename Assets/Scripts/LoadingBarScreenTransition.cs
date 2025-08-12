using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;

namespace ScreenTransition
{
    public class LoadingBarScreenTransition : MonoBehaviour, IScreenTransition
    {
        public CanvasGroup group;
        public Image fillImage;

        public float showDuration = .35f;
        public AnimationCurve showAlphaCurve;
        public float hideDuration = .25f;
        public AnimationCurve hideAlphaCurve;

        private float _targetProgress;

        private float _vel;
        private const float Smooth = .15f;

        private void Start() => group.gameObject.SetActive(false);

        public YieldInstruction Show()
        {
            group.gameObject.SetActive(true);
            group.blocksRaycasts = true;
            _targetProgress=fillImage.fillAmount=0;
            _vel = 0;
            
            IEnumerator Animation()
            {
                float t = 0;
                float startAlpha = group.alpha;
                do
                {
                    t += Time.unscaledDeltaTime / showDuration;
                    group.alpha = Mathf.Lerp(startAlpha, 1, showAlphaCurve.Evaluate(t));
                    yield return null;
                } while (t<1);
            }
            
            return StartCoroutine(Animation());
        }

        public YieldInstruction Hide()
        {
            group.blocksRaycasts = false;
            
            IEnumerator Animation()
            {
                float t = 0;
                float startAlpha = group.alpha;
                do
                {
                    t += Time.unscaledDeltaTime / hideDuration;
                    group.alpha = Mathf.Lerp(startAlpha, 0, hideAlphaCurve.Evaluate(t));
                    yield return null;
                } while (t<1);
                group.gameObject.SetActive(false);
            }
            
            return StartCoroutine(Animation());
        }
        public void SetProgress(float progress) => _targetProgress = progress;
        public void Update()
        {
            if(fillImage.fillAmount != _targetProgress)
                fillImage.fillAmount = Mathf.SmoothDamp(fillImage.fillAmount, _targetProgress, ref _vel, Smooth, float.MaxValue, Time.unscaledDeltaTime);
        }
    }
}