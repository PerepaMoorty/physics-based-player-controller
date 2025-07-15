using UnityEngine;

public class TabMove : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private RectTransform _transform;
    [SerializeField] private float _lerpTime;

    [Header("Y Values")]
    [SerializeField] private float inactiveY;
    [SerializeField] private float activeY;

    private bool _tabActive;
    private float _currentYValue;

    private void Start()
    {
        _tabActive = false;
        _currentYValue = inactiveY;
        _transform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _transform.localPosition = new Vector2(_transform.localPosition.x, Mathf.Lerp(_transform.localPosition.y, _currentYValue, _lerpTime));
    }

    public void ToggleTabState()
    {
        _tabActive = !_tabActive;
        if (_tabActive) _currentYValue = activeY;
        else _currentYValue = inactiveY;
    }
}