using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class FogWall : Interactable
{
    public Action OnTraverseFogStarted;
    public Action OnTraverseFogFinished;
    
    private bool alreadyInteracted;
    private bool _isTraversing;
    
    [SerializeField] private bool hasBoss;
    [SerializeField, ShowIf(nameof(hasBoss))] private Boss attachedBoss;
    
    [SerializeField] private Collider blockingMainCollider;
    [SerializeField] private Collider wallEntrance;       
    [SerializeField] private float traverseTime = 2f;

    [SerializeField] private Transform finalPoint;
    [SerializeField] private Transform guidingPoint;

    private void Start()
    {
        GetComponentInChildren<MeshRenderer>().enabled = false;
        
        if (hasBoss)
        {
            attachedBoss.Health.OnDead += DisableWall;
        }
    }
    private void OnDestroy()
    {
        if (hasBoss)
        {
            attachedBoss.Health.OnDead -= DisableWall;
        }
    }
    private void DisableWall(HealthComponent.HealthModificationReport healthModificationReport)
    {
        gameObject.SetActive(false);
    }
    public override void OnInteract()
    {
        if (alreadyInteracted) return;

        TraverseFog();
    }

    private void Update()
    {
        if (!_isTraversing) return;

        Vector3 finalpos = guidingPoint.position;
        finalpos.y = componentWithinReach.PlayerComponent.Motor.transform.position.y;

        componentWithinReach.PlayerComponent.Motor.SetTransientPosition(finalpos);
    }
    private void TraverseFog()
    {
        _isTraversing = true;
        
        alreadyInteracted = true;
        
        blockingMainCollider.enabled = false;
        
        OnTraverseFogStarted?.Invoke();
        
        Canvas_Playing.Instance.HideInteractDisplay();

        Vector3 finalpos = finalPoint.position;
        finalpos.y = componentWithinReach.PlayerComponent.Motor.transform.position.y;

        
        guidingPoint.DOMove(finalpos, traverseTime).SetEase(Ease.InOutSine)
            .OnStart(()=>componentWithinReach.PlayerComponent.isOnFog = true)
            .OnUpdate(() => {
                componentWithinReach.PlayerComponent.Animator.SetBool("Idle", false);
                componentWithinReach.PlayerComponent.Animator.SetBool("IsMoving", true);
            })
            .OnComplete(()=> {
                componentWithinReach.PlayerComponent.Animator.SetBool("IsMoving", false);
                componentWithinReach.PlayerComponent.Animator.SetBool("Idle", true);
                componentWithinReach.PlayerComponent.isOnFog = false;
                FinishedTraversing();
            });
    }

    private void FinishedTraversing()
    {
        wallEntrance.enabled = false;

        blockingMainCollider.enabled = true;

        _isTraversing = false;
        
        OnTraverseFogFinished?.Invoke();
    }
    public override void OnExitOnReachableRadius()
    {
    }
}
