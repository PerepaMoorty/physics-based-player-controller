using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTinkerer : MonoBehaviour
{
    [Header("Code References")]
    [SerializeField] private MovementController _moveController;

    [Header("Data Display - Movement")]
    [SerializeField] private TMP_Text _maxSpeed_Text;
    [SerializeField] private TMP_Text _moveSpeed_Text;
    [SerializeField] private TMP_Text _counterMoveSpeed_Text;
    [SerializeField] private TMP_Text _frictionValue_Text;

    [Header("Data Slider - Movement")]
    [SerializeField] private Slider _maxSpeed_Slider;
    [SerializeField] private Slider _moveSpeed_Slider;
    [SerializeField] private Slider _counterMoveSpeed_Slider;
    [SerializeField] private Slider _frictionValue_Slider;

    [Header("Data Display - Jumping")]
    [SerializeField] private TMP_Text _jumpForce_Text;
    [SerializeField] private TMP_Text _gravityForce_Text;
    [SerializeField] private TMP_Text _groundedGravityMultipler_Text;
    [SerializeField] private TMP_Text _slopeJumpMultiplier_Text;

    [Header("Data Slider - Jumping")]
    [SerializeField] private Slider _jumpForce_Slider;
    [SerializeField] private Slider _gravityForce_Slider;
    [SerializeField] private Slider _groundedGravityMultipler_Slider;
    [SerializeField] private Slider _slopeJumpMultiplier_Slider;

    [Header("Data Display - Wall Running")]
    [SerializeField] private TMP_Text _wallStickForce_Text;
    [SerializeField] private TMP_Text _offTheWall_Text;
    [SerializeField] private TMP_Text _alongTheWall_Text;
    [SerializeField] private TMP_Text _upTheWall_Text;

    [Header("Data Slider - Wall Running")]
    [SerializeField] private Slider _wallStickForce_Slider;
    [SerializeField] private Slider _offTheWall_Slider;
    [SerializeField] private Slider _alongTheWall_Slider;
    [SerializeField] private Slider _upTheWall_Slider;

    private void Awake()
    {
        // Movement Tab
        _maxSpeed_Slider.value = _moveController.maxSpeed;
        _moveSpeed_Slider.value = _moveController.moveSpeed;
        _counterMoveSpeed_Slider.value = _moveController.counterMoveSpeed;
        _frictionValue_Slider.value = _moveController.frictionValue;

        // Jumping Tab
        _jumpForce_Slider.value = _moveController.jumpForce;
        _gravityForce_Slider.value = _moveController.extraGravityForce;
        _groundedGravityMultipler_Slider.value = _moveController.groundGravityMultiplier;
        _slopeJumpMultiplier_Slider.value = _moveController.slopeJumpMultipler;

        // Wall Running Tab
        _wallStickForce_Slider.value = _moveController.wallStickForce;
        _offTheWall_Slider.value = _moveController._offTheWallMul;
        _alongTheWall_Slider.value = _moveController._forwardOffTheWallMul;
        _upTheWall_Slider.value = _moveController._upwardOffTheWallMul;
    }

    private void Update()
    {
        // Movement Tab
        _maxSpeed_Text.text = _moveController.maxSpeed.ToString();
        _moveSpeed_Text.text = _moveController.moveSpeed.ToString();
        _counterMoveSpeed_Text.text = _moveController.counterMoveSpeed.ToString();
        _frictionValue_Text.text = _moveController.frictionValue.ToString();
    
        // Jumping Tab
        _jumpForce_Text.text = _moveController.jumpForce.ToString();
        _gravityForce_Text.text = _moveController.extraGravityForce.ToString();
        _groundedGravityMultipler_Text.text = _moveController.groundGravityMultiplier.ToString();
        _slopeJumpMultiplier_Text.text = _moveController.slopeJumpMultipler.ToString();

        // Wall Running Tab
        _wallStickForce_Text.text = _moveController.wallStickForce.ToString();
        _offTheWall_Text.text = _moveController._offTheWallMul.ToString();
        _alongTheWall_Text.text = _moveController._forwardOffTheWallMul.ToString();
        _upTheWall_Text.text = _moveController._upwardOffTheWallMul.ToString();
    }

    // Movement Tab
    public void MaxSpeed() => _moveController.maxSpeed = _maxSpeed_Slider.value;
    public void MoveSpeed() => _moveController.moveSpeed = _moveSpeed_Slider.value;
    public void CounterMoveSpeed() => _moveController.counterMoveSpeed = _counterMoveSpeed_Slider.value;
    public void FrictionValue() => _moveController.frictionValue = _frictionValue_Slider.value;

    // Jumping Tab
    public void JumpForce() => _moveController.jumpForce = _jumpForce_Slider.value;
    public void GravityForce() => _moveController.extraGravityForce = _gravityForce_Slider.value;
    public void GroundedGravityMultiplier() => _moveController.groundGravityMultiplier = _groundedGravityMultipler_Slider.value;
    public void SlopeJumpMultiplier() => _moveController.slopeJumpMultipler = _slopeJumpMultiplier_Slider.value;

    // Wall Running Tab
    public void WallStickForce() => _moveController.wallStickForce = _wallStickForce_Slider.value;
    public void OffTheWallMultiplier() => _moveController._offTheWallMul = _offTheWall_Slider.value;
    public void AlongTheWallMultiplier() => _moveController._forwardOffTheWallMul = _alongTheWall_Slider.value;
    public void UpTheWallMultiplier() => _moveController._upwardOffTheWallMul = _upTheWall_Slider.value;

}