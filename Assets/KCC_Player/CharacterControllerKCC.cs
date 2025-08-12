using Audio;
using KinematicCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public struct PlayerInputs
{
    public float MoveAxisForward;
    public float MoveAxisRight;
    public Quaternion CameraRotation;
    public bool Roll;
    public bool BlockDown;
    public bool BlockUp;
    public bool PickUp;
    public bool AttackDown;
    public bool Attack;
    public bool ChargedAttack;
    public bool OpenInventory;
    public bool CloseInventory;
    public bool FocusEnemy;
    public bool Pause; //TODO
    public bool heal;
}
/// <summary>
/// Variables posibles a utilizar para cada acci�n
/// </summary>
[Serializable]
public class ActionValues
{
    //attack
    public int actionAmmount = 0;
    public bool canChangeAttack = false;

    public bool canDoAction = true;
    public float actionSpeed;
    public bool actionConsumed = false;
    public bool isDoingAction = false;
    public bool cancelableAction = true;
    public bool actionFinished = false;
    public bool animationFinished = false;
}
public enum CharacterState
{
    PlayerIdle,
    PlayerWalk,
    PlayerRoll,
    PlayerBlock,
    PlayerHurt,
    PlayerAttack,
    PlayerStun,
    PlayerWalkBlock,
    PlayerDeath,
}

public enum OrientationMethod
{
    TowardsCamera,
    TowardsMovement,
}

public enum BonusOrientationMethod
{
    None,
    TowardsGravity,
    TowardsGroundSlopeAndGravity,
}

public class CharacterControllerKCC : MonoBehaviour, ICharacterController
{
    Player _player;
    public ActionValues actionValues;
    [Header("Stable Movement")]
    public float MaxStableMoveSpeed = 10f;
    public float StableMovementSharpness = 15f;
    public float OrientationSharpness = 10f;
    public OrientationMethod orientationMethod = OrientationMethod.TowardsCamera;

    [Header("Air Movement")]
    public float MaxAirMoveSpeed = 10f;
    public float AirAccelerationSpeed = 5f;
    public float Drag = 0.1f;

    public Vector3 Gravity = new Vector3(0, -30f, 0);

    [Header("Roll")]
    public ActionValues rollAction;

    [Header("Attack")]
    public ActionValues attackAction;

    [Header("Charged Attack")]
    public ActionValues charhedAttackAction;

    [Header("Block")]
    public ActionValues blockAction;

    [Header("Hurt")]
    public ActionValues hurtAction;

    [Header("Stunned")]
    public ActionValues stunnedAction;

    [Header("Dead")]
    public ActionValues deadAction;

    [Header("OpenInventory")]
    public ActionValues openInventoryAction;

    [Header("Heal")]
    public ActionValues healAction;


    private Vector3 _internalVelocityAdd = Vector3.zero;

    public KinematicCharacterMotor motor;
    public Transform meshRoot;
    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;
    public bool orientTowardsGravity = false;
    public CharacterState CurrentCharacterState { get; private set; }

    //OnAnimationFinished es una funci�n que se ejecuta al llamar a la funci�n AnimationFinished (esta se deber�a llamar al terminar la animaci�n)
    public Action OnAnimationFinished;
    public Action OnActionFinished;
    [SerializeField] CharacterAnimationEventHandler _characterAnimationEventHandler;
    public void AnimationFinished()
    {
        OnAnimationFinished?.Invoke();
    }

    public void ActionFinished()
    {
        OnActionFinished?.Invoke();
    }

