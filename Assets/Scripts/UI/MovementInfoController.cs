using TMPro;
using UnityEngine;

public class MovementInfoController : MonoBehaviour
{
    [Header("Velocity Info")]
    [SerializeField] private TMP_Text xVelocity_Text;
    [SerializeField] private TMP_Text yVelocity_Text;
    [SerializeField] private TMP_Text zVelocity_Text;
    [SerializeField] private TMP_Text totalSpeed_Text;
    // ...
    private string label_xVelocity;
    private string label_yVelocity;
    private string label_zVelocity;
    private string label_totalSpeed;

    [Header("Contact Info")]
    [SerializeField] private TMP_Text onGround_Text;
    [SerializeField] private TMP_Text onWall_Text;
    [SerializeField] private TMP_Text slopeAngle_Text;
    // ...
    private string label_onGround;
    private string label_onWall;
    private string label_slopeAngle;

    [Header("Component References")]
    [SerializeField] private Rigidbody playerRigidBody;
    [SerializeField] private MovementController playerMovementController;

    private void Start()
    {
        label_xVelocity = xVelocity_Text.text;
        label_yVelocity = yVelocity_Text.text;
        label_zVelocity = zVelocity_Text.text;
        label_totalSpeed = totalSpeed_Text.text;
    
        label_onGround = onGround_Text.text;
        label_onWall = onWall_Text.text;
        label_slopeAngle = slopeAngle_Text.text;
    }

    private void Update()
    {
        yVelocity_Text.text = label_yVelocity + " " + playerRigidBody.linearVelocity.y.ToString("F2");
        zVelocity_Text.text = label_zVelocity + " " + playerRigidBody.linearVelocity.z.ToString("F2");
        xVelocity_Text.text = label_xVelocity + " " + playerRigidBody.linearVelocity.x.ToString("F2");
        totalSpeed_Text.text = label_totalSpeed + " " + playerRigidBody.linearVelocity.magnitude.ToString("F2");

        onGround_Text.text = label_onGround + " " + playerMovementController.OnGround.ToString();
        onWall_Text.text = label_onWall + " " + playerMovementController.OnWall.ToString();
        slopeAngle_Text.text = label_slopeAngle + " " + Vector3.Angle(Vector3.up, playerMovementController.ContactAngle).ToString("F2") + "°";
    }
}