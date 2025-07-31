using UnityEngine;

/// <summary>
/// This is a velocity-based moving platform controller
/// - Allows for an End-Position as an Input, Since the start position is the intial position of the object itself
/// - Animation Curve to allow for speed easing 
/// </summary>

public class MovingPlatform : MonoBehaviour
{
    [Header("References")]
    private Rigidbody platformBody;

    [Header("Platform Positions")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 finalPosition;
    // ...
    private Vector3 _currentEndPoint;
    private float _maxDistance;
    private float _currentDistance;
    private bool _towardsEnd;

    [Header("Platform Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private AnimationCurve speedEaseCurve;
    // ...
    private float _finalEasedSpeed;

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
        if(_towardsEnd) _currentEndPoint = finalPosition;
        else _currentEndPoint = startPosition;

            _currentDistance = (transform.position - _currentEndPoint).magnitude;
        _finalEasedSpeed = moveSpeed * speedEaseCurve.Evaluate(_currentDistance / _maxDistance);

        platformBody.linearVelocity = ((_currentEndPoint - transform.position) / _currentDistance) * _finalEasedSpeed;

        if(_currentDistance <= 0.01f)
            _towardsEnd = !_towardsEnd;
    }
}