    private void Start()
    {
        StartCoroutine(StopBug());
        //animator = GetComponentInChildren<Animator>();
        // Handle initial state
        //TransitionToState(CharacterState.PlayerIdle);
        // Assign to motor
        motor.CharacterController = this;
        _player = GetComponentInChildren<Player>();
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
        {
            Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
            currentRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        }

        if (orientTowardsGravity)
        {
            currentRotation = Quaternion.FromToRotation(currentRotation * Vector3.up, -Gravity) * currentRotation;
        }
        currentRotation.x = 0f;
        currentRotation.z = 0f;
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = _velocity;
        #region old
        /*
        switch (CurrentCharacterState)
        {
            case CharacterState.PlayerIdle:
            case CharacterState.PlayerHurt:
            case CharacterState.PlayerStun:
            case CharacterState.PlayerAttack:
            case CharacterState.PlayerBlock:
            case CharacterState.PlayerDeath:
                {
                    HaltVelocity(deltaTime);
                    break;
                }
            case CharacterState.PlayerRoll:
                {
                    //Roll();
                    break;
                }
            case CharacterState.PlayerWalk:
                {
                    //Walk(deltaTime);
                    break;
                }
            case CharacterState.PlayerWalkBlock:
                {
                    Vector3 targetMovementVelocity = Vector3.zero;
                    if (motor.GroundingStatus.IsStableOnGround)
                    {
                        // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
                        currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                        // Calculate target velocity
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, motor.CharacterUp);
                        Vector3 reorientedInput = Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                        targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                        // Smooth movement Velocity
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity * 0.5f, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
                    }
                    else
                    {
                        // Add move input
                        if (_moveInputVector.sqrMagnitude > 0f)
                        {
                            targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed * 0.5f;

                            // Prevent climbing on un-stable slopes with air movement
                            if (motor.GroundingStatus.FoundAnyGround)
                            {
                                Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                                targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                            }

                            Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                            currentVelocity += AirAccelerationSpeed * deltaTime * velocityDiff;
                        }

                        // Gravity
                        currentVelocity += Gravity * deltaTime;

                        // Drag
                        currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                    }
                    break;
                }
        }

        // Take into account additive velocity
        if (_internalVelocityAdd.sqrMagnitude > 0f)
        {
            currentVelocity += _internalVelocityAdd;
            _internalVelocityAdd = Vector3.zero;
        }
        */
        #endregion
    }

    private Vector3 _velocity;

