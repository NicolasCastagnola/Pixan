using System;
using UnityEngine;
using Audio;
using Sirenix.OdinInspector;

public class RainController : MonoBehaviour
{
    [SerializeField] private AudioConfigurationData _audioConfigurationData;
    [SerializeField] private Transform soundPivot;
    [SerializeField, MinMaxSlider(0.5f, 60f)] private Vector2 distance = new Vector2(0.5f , 60f);

    private void Awake()
    {
        _audioConfigurationData.Play3D(gameObject.name, new Audio.AudioManager.SoundParameters(soundPivot, distance));
    }
}
