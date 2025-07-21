using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Physics-Based Player Movement Controller:
/// 
/// A general use case 3D Physics based movement controller for games
/// Features:
/// - Basic Movement using the new Input system
///     -- MoveInputVector, JumpKeyPressed are the inputs which can modified using an Input Controller script
/// - Wall Running
/// - Camera Lean with modifiable camera dutch settings
/// 
/// </summary>

public class MovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private Transform cameraOrientation;
    [SerializeField] private CinemachineRecomposer cameraRecomposer;

    [Header("Basic Movement Variables")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float counterMoveSpeed;
    [SerializeField] private float frictionValue;
    // ... 
    private float _groundAirMoveMultiplier = 1f;
    private float _counterMoveMultiplier = 1f;
    private float _fricitionMultiplier = 1f;
    private Vector2 _relativeVelocity;
    [HideInInspector] public Vector2 MoveInputVector;

    [Header("Air Movement Multiplers")]
    [SerializeField] private float airMoveMultipler;
    [SerializeField] private float counterAirMoveMultipler;
    [SerializeField] private float airFrictionMultipler;

    [Header("Grounded Checks")]
    // ...
    [SerializeField] private LayerMask groundLayer;
    [HideInInspector] public bool IsGrounded;

    [Header("Slope Angles / Extra Gravity")]
    [SerializeField] private float extraGravityForce;
    [SerializeField] private float groundGravityMultiplier;
    // ...
    [SerializeField] private float maxSlopeAngle;
    private Vector3 _groundNormal;
    private float _gravityToggle = 1f;
    [HideInInspector] public Vector3 ContactAngle;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float slopeJumpMultipler;
    // ...
    private bool _readyToJump = true;
    [HideInInspector] public float JumpKeyPressed;
    
    [Header("Camera Effects")]
    [SerializeField] private float wallCameraLeanAngle;
    [SerializeField] private float cameraLeanLerpTime;
    [SerializeField] private float resetCamLeanMultiplier;
    // ...
    private float _floatWallDirectionRelative;
    private bool _leanCameraNow;

    [Header("Wall Running")]
    [SerializeField] private float wallStickForce;
    [SerializeField] private float minWallRunSpeed;
    [SerializeField] private float minWallAngle;
    // ...
    private Vector3 _wallNormal;
    private bool _readyToJumpOffWall = true;
    [HideInInspector] public bool OnWall;

    [Header("Wall Running Multiplers")]
    [SerializeField] private float offTheWallMul;
    [SerializeField] private float forwardOffTheWallMul;
    [SerializeField] private float upwardOffTheWallMul;

    private void Awake()
    {
        // Initalising Component Values [This Controller requires the use of Cinemachine-Camera]
        cameraRecomposer.Dutch = 0f;
        _leanCameraNow = false;
        _readyToJump = true;
        _readyToJumpOffWall = true;
    }

    private void Update()
    {
        // Physical Rotation of the Player Body
        transform.rotation = Quaternion.Euler(0f, cameraOrientation.eulerAngles.y, 0f);

        // Camera Lean
        _floatWallDirectionRelative = (_wallNormal.y > 0f ? 1f : -1f);
        cameraRecomposer.Dutch = _leanCameraNow ? 
            Mathf.Lerp(cameraRecomposer.Dutch, wallCameraLeanAngle * _floatWallDirectionRelative, cameraLeanLerpTime) : 
            Mathf.Lerp(cameraRecomposer.Dutch, 0f, cameraLeanLerpTime * resetCamLeanMultiplier);
    }
    private void FixedUpdate()
    {
        _relativeVelocity = FindVelocity_RelativeDirection(transform, playerBody);
        LimitMaxVelocity(_relativeVelocity);
        BasicMovement();
        SimulateFriction();
        CounterMovement();
        AdditionalGravity();

        if(JumpKeyPressed != 0f)
            BasicJump();

        WallRun();
    }
    
    // Movement
    private void LimitMaxVelocity(Vector2 _relativeVelocity)
    {
        // Max Speed Limit - rejects input if the speed of the player is past the maxSpeed
        if (MoveInputVector.x > 0f && _relativeVelocity.x > maxSpeed) MoveInputVector.x = 0f;
        if (MoveInputVector.x < 0f && _relativeVelocity.x < -maxSpeed) MoveInputVector.x = 0f;
        if (MoveInputVector.y > 0f && _relativeVelocity.y > maxSpeed) MoveInputVector.y = 0f;
        if (MoveInputVector.y < 0f && _relativeVelocity.y < -maxSpeed) MoveInputVector.y = 0f;
    }
    private void BasicMovement()
    {
        // Air Movement Limiter
        _groundAirMoveMultiplier = IsGrounded ? 1f : airMoveMultipler;

        playerBody.AddForce(transform.forward * MoveInputVector.y * moveSpeed * _groundAirMoveMultiplier * Time.fixedDeltaTime, ForceMode.Acceleration);
        playerBody.AddForce(transform.right * MoveInputVector.x * moveSpeed * _groundAirMoveMultiplier * Time.fixedDeltaTime, ForceMode.Acceleration);
    }
    private void CounterMovement()
    {
        _counterMoveMultiplier = IsGrounded ? 1f : counterAirMoveMultipler;

        if ((_relativeVelocity.x < -0.01f && MoveInputVector.x > 0f) || (_relativeVelocity.x > 0.01f && MoveInputVector.x < 0f))
            playerBody.AddForce(transform.right * -_relativeVelocity.x * counterMoveSpeed * _counterMoveMultiplier * Time.fixedDeltaTime);
        if ((_relativeVelocity.y < -0.01f && MoveInputVector.y > 0f) || (_relativeVelocity.y > 0.01f && MoveInputVector.y < 0f))
            playerBody.AddForce(transform.forward * -_relativeVelocity.y * counterMoveSpeed * _counterMoveMultiplier * Time.fixedDeltaTime);
    }
    private void SimulateFriction()
    {
        // Ground and Air Friction
        _fricitionMultiplier = IsGrounded ? 1f : airFrictionMultipler;

        if ((Mathf.Abs(_relativeVelocity.x) > 0.01f && Mathf.Abs(MoveInputVector.x) < 0.05f))
            playerBody.AddForce(transform.right * -_relativeVelocity.x * frictionValue * _fricitionMultiplier * Time.fixedDeltaTime);
        if ((Mathf.Abs(_relativeVelocity.y) > 0.01f && Mathf.Abs(MoveInputVector.y) < 0.05f))
            playerBody.AddForce(transform.forward * -_relativeVelocity.y * frictionValue * _fricitionMultiplier * Time.fixedDeltaTime);
    }
    private void AdditionalGravity()
    {
        // Adding Different Values of Gravity depending on grounded-State
        if (!IsGrounded)
        {
            playerBody.AddForce(Vector3.down * extraGravityForce * _gravityToggle * Time.fixedDeltaTime, ForceMode.Acceleration);
            return;
        }

        playerBody.AddForce(Vector3.down * extraGravityForce * _gravityToggle * groundGravityMultiplier * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    // Collision Detection
    private void OnCollisionStay(Collision collision)
    {
        // Bit-Wise check to reject other layers
        if ((groundLayer & (1 << collision.gameObject.layer)) == 0) return;

        // Iterating through every collider in the Collision array
        for (int i = 0; i < collision.contactCount; i++)
        {
            if(!isWall(collision.GetContact(i).normal))
                ContactAngle = collision.GetContact(i).normal;

            if (isFloor(collision.GetContact(i).normal))
            {
                IsGrounded = true;
                _groundNormal = collision.GetContact(i).normal;
                CancelInvoke(nameof(StopGrounded));
            }
            else if(isWall(collision.GetContact(i).normal))
            {
                OnWall = true;
                _wallNormal = VectorDirection(cameraOrientation.forward, collision.GetContact(i).normal);
                CancelInvoke(nameof(StopOnWall));
            }
        }

        if(IsGrounded)
            Invoke(nameof(StopGrounded), Time.deltaTime * 3f);
        
        if(OnWall) Invoke(nameof(StopOnWall), Time.deltaTime * 3f);
    }
    private void StopGrounded() => IsGrounded = false;
    private void StopOnWall() => OnWall = false;

    // Jumping
    private void BasicJump()
    {
        if(IsGrounded && _readyToJump)
        {
            _readyToJump = false;

            playerBody.AddForce(Vector3.up * jumpForce * Time.fixedDeltaTime, ForceMode.Impulse);
            playerBody.AddForce(_groundNormal * jumpForce * slopeJumpMultipler * Time.fixedDeltaTime, ForceMode.Impulse);
            
            ResetYVelocity();
        }

        if (!_readyToJump)
            Invoke(nameof(ResetJump), Time.deltaTime * 3f);
    }
    private void ResetJump() => _readyToJump = true;

    // Wall Running
    private void WallRun()
    {
        if (OnWall && !IsGrounded)
        {
            _leanCameraNow = true;
            _gravityToggle = playerBody.linearVelocity.magnitude >= minWallRunSpeed ? 0.25f : 0.75f;

            if (JumpKeyPressed != 0f)
            {
                JumpOffWall();
                return;
            }

            // Adding Extra Forces - Player Sticks to the Wall
            playerBody.AddForce(VectorDirection(cameraOrientation.forward, _wallNormal) 
                * (new Vector3(playerBody.linearVelocity.x, 0f, playerBody.linearVelocity.z).magnitude >= 1f 
                ? 1f : new Vector3(playerBody.linearVelocity.x, 0f, playerBody.linearVelocity.z).magnitude) 
                * wallStickForce 
                * Time.fixedDeltaTime, 
                ForceMode.Force);
        }
        else
        {
            _leanCameraNow = false;
            _gravityToggle = 1f;
            _readyToJumpOffWall = true;
        }
    }
    private void JumpOffWall()
    {
        if(_readyToJumpOffWall)
        {
            _readyToJumpOffWall = false;

            // Off the Walls
            playerBody.AddForce(VectorDirection(_wallNormal, cameraOrientation.forward) * wallStickForce * offTheWallMul * Time.fixedDeltaTime, ForceMode.Impulse);

            // Forward
            Vector3 _horizontalForward = new Vector3(cameraOrientation.forward.x, 0f, cameraOrientation.forward.z);
            playerBody.AddForce(_horizontalForward * wallStickForce * forwardOffTheWallMul * Time.fixedDeltaTime, ForceMode.Impulse);
            
            // Upward
            if(new Vector3(playerBody.linearVelocity.x, 0f, playerBody.linearVelocity.z).magnitude >= minWallRunSpeed)
            {
                ResetYVelocity();
                playerBody.AddForce(Vector3.up * wallStickForce * upwardOffTheWallMul * Time.fixedDeltaTime, ForceMode.Impulse);
                playerBody.AddForce(new Vector3(0f, cameraOrientation.forward.y, 0f) * wallStickForce * forwardOffTheWallMul * Time.fixedDeltaTime, ForceMode.Impulse);
            }
        
            if(IsGrounded && playerBody.linearVelocity.magnitude <= minWallRunSpeed)
                playerBody.AddForce(new Vector3(0f, cameraOrientation.forward.y, 0f) * wallStickForce * forwardOffTheWallMul * Time.fixedDeltaTime, ForceMode.Impulse);

        }
    }

    // General Tools
    private Vector2 FindVelocity_RelativeDirection(Transform cameraTransform, Rigidbody rigidbody)
    {
        // Both Values in Degrees
        float cameraLook_Angle = cameraTransform.rotation.eulerAngles.y;
        float velocityVector_Angle = Mathf.Atan2(rigidbody.linearVelocity.x, rigidbody.linearVelocity.z) * Mathf.Rad2Deg;

        float deltaAngle = Mathf.DeltaAngle(cameraLook_Angle, velocityVector_Angle) * Mathf.Deg2Rad;
        
        float vX_Relative = rigidbody.linearVelocity.magnitude * Mathf.Cos(deltaAngle);
        float vY_Relative = rigidbody.linearVelocity.magnitude * Mathf.Sin(deltaAngle);

        return new Vector2(vY_Relative, vX_Relative);
    }
    private bool isFloor(Vector3 _vector)
    {
        float angle = Vector3.Angle(Vector3.up, _vector);
        return angle <= maxSlopeAngle;
    }
    private bool isWall(Vector3 _vector)
    {
        float angle = Vector3.Angle(Vector3.up, _vector);
        return angle > minWallAngle;
    }
    private Vector3 VectorDirection(Vector3 _referenceVector, Vector3 _checkVector)
    {
        Vector3 _referenceVector_Corrected = new Vector3(_referenceVector.x, 0f, _referenceVector.z);
        Vector3 _checkVectorCorrected = new Vector3(_checkVector.x, 0f, _checkVector.z);
        
        Vector3 _crossedVector = Vector3.Cross(_checkVector, _referenceVector);
        
        return _crossedVector / _crossedVector.magnitude;
    }
    private void ResetYVelocity()
    {
        if (playerBody.linearVelocity.y < 0.01f)
            playerBody.linearVelocity = new Vector3(playerBody.linearVelocity.x, 0f, playerBody.linearVelocity.z);
    }
}