    public void HaltVelocity(float deltaTime) //TODO: Volar el deltaTime si siempre se va a usar el Time.delta
    {
        if (!motor.GroundingStatus.FoundAnyGround || !motor.GroundingStatus.IsStableOnGround)
        {
            _velocity += Gravity * deltaTime;
            _moveInputVector = Vector3.zero;
            GameManager.ShowLog("Falling");
            // IsFalling = true;
            //TODO: Hacer un falling timer
            return;
        }
        
        _velocity = Vector3.zero;
        return;
        if (_velocity.magnitude < 0.01f)
        {
            _velocity = Vector3.zero;
        }
        else
        {
            Vector3 targetMovementVelocity = Vector3.zero;
            // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
            _velocity = motor.GetDirectionTangentToSurface(_velocity, motor.GroundingStatus.GroundNormal) * _velocity.magnitude;


            // Smooth movement Velocity
            _velocity = Vector3.Lerp(_velocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
        }
    }

    public void Roll(float rollSpeed)
    {
        Vector3 rollVelocity = transform.forward * rollSpeed * Time.deltaTime;

        // Gravity
        rollVelocity.y = _velocity.y+ Gravity.y * Time.deltaTime;

       _velocity = rollVelocity;

        // Drag
        _velocity *= (1f / (1f + (Drag * Time.deltaTime)));
    }

    public void Walk(PlayerInputs inputs, float deltaTime, bool cameraLocked = false, bool cantRotate = false)
    {
        if (!motor.GroundingStatus.FoundAnyGround || !motor.GroundingStatus.IsStableOnGround)
        {
            _velocity += Gravity * deltaTime;
            _moveInputVector = Vector3.zero;
            return;
        }

        SetControllerMoveInputs(inputs, cameraLocked);

        Vector3 targetMovementVelocity = Vector3.zero;

        if (motor.GroundingStatus.IsStableOnGround)
        {
            _velocity = motor.GetDirectionTangentToSurface(_velocity, motor.GroundingStatus.GroundNormal) * _velocity.magnitude;

            Vector3 inputRight = Vector3.Cross(_moveInputVector, motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
            targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

            _velocity = Vector3.Lerp(_velocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
        }
        else
        {
            if (_moveInputVector.sqrMagnitude > 0f)
            {
                targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                if (motor.GroundingStatus.FoundAnyGround)
                {
                    Vector3 perpendicularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpendicularObstructionNormal);
                }

                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - _velocity, Gravity);
                _velocity += AirAccelerationSpeed * deltaTime * velocityDiff;
            }

            _velocity += Gravity * deltaTime;
            _velocity *= (1f / (1f + (Drag * deltaTime)));
        }
    }

    public void Block()
    {
        //GameManager.ShowLog("Bloqueanding anding");
    }
    public void Attack()
    {
        //GameManager.ShowLog("Atacanding anding");
    }
    public void SetControllerMoveInputs(PlayerInputs inputs, bool cameraLocked)
    {
        var cameraControl = CameraControl(inputs);
        var rotation = cameraControl.Item1;
        var direction = cameraControl.Item2;
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

        if (cameraLocked)
        {
            _moveInputVector = inputs.CameraRotation * moveInputVector;
            _lookInputVector = inputs.CameraRotation * Vector3.forward;
        }
        else
        {
            _moveInputVector = rotation * moveInputVector;
            _lookInputVector = direction;
        }

        switch (orientationMethod)
        {
            case OrientationMethod.TowardsCamera:
                _lookInputVector = direction;
                break;
            case OrientationMethod.TowardsMovement:
                _lookInputVector = _moveInputVector.normalized;
                break;
        }
    }

    #region Unused
    public void AfterCharacterUpdate(float deltaTime)
    {

    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        // This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        // This is called when the motor's ground probing detects a ground hit
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        // This is called when the motor's movement logic detects a hit
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        // This is called after every hit detected in the motor, to give you a chance to modify the HitStabilityReport any way you want
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        // This is called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
    }
    #endregion


    public Tuple<Quaternion, Vector3> CameraControl(PlayerInputs inputs)
    {
        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, motor.CharacterUp).normalized;
        
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, motor.CharacterUp).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Vector3.up);

        return Tuple.Create(cameraPlanarRotation, cameraPlanarDirection);
    }


    #region OLD FSM
    public void TransitionToState(CharacterState newState)
    {
        CharacterState tmpInitialState = CurrentCharacterState;
        OnStateExit(tmpInitialState, newState);
        CurrentCharacterState = newState;
        OnStateEnter(newState, tmpInitialState);
    }
    public void OnStateEnter(CharacterState state, CharacterState fromState)
    {
        switch (state)
        {
            case CharacterState.PlayerIdle:
                {
                    //SetAnim("Idle");
                    break;
                }
            case CharacterState.PlayerBlock:
                {
                    if (!blockAction.isDoingAction)
                    {
                        //SetAnim("Block");
                        blockAction.isDoingAction = true;
                        blockAction.animationFinished = false;
                        _characterAnimationEventHandler.EnableBlockingColission();
                    }
                    break;
                }
            case CharacterState.PlayerWalkBlock:
                {
                    if (!blockAction.isDoingAction)
                    {
                        //SetAnim("WalkBlock");
                        blockAction.isDoingAction = true;
                        blockAction.animationFinished = false;
                        _characterAnimationEventHandler.EnableBlockingColission();
                    }
                    break;
                }
            case CharacterState.PlayerAttack:
                {
                    //TODO: Ataque player
                    if (!attackAction.isDoingAction)
                    {

                        attackAction.canChangeAttack = false;

                        // SetAnim("Attack" + attackAction.actionAmmount);
                        attackAction.actionAmmount = attackAction.actionAmmount++;

                        attackAction.actionConsumed = true;

                        attackAction.isDoingAction = true;
                        attackAction.actionFinished = false;
                        attackAction.animationFinished = false;

                        OnActionFinished = () =>
                        {
                            attackAction.actionFinished = true;
                            attackAction.actionConsumed = false;
                            attackAction.actionAmmount++;
                        };

                        OnAnimationFinished = () =>
                        {
                            attackAction.actionAmmount = 0;
                            if (attackAction.isDoingAction)
                                TransitionToState(CharacterState.PlayerIdle);
                        };

                        //_player.soundAttack.Play2D();
                    }
                    break;
                }
            case CharacterState.PlayerRoll:
                {
                    if (!rollAction.isDoingAction)
                    {
                        //SetAnim("Roll");
                        rollAction.actionConsumed = true;
                        rollAction.isDoingAction = true;
                        rollAction.actionFinished = false;
                        rollAction.animationFinished = false;

                        _player.DisableDamage();

                        OnActionFinished = () =>
                        {
                            rollAction.actionFinished = true;
                        };
                        OnAnimationFinished = () =>
                        {
                            if (rollAction.isDoingAction)
                                TransitionToState(CharacterState.PlayerIdle);
                            _player.EnableDamage();
                        };
                    }
                    break;
                }
            case CharacterState.PlayerStun:
                {
                    //SetAnim("Stun");
                    _player.DisableDamage();
                    OnAnimationFinished = () =>
                    {
                        TransitionToState(CharacterState.PlayerIdle);
                        _player.EnableDamage();
                    };
                    break;
                }
        }
    }
    public void OnStateExit(CharacterState state, CharacterState toState)
    {
        switch (state)
        {
            case CharacterState.PlayerIdle:
                {
                    //motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                    //meshRoot.localScale = new Vector3(1f, 1f, 1f);
                    break;
                }
            case CharacterState.PlayerBlock:
            case CharacterState.PlayerWalkBlock:
                {
                    if (blockAction.isDoingAction)
                    {
                        blockAction.isDoingAction = false;
                        blockAction.animationFinished = true;
                        _characterAnimationEventHandler.DisableBlockingColission();
                    }
                    break;
                }
            case CharacterState.PlayerAttack:
                {
                    if (attackAction.isDoingAction)
                    {
                        attackAction.actionConsumed = false;
                        attackAction.isDoingAction = false;
                        attackAction.actionFinished = true;
                        attackAction.animationFinished = true;
                    }
                    break;
                }
            case CharacterState.PlayerRoll:
                {
                    if (rollAction.isDoingAction)
                    {
                        rollAction.actionConsumed = false;
                        rollAction.isDoingAction = false;
                        rollAction.actionFinished = true;
                        rollAction.animationFinished = true;

                        _player.EnableDamage();
                    }
                    break;
                }
            case CharacterState.PlayerHurt:
            case CharacterState.PlayerStun:
            case CharacterState.PlayerDeath:
                break;
        }
    }
    IEnumerator WaitAndDoAction(float seconds, Action act)
    {
        yield return new WaitForSeconds(seconds);
        act();
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        // This is called before the motor does anything
        switch (CurrentCharacterState)
        {
            case CharacterState.PlayerBlock:
                {
                    // Update times
                    //_timeSinceStartedCharge += deltaTime;
                    break;
                }

            case CharacterState.PlayerIdle:
                break;
            case CharacterState.PlayerWalk:
                break;
            case CharacterState.PlayerRoll:
                break;
            case CharacterState.PlayerHurt:
                break;
            case CharacterState.PlayerAttack:
                break;
            case CharacterState.PlayerStun:
                break;
            case CharacterState.PlayerDeath:
                break;
            case CharacterState.PlayerWalkBlock:
                break;
        }
    }

    public void SetInputs(ref PlayerInputs inputs)
    {
        if (CurrentCharacterState == CharacterState.PlayerDeath) return;
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);


        switch (CurrentCharacterState)
        {
            case CharacterState.PlayerIdle:
                {
                    if (moveInputVector != Vector3.zero)
                    {
                        TransitionToState(CharacterState.PlayerWalk);
                    }
                    // Block input
                    if (inputs.BlockDown)
                    {
                        TransitionToState(CharacterState.PlayerBlock);
                    }
                    // Attack input
                    if (inputs.Attack && !attackAction.actionConsumed)
                    {
                        TransitionToState(CharacterState.PlayerAttack);
                    }
                    // Roll input
                    if (inputs.Roll)
                    {
                        TransitionToState(CharacterState.PlayerRoll);
                    }
                    break;
                }
            case CharacterState.PlayerBlock:
                {

                    // Block input
                    if (inputs.BlockUp)
                    {
                        TransitionToState(CharacterState.PlayerIdle);
                    }
                    if (moveInputVector != Vector3.zero)
                    {
                        TransitionToState(CharacterState.PlayerWalkBlock);
                    }
                    if (inputs.Attack && !attackAction.actionConsumed)
                    {
                        TransitionToState(CharacterState.PlayerAttack);
                    }
                    if (inputs.Roll)
                    {
                        TransitionToState(CharacterState.PlayerRoll);
                    }
                    break;
                }
            case CharacterState.PlayerAttack:
                {
                    if (attackAction.actionFinished)
                    {
                        if (moveInputVector != Vector3.zero)
                        {
                            TransitionToState(CharacterState.PlayerWalk);
                        }
                        // Block input
                        if (inputs.BlockDown)
                        {
                            TransitionToState(CharacterState.PlayerBlock);
                        }
                        // Attack input
                        if (inputs.Attack && !attackAction.actionConsumed && attackAction.actionAmmount < _player.maxActionAmount && attackAction.canChangeAttack)
                        {
                            StopAllCoroutines();
                            TransitionToState(CharacterState.PlayerAttack);
                        }
                        // Roll input
                        if (inputs.Roll)
                        {
                            TransitionToState(CharacterState.PlayerRoll);
                        }
                    }
                    else
                    {
                        if (inputs.Attack && attackAction.actionAmmount < _player.maxActionAmount)
                        {
                            StopAllCoroutines();
                            StartCoroutine(AttackWait());
                        }
                    }
                    break;
                }
            case CharacterState.PlayerRoll:
                {
                    if (rollAction.actionFinished)
                    {
                        /*if (moveInputVector != Vector3.zero)
                        {
                            TransitionToState(CharacterState.PlayerWalk);
                        }*/
                        // Block input
                        if (inputs.BlockDown)
                        {
                            TransitionToState(CharacterState.PlayerBlock);
                        }
                        // Attack input
                        if (inputs.Attack && !attackAction.actionConsumed)
                        {
                            TransitionToState(CharacterState.PlayerAttack);
                        }
                    }
                    break;
                }
            case CharacterState.PlayerWalk:
                {
                    if (moveInputVector == Vector3.zero)
                    {
                        TransitionToState(CharacterState.PlayerIdle);
                    }
                    if (inputs.BlockDown)
                    {
                        TransitionToState(CharacterState.PlayerWalkBlock);
                    }
                    if (inputs.Roll)
                    {
                        TransitionToState(CharacterState.PlayerRoll);
                    }
                    if (inputs.Attack && !attackAction.actionConsumed)
                    {
                        TransitionToState(CharacterState.PlayerAttack);
                    }
                    // Move and look inputs
                    /* _moveInputVector = cameraPlanarRotation * moveInputVector;
                     _lookInputVector = cameraPlanarDirection;

                     switch (orientationMethod)
                     {
                         case OrientationMethod.TowardsCamera:
                             _lookInputVector = cameraPlanarDirection;
                             break;
                         case OrientationMethod.TowardsMovement:
                             _lookInputVector = _moveInputVector.normalized;
                             break;
                     }*/

                    break;
                }
            case CharacterState.PlayerWalkBlock:
                {
                    if (inputs.BlockUp)
                    {
                        TransitionToState(CharacterState.PlayerWalk);
                    }
                    if (moveInputVector == Vector3.zero)
                    {
                        TransitionToState(CharacterState.PlayerBlock);
                    }
                    if (inputs.Roll)
                    {
                        TransitionToState(CharacterState.PlayerRoll);
                    }
                    if (inputs.Attack && !attackAction.actionConsumed)
                    {
                        TransitionToState(CharacterState.PlayerAttack);
                    }
                    // _moveInputVector = cameraPlanarRotation * moveInputVector;
                    // _lookInputVector = cameraPlanarDirection;
                    switch (orientationMethod)
                    {
                        case OrientationMethod.TowardsCamera:
                            // _lookInputVector = cameraPlanarDirection;
                            break;
                        case OrientationMethod.TowardsMovement:
                            _lookInputVector = _moveInputVector.normalized;
                            break;
                    }
                    break;
                }
            /*case CharacterState.PlayerHurt:
                {

                    break;
                }
            case CharacterState.PlayerStun:
                {

                    break;
                }
            case CharacterState.PlayerDeath:
                {

                    break;
                }*/
            default:
                break;
        }
    }
    public IEnumerator StopBug()
    {
        CharacterState currentState = CurrentCharacterState;
        while (true)
        {
            currentState = CurrentCharacterState;
            yield return new WaitForSeconds(2);
            if (currentState == CurrentCharacterState)
                TransitionToState(CharacterState.PlayerIdle);
        }
    }
    public IEnumerator AttackWait()
    {
        while (!attackAction.actionFinished)
            yield return new WaitForEndOfFrame();
        TransitionToState(CharacterState.PlayerAttack);
    }

    public void AddVelocity(Vector3 velocity)
    {
        _internalVelocityAdd += velocity;
    }

    public void InterruptAction(CharacterState to)
    {
        if (CurrentCharacterState == to) return;
        TransitionToState(to);
    }
    public void CanAttack() { attackAction.canChangeAttack = true; attackAction.actionConsumed = false; }
    public void CantAttack() => attackAction.canChangeAttack = false;

    #endregion
}
