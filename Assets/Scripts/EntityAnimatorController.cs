using Sirenix.OdinInspector;
using UnityEngine;

public abstract class EntityAnimatorController : MonoBehaviour
{
    private static readonly int Cast = Animator.StringToHash("Cast");
    private static readonly int Throw = Animator.StringToHash("Throw");
    private static readonly int Punch = Animator.StringToHash("Punch");
    private static readonly int Stomp = Animator.StringToHash("Stomp");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Locomotion = Animator.StringToHash("Locomotion");

    
    [ShowInInspector, ReadOnly] protected Animator _animator;
    [ShowInInspector, ReadOnly] protected Entity _owner;
    public virtual void Initialize(Entity owner)
    {
        _owner = owner;
        _animator = owner.Animator;
    }
    public void Terminate(){}
    public void TriggerPunch() => _animator.SetTrigger(Punch);
    public void TriggerThrow() => _animator.SetTrigger(Throw);
    public virtual void TriggerStomp() => _animator.SetTrigger(Stomp);
    public virtual void TriggerCast() => _animator.SetTrigger(Cast);
    public void SetLocomotion(float value) => _animator.SetFloat(Locomotion, value);
    public void TriggerDeath() => _animator.SetTrigger(Death);
    public void PlayAnimation(string animName) => _animator.Play(animName);
}