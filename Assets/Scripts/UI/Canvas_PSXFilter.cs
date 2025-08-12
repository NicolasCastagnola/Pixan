using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Canvas_PSXFilter : BaseMonoSingleton<Canvas_PSXFilter>
{
    public void Initialize()
    {
    }
    public void Terminate()
    {
    }
    
    [field:SerializeField] public RawImage RawImageFilter { get; private set; }
    [field:SerializeField] public Volume Volume { get; private set; }
}
