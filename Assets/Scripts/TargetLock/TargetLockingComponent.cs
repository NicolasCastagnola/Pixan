using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class TargetLockingComponent : MonoBehaviour
{
    public bool LockingActive => CurrentTarget;
    public event Action<CameraLockTarget> OnTargetLock; 
    public event Action OnTargetUnlock; 
    
    private SphereCollider _collider;
    private Player _owner;
    
    [ShowInInspector, ReadOnly] private List<CameraLockTarget> availableTargets = new List<CameraLockTarget>();
    [ShowInInspector, ReadOnly] public CameraLockTarget CurrentTarget { get; private set; }
    
    [SerializeField] private float radius = 9f;

    public void Initialize(Player owner, Action<CameraLockTarget> onTargetLock, Action onTargetUnlock)
    {
        _owner = owner;
        _collider ??= GetComponent<SphereCollider>();
        _collider.radius = radius;

        OnTargetUnlock = onTargetUnlock;
        OnTargetLock = onTargetLock;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CameraLockTarget target)) return;
            
        if (target.IsAvailable)
        {
            availableTargets.Add(target);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CameraLockTarget target))
        {
            availableTargets.Remove(target);
        }
    }

    private void Update()
    {
        if (CurrentTarget && !CurrentTarget.IsAvailable)
        {
            //UNLOCK TARGET CAUSE ITS DEAD
            Unlock(true);
        }
    }

    private CameraLockTarget GetNearest()
    {
        return availableTargets
              .OrderBy(obj => Vector3.Distance(transform.position, obj.transform.position))
              .FirstOrDefault();
    }

    public void Lock()
    {
        CurrentTarget = GetNearest();

        if (CurrentTarget == null) return;
        
        CurrentTarget.SetIndicator(true);

        OnTargetLock?.Invoke(CurrentTarget);
    }

    public void Unlock(bool enemyDead = false)
    {
        if (CurrentTarget == null) return;
        
        if(!enemyDead) CurrentTarget.SetIndicator(false);
        CurrentTarget = null;
        
        OnTargetUnlock?.Invoke();
    }
}
