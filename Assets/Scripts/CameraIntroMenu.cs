using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CameraIntroMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] titles;
    [SerializeField] RectTransform background;
    [SerializeField] RectTransform buttons;

    void Start()
    {
        transform.DORotate(new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z), 20f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

        DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> manteca = null;

        foreach (var item in titles)
        {
            var jaj = item.DOFade(1, 2).SetDelay(2f);

            if (manteca == null) manteca = jaj;
        }

        manteca.OnComplete(() => background.DOSizeDelta(new Vector2(background.sizeDelta.x + 2000, background.sizeDelta.y), 1.25f).SetEase(Ease.Linear)
        .OnComplete(() => buttons.DOAnchorPos(new Vector2(150, buttons.anchoredPosition.y), 0.5f).SetEase(Ease.Linear).SetDelay(0.5f)))
        ;
    }
}
