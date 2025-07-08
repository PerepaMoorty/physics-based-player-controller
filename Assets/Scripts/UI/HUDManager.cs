using System;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [Header("Code References")]
    [SerializeField] private MovementController playerController;
    [SerializeField] private Rigidbody playerBody;

    [Header("UI References")]
    [SerializeField] private TMP_Text speedoText;
    [SerializeField] private TMP_Text groundedText;
    [SerializeField] private TMP_Text slopeAngleText;
    [SerializeField] private TMP_Text onWallText;

    private float _playerVelocity;
    private string _playerGrounded;
    private string _playerOnWall;
    private float _slopeAngle;

    private void Update()
    {
        _playerVelocity = playerBody.linearVelocity.magnitude >= 0.01f ? playerBody.linearVelocity.magnitude : 0f;
        speedoText.text = String.Format("Speed: {0:0.00}", _playerVelocity);

        _playerGrounded = playerController._IsGrounded ? "Yes" : "No";
        groundedText.text = "Grounded: " + _playerGrounded;

        _playerOnWall = playerController._OnWall ? "Yes" : "No";
        onWallText.text = "On Wall: " + _playerOnWall;

        _slopeAngle = Vector3.Angle(Vector3.up, playerController._SlopeNormal);
        slopeAngleText.text = String.Format("Slope Angle: {0:0.00}°", _slopeAngle);
    }
}