using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterCameraKCC : MonoBehaviour
{
    [Header("Framing")]
    public Camera Camera;
    public Vector2 FollowPointFraming = new Vector2(0f, 0f);
    public float FollowingSharpness = 10000f;

    [Header("Distance")]
    public float DefaultDistance = 6f;
    public float MinDistance = 0f;
    public float MaxDistance = 10f;
    public float DistanceMovementSpeed = 5f;
    public float DistanceMovementSharpness = 10f;

    [Header("Rotation")]
    public bool InvertX = false;
    public bool InvertY = false;
    [Range(-90f, 90f)]
    public float DefaultVerticalAngle = 20f;
    [Range(-90f, 90f)]
    public float MinVerticalAngle = -90f;
    [Range(-90f, 90f)]
    public float MaxVerticalAngle = 90f;
    public float RotationSpeed = 1f;
    public float RotationSharpness = 10000f;
    public bool RotateWithPhysicsMover = false;

    [Header("Obstruction")]
    public float ObstructionCheckRadius = 0.2f;
    public LayerMask ObstructionLayers = -1;
    public float ObstructionSharpness = 10000f;
    public List<Collider> IgnoredColliders = new List<Collider>();

    [Header("Shake")]
    public bool isShaking = false;
    public Vector3 shakeOffset = Vector3.zero;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.25f;

    public Transform Transform { get; private set; }
    public Transform FollowTransform { get; private set; }

    public Vector3 PlanarDirection { get; set; }
    public float TargetDistance { get; set; }

    private bool _distanceIsObstructed;
    private float _currentDistance;
    private float _targetVerticalAngle;
    private RaycastHit _obstructionHit;
    private int _obstructionCount;
    private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];
    private float _obstructionTime;
    private Vector3 _currentFollowPosition;

    private const int MaxObstructions = 32;

    void OnValidate()
    {
        DefaultDistance = Mathf.Clamp(DefaultDistance, MinDistance, MaxDistance);
        DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
    }

    private void Awake()
    {
        Transform = this.transform;

        _currentDistance = DefaultDistance;
        TargetDistance = _currentDistance;

        _targetVerticalAngle = 0f;

        PlanarDirection = Vector3.forward;
    }

    // Set the transform that the camera will orbit around
    public void SetFollowTransform(Transform t, bool forceCustomRotation = false)
    {
        if (forceCustomRotation)
        {
            transform.rotation = t.rotation;
        }
        
        FollowTransform = t;
        PlanarDirection = FollowTransform.forward;
        _currentFollowPosition = FollowTransform.position;
    }

    [ReadOnly, ShowInInspector] private bool cameraLocked;
    private CameraLockTarget CameraLockTarget;
    public void SetCameraLockState(bool value) => cameraLocked = value;
    public void SetCameraLockTarget(CameraLockTarget value) => CameraLockTarget = value;
    
    private void LockOnTarget(float deltaTime)
    {
        if (CameraLockTarget == null) return;

        var directionToTarget = (CameraLockTarget.transform.position - FollowTransform.position).normalized;
        var targetRotation = Quaternion.LookRotation(directionToTarget);
        Transform.rotation = Quaternion.Slerp(Transform.rotation, targetRotation, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

        _currentDistance = Mathf.Lerp(_currentDistance, TargetDistance, 1f - Mathf.Exp(-DistanceMovementSharpness * deltaTime));

        _currentDistance = Mathf.Clamp(_currentDistance, MinDistance, TargetDistance);

        // Initialize a variable to track the closest obstruction
        var closestHit = new RaycastHit { distance = Mathf.Infinity };

        // Perform the obstruction check
        _obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -Transform.forward, _obstructions, TargetDistance, ObstructionLayers, QueryTriggerInteraction.Ignore);

        for (var i = 0; i < _obstructionCount; i++)
        {
            bool isIgnored = IgnoredColliders.Any(t => t == _obstructions[i].collider);

            if (!isIgnored && _obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0)
            {
                closestHit = _obstructions[i];
            }
        }

        // Adjust the camera distance based on obstructions
        if (closestHit.distance < Mathf.Infinity)
        {
            _distanceIsObstructed = true;
            _currentDistance = Mathf.Lerp(_currentDistance, closestHit.distance, 1 - Mathf.Exp(-ObstructionSharpness * deltaTime));
        }
        else
        {
            _distanceIsObstructed = false;
            _currentDistance = Mathf.Lerp(_currentDistance, TargetDistance, 1 - Mathf.Exp(-DistanceMovementSharpness * deltaTime));
        }

        // Ensure the distance is within acceptable bounds after adjustments
        _currentDistance = Mathf.Clamp(_currentDistance, MinDistance, TargetDistance);

        // Handle framing
        var targetPosition = FollowTransform.position - (Transform.forward * _currentDistance);
    
        targetPosition += Transform.right * FollowPointFraming.x;
        targetPosition += Transform.up * FollowPointFraming.y;

        // Apply position
        Transform.position = targetPosition;
    }

    public void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput)
    {
        if (cameraLocked && CameraLockTarget.IsAvailable)
        {
            LockOnTarget(deltaTime);
            _currentFollowPosition = FollowTransform.position + shakeOffset;
            return;
        }
        
        if (FollowTransform)
        {
            if (InvertX)
            {
                rotationInput.x *= -1f;
            }
            if (InvertY)
            {
                rotationInput.y *= -1f;
            }

            // Process rotation input
            Quaternion rotationFromInput = Quaternion.Euler(FollowTransform.up * (rotationInput.x * RotationSpeed));
            PlanarDirection = rotationFromInput * PlanarDirection;
            PlanarDirection = Vector3.Cross(FollowTransform.up, Vector3.Cross(PlanarDirection, FollowTransform.up));
            Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, FollowTransform.up);

            _targetVerticalAngle -= (rotationInput.y * RotationSpeed);
            _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
            Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
            Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

            // Apply rotation
            Transform.rotation = targetRotation;

            // Process distance input
            if (_distanceIsObstructed && Mathf.Abs(zoomInput) > 0f)
            {
                TargetDistance = _currentDistance;
            }
            TargetDistance += zoomInput * DistanceMovementSpeed;
            TargetDistance = Mathf.Clamp(TargetDistance, MinDistance, MaxDistance);

            // Find the smoothed follow position
            _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));

            // Handle obstructions
            {
                var closestHit = new RaycastHit
                {
                    distance = Mathf.Infinity
                };
                
                _obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -Transform.forward, _obstructions, TargetDistance, ObstructionLayers, QueryTriggerInteraction.Ignore);
                
                for (var i = 0; i < _obstructionCount; i++)
                {
                    bool isIgnored = IgnoredColliders.Any(t => t == _obstructions[i].collider) || IgnoredColliders.Any(t => t == _obstructions[i].collider);

                    if (!isIgnored && _obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0)
                    {
                        closestHit = _obstructions[i];
                    }
                }   

                // If obstructions detecter
                if (closestHit.distance < Mathf.Infinity)
                {
                    _distanceIsObstructed = true;
                    _currentDistance = Mathf.Lerp(_currentDistance, closestHit.distance, 1 - Mathf.Exp(-ObstructionSharpness * deltaTime));
                }
                // If no obstruction
                else
                {
                    _distanceIsObstructed = false;
                    _currentDistance = Mathf.Lerp(_currentDistance, TargetDistance, 1 - Mathf.Exp(-DistanceMovementSharpness * deltaTime));
                }
            }

            // Find the smoothed camera orbit position
            var targetPosition = _currentFollowPosition - ((targetRotation * Vector3.forward) * _currentDistance);

            // Handle framing
            targetPosition += Transform.right * FollowPointFraming.x;
            targetPosition += Transform.up * FollowPointFraming.y;

            // Apply position
            Transform.position = targetPosition + shakeOffset;
        }
    }

    public void StartShake()
    {
        if (isShaking) return;
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            float z = Random.Range(-1f, 1f) * shakeIntensity;

            shakeOffset = new Vector3(x, y, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        isShaking = false;
    }
}