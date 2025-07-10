using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// _MoveInputVector.x ---> W-S Input
/// _MoveInputVector.y ---> A-D Input
/// 
/// FindVelocity_RelativeDirection().x ---> Forward Component
/// FindVelocity_RelativeDirection().y ---> Right Component
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
    private float _groundAirMoveMultiplier = 1f;
    private float _counterMoveMultiplier = 1f;
    private float _fricitionMultiplier = 1f;
    private Vector2 _relativeVelocity;
    [HideInInspector] public Vector2 _MoveInputVector;

    [Header("Grounded Checks")]
    [SerializeField] private LayerMask groundLayer;
    [HideInInspector] public bool _IsGrounded;

    [Header("Slope Angles / Extra Gravity")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private float extraGravityForce;
    [SerializeField] private float groundGravityMultiplier;
    private Vector3 _groundNormal;
    private float _gravityToggle = 1f;
    [HideInInspector] public Vector3 _ContactAngle;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float slopeJumpMultipler;
    private bool _readyToJump = true;
    [HideInInspector] public float _JumpKeyPressed;
    
    [Header("Camera Effects")]
    [SerializeField] private float wallCameraLeanAngle;
    [SerializeField] private float _cameraLeanLerpTime;
    [SerializeField] private float _resetCamLeanMultiplier;
    private float _floatWallDirectionRelative;
    private bool _leanCameraNow;

    [Header("Wall Running")]
    [SerializeField] private float minWallAngle;
    [SerializeField] private float wallStickForce;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private float minWallRunSpeed;
    private Vector3 _wallNormal;
    private bool _readyToJumpOffWall = true;
    [HideInInspector] public bool _OnWall;

    [Header("Wall Running Multiplers")]
    [SerializeField] private float _offTheWallMul;
    [SerializeField] private float _forwardOffTheWallMul;
    [SerializeField] private float _upwardOffTheWallMul;

    private void Awake()
    {
        cameraRecomposer.Dutch = 0f;
        _leanCameraNow = false;
        _readyToJump = true;
        _readyToJumpOffWall = true;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0f, cameraOrientation.eulerAngles.y, 0f);

        // Camera Lean
        _floatWallDirectionRelative = (_wallNormal.y > 0f ? 1f : -1f);
        cameraRecomposer.Dutch = _leanCameraNow ? 
            Mathf.Lerp(cameraRecomposer.Dutch, wallCameraLeanAngle * _floatWallDirectionRelative, _cameraLeanLerpTime) : 
            Mathf.Lerp(cameraRecomposer.Dutch, 0f, _cameraLeanLerpTime * _resetCamLeanMultiplier);
    }
    private void FixedUpdate()
    {
        _relativeVelocity = FindVelocity_RelativeDirection(transform, playerBody);
        LimitMaxVelocity(_relativeVelocity);
        BasicMovement();
        SimulateFriction();
        CounterMovement();
        AdditionalGravity();

        if(_JumpKeyPressed != 0f)
            BasicJump();

        WallRun();
    }
    
    // Movement
    private void LimitMaxVelocity(Vector2 _relativeVelocity)
    {
        // Max Speed Limit
        if (_MoveInputVector.x > 0f && _relativeVelocity.x > maxSpeed) _MoveInputVector.x = 0f;
        if (_MoveInputVector.x < 0f && _relativeVelocity.x < -maxSpeed) _MoveInputVector.x = 0f;
        if (_MoveInputVector.y > 0f && _relativeVelocity.y > maxSpeed) _MoveInputVector.y = 0f;
        if (_MoveInputVector.y < 0f && _relativeVelocity.y < -maxSpeed) _MoveInputVector.y = 0f;
    }
    private void BasicMovement()
    {
        _groundAirMoveMultiplier = _IsGrounded ? 1f : 0.6f;

        playerBody.AddForce(transform.forward * _MoveInputVector.y * moveSpeed * _groundAirMoveMultiplier * Time.fixedDeltaTime, ForceMode.Acceleration);
        playerBody.AddForce(transform.right * _MoveInputVector.x * moveSpeed * _groundAirMoveMultiplier * Time.fixedDeltaTime, ForceMode.Acceleration);
    }
    private void CounterMovement()
    {
        _counterMoveMultiplier = _IsGrounded ? 1f : 0.45f;

        if ((_relativeVelocity.x < -0.01f && _MoveInputVector.x > 0f) || (_relativeVelocity.x > 0.01f && _MoveInputVector.x < 0f))
            playerBody.AddForce(transform.right * -_relativeVelocity.x * counterMoveSpeed * _counterMoveMultiplier * Time.fixedDeltaTime);
        if ((_relativeVelocity.y < -0.01f && _MoveInputVector.y > 0f) || (_relativeVelocity.y > 0.01f && _MoveInputVector.y < 0f))
            playerBody.AddForce(transform.forward * -_relativeVelocity.y * counterMoveSpeed * _counterMoveMultiplier * Time.fixedDeltaTime);
    }
    private void SimulateFriction()
    {
        // Ground and Air Friction
        _fricitionMultiplier = _IsGrounded ? 1f : 0.75f;

        if ((Mathf.Abs(_relativeVelocity.x) > 0.01f && Mathf.Abs(_MoveInputVector.x) < 0.05f))
            playerBody.AddForce(transform.right * -_relativeVelocity.x * frictionValue * _fricitionMultiplier * Time.fixedDeltaTime);
        if ((Mathf.Abs(_relativeVelocity.y) > 0.01f && Mathf.Abs(_MoveInputVector.y) < 0.05f))
            playerBody.AddForce(transform.forward * -_relativeVelocity.y * frictionValue * _fricitionMultiplier * Time.fixedDeltaTime);
    }
    private void AdditionalGravity()
    {
        if (!_IsGrounded)
        {
            playerBody.AddForce(Vector3.down * extraGravityForce * _gravityToggle * Time.fixedDeltaTime, ForceMode.Acceleration);
            return;
        }

        playerBody.AddForce(Vector3.down * extraGravityForce * _gravityToggle * groundGravityMultiplier * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    // Collision Detection
    private void OnCollisionStay(Collision collision)
    {
        if ((groundLayer & (1 << collision.gameObject.layer)) == 0) return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if(!isWall(collision.GetContact(i).normal))
                _ContactAngle = collision.GetContact(i).normal;

            if (isFloor(collision.GetContact(i).normal))
            {
                _IsGrounded = true;
                _groundNormal = collision.GetContact(i).normal;
                CancelInvoke(nameof(StopGrounded));
            }
            else if(isWall(collision.GetContact(i).normal))
            {
                _OnWall = true;
                _wallNormal = VectorDirection(cameraOrientation.forward, collision.GetContact(i).normal);
                CancelInvoke(nameof(StopOnWall));
            }
        }

        if(_IsGrounded)
            Invoke(nameof(StopGrounded), Time.deltaTime * 3f);
        
        if(_OnWall) Invoke(nameof(StopOnWall), Time.deltaTime * 3f);
    }
    private void StopGrounded() => _IsGrounded = false;
    private void StopOnWall() => _OnWall = false;

    // Jumping
    private void BasicJump()
    {
        if(_IsGrounded && _readyToJump)
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
        if (_OnWall && !_IsGrounded)
        {
            _leanCameraNow = true;
            _gravityToggle = playerBody.linearVelocity.magnitude >= minWallRunSpeed ? 0.25f : 0.75f;

            if (_JumpKeyPressed != 0f)
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
            playerBody.AddForce(VectorDirection(_wallNormal, cameraOrientation.forward) * wallJumpForce * _offTheWallMul * Time.fixedDeltaTime, ForceMode.Impulse);

            // Forward
            Vector3 _horizontalForward = new Vector3(cameraOrientation.forward.x, 0f, cameraOrientation.forward.z);
            playerBody.AddForce(_horizontalForward * wallJumpForce * _forwardOffTheWallMul * Time.fixedDeltaTime, ForceMode.Impulse);
            
            // Upward
            if(new Vector3(playerBody.linearVelocity.x, 0f, playerBody.linearVelocity.z).magnitude >= minWallRunSpeed)
            {
                ResetYVelocity();
                playerBody.AddForce(Vector3.up * wallJumpForce * _upwardOffTheWallMul * Time.fixedDeltaTime, ForceMode.Impulse);
                playerBody.AddForce(new Vector3(0f, cameraOrientation.forward.y, 0f) * wallJumpForce * _forwardOffTheWallMul * Time.fixedDeltaTime, ForceMode.Impulse);
            }
        
            if(_IsGrounded && playerBody.linearVelocity.magnitude <= minWallRunSpeed)
                playerBody.AddForce(new Vector3(0f, cameraOrientation.forward.y, 0f) * wallJumpForce * _forwardOffTheWallMul * Time.fixedDeltaTime, ForceMode.Impulse);

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