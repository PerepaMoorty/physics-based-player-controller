using UnityEngine;

/// <summary>
/// Velocity-based moving platform controller
/// - Uses Rigidbody linear velocity for motion
/// - Supports easing via AnimationCurve
/// - Exposes CurrentVelocity for player controller
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [Header("References")]
    private Rigidbody platformBody;

    [Header("Platform Positions")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 finalPosition;
    private Vector3 _currentEndPoint;
    private float _maxDistance;
    private float _currentDistance;
    private bool _towardsEnd;

    [Header("Platform Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private AnimationCurve speedEaseCurve;
    private float _finalEasedSpeed;

    public Vector3 CurrentVelocity { get; private set; }

    private void Start()
    {
        platformBody = GetComponent<Rigidbody>();
        _towardsEnd = true;
        _currentEndPoint = finalPosition;

        startPosition = transform.position;
        _maxDistance = (finalPosition - startPosition).magnitude;
    }

    private void Update()
    {
        _currentEndPoint = _towardsEnd ? finalPosition : startPosition;

        _currentDistance = (transform.position - _currentEndPoint).magnitude;
        _finalEasedSpeed = moveSpeed * speedEaseCurve.Evaluate(_currentDistance / _maxDistance);

        CurrentVelocity = ((_currentEndPoint - transform.position) / _currentDistance) * _finalEasedSpeed;
        platformBody.linearVelocity = CurrentVelocity;

        if (_currentDistance <= 0.01f)
            _towardsEnd = !_towardsEnd;
    }
}
