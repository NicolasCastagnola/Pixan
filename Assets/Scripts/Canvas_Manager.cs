using UnityEngine;

public class Canvas_Manager : BaseMonoSingleton<Canvas_Manager>
{
    [field:SerializeField] public Canvas_Playing CanvasPlaying { get; private set; }
    [field:SerializeField] public Canvas_Inventory CanvasInventory { get; private set; }
    [field:SerializeField] public Canvas_Pause CanvasPause { get; private set; }
    [field:SerializeField] public Canvas_Debug CanvasDebug { get; private set; }
    
    public void Initialize(){}
    public void Terminate(){}
}
