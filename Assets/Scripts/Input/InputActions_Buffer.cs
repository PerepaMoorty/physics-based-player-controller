using UnityEngine;
using UnityEngine.SceneManagement;

public class InputActions_Buffer : MonoBehaviour
{
    [Header("References")]
    private MovementController _movementController;
    private CursorControl _cursorController;
    private IA_Player player_IA;

    private void Start()
    {
        _movementController = GetComponent<MovementController>();
        _cursorController = GetComponent<CursorControl>();

        player_IA = new IA_Player();
        player_IA.Movement.Enable();

        player_IA.Movement.Unlock_Cursor.performed += _cursorController.ToggleCursorLockState;
    }
    private void Update()
    {
        _movementController.MoveInputVector = player_IA.Movement.Move.ReadValue<Vector2>();
        _movementController.JumpKeyPressed = player_IA.Movement.Jump.ReadValue<float>();

        if (Input.GetKeyDown(KeyCode.T))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDisable() => player_IA.Movement.Disable();
    private void OnDestroy() => player_IA.Movement.Disable();
}