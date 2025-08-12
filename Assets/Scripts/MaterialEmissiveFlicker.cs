using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialEmissiveFlicker : MonoBehaviour
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private Renderer rend;
    
    public bool shouldFlicker;
    public float flickerSpeed = 0.1f;
    public Color minEmissionColor = Color.black;
    public Color maxEmissionColor = Color.white;
    private void Start()
    {
        rend = GetComponent<Renderer>();
        StartCoroutine(FlickerEmission());
    }
    private IEnumerator FlickerEmission()
    {
        while (shouldFlicker)
        {
            var t = 0f;
            
            while (t < 1f)
            {
                t += Time.deltaTime / flickerSpeed;
                rend.material.SetColor(EmissionColor, Color.Lerp(minEmissionColor, maxEmissionColor, t));
                yield return null;
            }

            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}
