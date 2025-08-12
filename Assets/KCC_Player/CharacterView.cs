using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAnimations;
using System;
using Audio;
using PlayerSounds;

public class CharacterView : MonoBehaviour
{
    private Animator _animator;
    private CharacterAnimationEventHandler _handler;
    public CharacterAnimationEventHandler Handler { get => _handler; }
    private Dictionary<Animations, AnimationAction> _animationKeys;
    private Animations _currentAnimation;
    private Dictionary<Sounds, AudioConfigurationData> _soundsKeys;

    // TODO: Manejar: Particulas, sonidos, etc.

    public ParticleSystem iceWeaponParticle;
    public ParticleSystem fireWeaponParticle;
    public ParticleSystem chargeWeaponParticle;
    public AudioConfigurationData chargeWeaponSound;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _handler = GetComponentInChildren<CharacterAnimationEventHandler>();
        _handler.DisableParticleSystem();

        _animationKeys = new()
        {
            { Animations.Idle, new AnimationAction("Idle") },
            { Animations.Walk, new AnimationAction("IsMoving") },
            { Animations.Roll, new AnimationAction("Roll") },
            { Animations.Attack0, new AnimationAction("Attack0") },
            { Animations.Attack1, new AnimationAction("Attack1") },
            { Animations.ChargedAttack, new AnimationAction("ChargedAttack") },
            { Animations.Block, new AnimationAction("Block") },
            { Animations.Hurt, new AnimationAction("Hurt") },
            { Animations.Death, new AnimationAction("Death") },
            { Animations.Sit, new AnimationAction("Sit") },
            { Animations.Heal, new AnimationAction("Heal") }
        };

        _soundsKeys = new()
        {
            { Sounds.soundAttack, soundAttack },
            { Sounds.soundBlock, soundBlock },
            { Sounds.soundDeath, soundDeath },
            { Sounds.soundHeal, soundHeal },
            { Sounds.soundParry, soundParry },
            { Sounds.soundGolpe, soundGolpe },
            { Sounds.soundWalk, soundWalk },
            { Sounds.soundChargeAttack, soundChargeAttack }
        };

        InitializeAllStateBehaviours();
    }

    private void InitializeAllStateBehaviours()
    {
        var animStates = _animator.GetBehaviours<BaseAnimationState>();
        foreach (var behaviour in animStates)
        {
            behaviour.Initialize(this);
        }
    }

    void SetAnim(string key) => _animator.SetTrigger(key);

    public void SetBool(string key, bool value = true) => _animator.SetBool(key, value);

    public void ChangeAnim(Animations animation, bool doNotStopPreviousAnim = false)
    {
        if (!_animationKeys.ContainsKey(animation)) return;

        // Ejecuto la función OnAnimationFinished de la current animation si es que tiene
        _animationKeys[_currentAnimation].OnAnimationFinished?.Invoke();

        // Setteo en false el bool de la animación anterior
        if (!doNotStopPreviousAnim) SetBool(_animationKeys[_currentAnimation].name, false);

        // Setteo en true el bool de la nueva animación
        SetBool(_animationKeys[animation].name);
        _currentAnimation = animation;

        // Ejecuto la función OnAnimationStart de la nueva animation si es que tiene
        _animationKeys[animation].OnAnimationStart?.Invoke();
    }

    public bool FinishedAction(Animations key)
    {
        // Cambiar por CharacterAnimationEventHandler
        return _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != _animationKeys[key].name;
    }

    public void ResetAllBools()
    {
        foreach (var item in _animator.parameters)
        {
            _animator.SetBool(item.name, false);
        }
    }

    // Forzoso
    void StopAllTriggers()
    {
        foreach (var item in _animator.parameters)
        {
            _animator.ResetTrigger(item.name);
        }
    }

    public bool AnimIsPlaying(Animations anim)
    {
        return _currentAnimation == anim;
    }

    public Animations GetCurrentAnimation()
    {
        return _currentAnimation;
    }

    #region Sound

    public AudioConfigurationData soundAttack, soundBlock, soundDeath, soundHeal, soundParry, soundGolpe, soundWalk, soundChargeAttack;


    public void Play2DAudio(Sounds sound)
    {
        if (!_soundsKeys.ContainsKey(sound)) return;
        _soundsKeys[sound].Play2D();
    }

    public void StopAudio(Sounds sound)
    {
        if (!_soundsKeys.ContainsKey(sound)) return;
        _soundsKeys[sound].Stop();
    }

    #endregion
}

namespace PlayerAnimations
{
    public enum Animations
    {
        Idle,
        Walk,
        Attack0,
        Attack1,
        Attack2,
        ChargedAttack,
        Roll,
        Block,
        Hurt,
        Death,
        Sit,
        Heal
    }

    public struct AnimationAction
    {
        public string name;
        public Action OnAnimationStart;
        public Action OnAnimationFinished;
        public Action OnAnimationCanceled;

        public AnimationAction(string name, Action onAnimationStart, Action onAnimationFinished, Action onAnimationCanceled)
        {
            this.name = name ?? "";
            OnAnimationStart = onAnimationStart ?? (() => { });
            OnAnimationFinished = onAnimationFinished ?? (() => { });
            OnAnimationCanceled = onAnimationCanceled ?? (() => { });
        }

        public AnimationAction(string name)
        {
            this.name = name ?? "";
            OnAnimationStart = () => { };
            OnAnimationFinished = () => { };
            OnAnimationCanceled = () => { };
        }
    }
}

namespace PlayerSounds
{
    public enum Sounds
    {
        soundAttack,
        soundBlock,
        soundDeath,
        soundHeal,
        soundParry,
        soundGolpe,
        soundWalk,
        soundChargeAttack
    }
}