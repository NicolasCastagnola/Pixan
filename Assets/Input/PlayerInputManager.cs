using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerInputManager : BaseMonoSingleton<PlayerInputManager>
{
    [ShowInInspector, ReadOnly, TabGroup("Locomotion")] public Vector2 MoveInput { get; private set; }
    
    private PlayerControls _playerControls;

    protected override void Awake()
    {
        _playerControls = new PlayerControls();
        _playerControls.PlayerMovement.Movement.performed += i => MoveInput = i.ReadValue<Vector2>();
        _playerControls.Enable();
    }
}
