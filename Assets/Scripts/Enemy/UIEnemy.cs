using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIEnemy : MonoBehaviour
{
    private Transform _targetToFace;
    private Entity _owner;
    
    [SerializeField] private Image enemyLife;
    [SerializeField] private float barFillDuration = 0.1f;
    public void Initialize(Transform targetToFace, Entity owner)
    {
        _owner = owner;
        _targetToFace = targetToFace;

        _owner.Health.OnUpdate += UpdateLife;
    }
    public void Terminate()
    {
        _owner.Health.OnUpdate -= UpdateLife;
        
        Destroy(gameObject);
    }
    private void UpdateLife(HealthComponent.HealthModificationReport healthModificationReport) => enemyLife.DOFillAmount(_owner.Health.CurrentPercentage, barFillDuration);
    private void Update()
    {
        if (_targetToFace == null) return;
        
        transform.LookAt(_targetToFace.transform);
    }
}
