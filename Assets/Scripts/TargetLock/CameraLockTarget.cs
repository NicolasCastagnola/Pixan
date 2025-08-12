using UnityEngine;

public class CameraLockTarget : MonoBehaviour
{
    [SerializeField] private Entity Owner;
    [SerializeField] private GameObject Indicator;
    public Transform LockPivot => Owner.CenterOfMassPivot;
    public bool IsAvailable => Owner.Health.IsAlive;
    private void Awake() => Owner ??= GetComponentInParent<Entity>();
    public void SetIndicator(bool b) => Indicator.SetActive(b);
}
