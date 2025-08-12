using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class MusicLevel : MonoBehaviour
{
    [SerializeField] private string key;
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
                if (Audio.AudioManager.IsPlaying(key)) return;
                
                _audioConfigurationData.Play2D(key);
            }
        }
    }
}
