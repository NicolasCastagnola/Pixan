using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace SaveHandler
{
    [System.Serializable]
    public class Options
    {
        [SerializeField] Scrollbar volumeScrollbar, brightnessScrollbar, saturationScrollbar;
        [SerializeField] VolumeConfig volume;
        [SerializeField] AudioMixer mixer;

        SaveManager<OptionsObject> saveManager;

        public void SetVolume(VolumeConfig volumeConfig) => volume = volumeConfig;
        
        public IEnumerator WaitLoadOptions()//call first
        {
            saveManager = new SaveManager<OptionsObject>("Options");//Name of the Json;
            yield return new WaitForSecondsRealtime(0.1f);
            LoadOptions();
        }
        public void LoadOptions() => saveManager.Load(options =>
        {//Load(Gives data in Option Struct when called)
            volumeScrollbar.value     = options.volume;
            brightnessScrollbar.value = options.brightness;
            saturationScrollbar.value = options.saturation;

            //UpdateRealTimeVolume();
            UpdateRealTimeBrightness();
            UpdateRealTimeSaturation();
        });

        public void SaveOptions() => saveManager.Save(() =>
             new OptionsObject().Initialize(volumeScrollbar.value, brightnessScrollbar.value, saturationScrollbar.value));

        public void UpdateRealTimeVolume()
        {
            mixer.SetFloat("MASTER_VOLUME", Mathf.Log10(Mathf.Clamp(volumeScrollbar.value,0.01f,1)) * 20);
        }
        public void UpdateRealTimeBrightness()
        {
            volume.UpdateBrightness(brightnessScrollbar.value);
        }
        public void UpdateRealTimeSaturation()
        {
            volume.UpdateSaturation(saturationScrollbar.value);
        }
    }
}