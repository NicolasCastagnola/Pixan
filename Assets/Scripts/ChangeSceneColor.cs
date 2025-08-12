using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using DG.Tweening;

public class ChangeSceneColor : MonoBehaviour
{
    [ColorUsage(false, true)]
    [SerializeField] Color newColor;
    //public Renderer renderMaterial;
    //public Material mat => renderMaterial.sharedMaterial;// "Dither" Material
    public bool isInitialColor;

    ColorAdjustments colorAdjustments;

    [SerializeField] private float fadeDuration = 2f;

    private void Start()
    {
        if (isInitialColor)
        {
            InstantChangeColor();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged+=(x,y) => InstantChangeColor();
        }
    }
    void FindColorAdjustments()
    {
        var volume = FindObjectOfType<Volume>();
        if (volume == null)
        {
            Debug.LogWarning("No se encontró el Volume.");
            return;
        }
        if (volume.profile.TryGet(out ColorAdjustments _colorAdjustments))
            colorAdjustments = _colorAdjustments;
        else
        {
            Debug.LogWarning("No se encontró el componente ColorAdjustments en el perfil.");
            return;
        }
    }
    public void FadeColor()
    {
        if (colorAdjustments == null)
            FindColorAdjustments();

        if (colorAdjustments != null){
            Fade();
        }

    }
    public void InstantChangeColor()
    {
        if (colorAdjustments == null)
            FindColorAdjustments();

        if (colorAdjustments != null)
            ChangeColor(newColor);
    }

    void ChangeColor(Color color)
    {
        colorAdjustments.colorFilter.value = color;
    }
    public void Fade()
    {
        Color startColor = colorAdjustments.colorFilter.value;
        DOTween.To(
            () => startColor,
            x => colorAdjustments.colorFilter.value = x,
            newColor,
            fadeDuration
        ).SetEase(Ease.InOutSine);
    }
}
