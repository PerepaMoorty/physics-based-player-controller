using TMPro;
using UnityEngine;

public class InfoDisplay : MonoBehaviour
{
    [Header("Code References")]
    [SerializeField] private MovementController _moveController;

    [Header("UI Elements - Velocity and Speed")]
    [SerializeField] private TMP_Text _velocityX_Text;
    [SerializeField] private TMP_Text _velocityY_Text;
    [SerializeField] private TMP_Text _velocityZ_Text;
    [SerializeField] private TMP_Text _speed_Text;
    
    [Header("UI Elements - Checks")]
    [SerializeField] private TMP_Text _slopeAngle_Text;
    [SerializeField] private TMP_Text _isGrounded_Text;
    [SerializeField] private TMP_Text _onWall_Text;

    [Header("Private Calculators")]
    private Vector3 _relativeVelocity;

    private void Update()
    {
        _relativeVelocity = new Vector3(_moveController._RelativeVelocity.x, _moveController.GetComponent<Rigidbody>().linearVelocity.y, _moveController._RelativeVelocity.y);
        _velocityX_Text.text = string.Format("Velocity X: {0:0.00}", _relativeVelocity.x);
        _velocityY_Text.text = string.Format("Velocity Y: {0:0.00}", _relativeVelocity.y);
        _velocityZ_Text.text = string.Format("Velocity Z: {0:0.00}", _relativeVelocity.z);
        _speed_Text.text = string.Format("Speed: {0:0.00}", _relativeVelocity.magnitude);

        _slopeAngle_Text.text = string.Format("Slope Angle: {0:0.00}°", Vector3.Angle(Vector3.up, _moveController._ContactAngle));
        _isGrounded_Text.text = _moveController._IsGrounded ? "Is Grounded: Yes" : "Is Grounded: No";
        _onWall_Text.text = _moveController._OnWall ? "On Wall: Yes" : "On Wall: No";
    }
}