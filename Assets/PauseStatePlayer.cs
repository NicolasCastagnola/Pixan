using PlayerAnimations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseStatePlayer : IState
{
    private Player _player;

    Vector3 pos;
    Quaternion rot;

    bool isPlayingFire, isPlayingIce;

    ParticleSystem[] particleSystems;

    public PauseStatePlayer(Player player)
    {
        _player = player;
    }
    public void OnEnter()
    {
        _player.CantMove = true;
        _player.view.ChangeAnim(Animations.Idle);
        _player.Animator.speed = 0;
        pos = _player.transform.position;
        rot = _player.transform.rotation;

        if (_player.view.fireWeaponParticle.isPlaying)
        {
            isPlayingFire = true;
            _player.view.fireWeaponParticle.Pause();
        }
        else isPlayingFire = false;
        if (_player.view.iceWeaponParticle.isPlaying)
        {
            isPlayingIce = true;
            _player.view.iceWeaponParticle.Pause();
        }
        else isPlayingIce = false;

        _player.view.chargeWeaponParticle.Stop();

        particleSystems = _player.FindParticles();

        foreach (var item in particleSystems)
            item.Pause();
    }

    public void OnExit()
    {
        _player.CantMove = false;
        _player.Animator.speed = 1;

        if (isPlayingFire)
            _player.view.fireWeaponParticle.Play();
        if (isPlayingIce)
            _player.view.iceWeaponParticle.Play();

        foreach (var item in particleSystems)
        {
            if (item == null)
                continue;
            item.Play();
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnUpdate()
    {
        _player.Motor.SetPositionAndRotation(pos, rot);
    }

}
