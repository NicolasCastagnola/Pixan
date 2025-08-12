using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class RainSoundTrigger : MonoBehaviour
{
    [SerializeField] private bool shouldStop;
    [SerializeField] private AudioConfigurationData _audioConfigurationData;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (shouldStop)
            {
                _audioConfigurationData.Stop();
            }
            else
            {
                if (Audio.AudioManager.IsPlaying("Rain")) return;
                
                _audioConfigurationData.Play2D("Rain");
            }
        }
    }
}
