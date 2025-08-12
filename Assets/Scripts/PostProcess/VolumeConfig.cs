using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using SaveHandler;

public class VolumeConfig : MonoBehaviour
{
    [SerializeField] bool isSaveManager;

    Volume volume;

    float saturation, brightness;

    AudioSource[] audioSources = new AudioSource[0];

    private void Start()
    {
        volume = GetComponent<Volume>();

        if (isSaveManager)
        {
            new SaveManager<OptionsObject>("Options").Load((options) =>
                StartCoroutine(UpdateValues(options)));
        }
    }
    public IEnumerator UpdateValues(OptionsObject options)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        UpdateSound(options.volume);
        UpdateSaturation(options.saturation);
        UpdateBrightness(options.brightness);
    }
    public void UpdateSound(float volume)
    {
        if (audioSources.Length <= 0)
            audioSources = FindObjectsOfType<AudioSource>();

        foreach (var item in audioSources)
            item.volume = volume;
    }
    public void UpdateSaturation(float saturation)
    {
        var constrast = new ColorAdjustments();

        if (volume.profile.TryGet(out constrast))
        {
            constrast.contrast.value = saturation.map(0, 1, -24, 10);
        }
    }
    public void UpdateBrightness(float brightness)
    {
        var constrast = new ColorAdjustments();

        if (volume.profile.TryGet(out constrast))
        {
            constrast.postExposure.value = brightness.map(0, 1, -1, 1);
        }
    }
}
