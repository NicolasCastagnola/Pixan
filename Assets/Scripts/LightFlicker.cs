using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LightFlicker : MonoBehaviour
{
    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    public new Light light;
    [Tooltip("Minimum random light intensity")]
    public float minIntensity = 0f;
    [Tooltip("Maximum random light intensity")]
    public float maxIntensity = 1f;
    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    public int smoothing = 5;
    
    private Queue<float> _smoothQueue;
    private float _lastSum = 0;
    
    public void Reset() 
    {
        _smoothQueue.Clear();
        _lastSum = 0;
    }
    private void Awake() 
    {
        _smoothQueue = new Queue<float>(smoothing);

        if (light == null) light = GetComponent<Light>();
    }
    public void Update() 
    {
        while (_smoothQueue.Count >= smoothing) 
        {
            _lastSum -= _smoothQueue.Dequeue();
        }

        var newVal = Random.Range(minIntensity, maxIntensity);
        _smoothQueue.Enqueue(newVal);
        _lastSum += newVal;
        
        light.intensity = _lastSum / _smoothQueue.Count;
    }
}
