using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorControl : MonoBehaviour
{
    private bool _cursorLocked;
    
    private void Start()
    {
        _cursorLocked = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    } 

    public void ToggleCursorLockState(InputAction.CallbackContext context)
    {
        _cursorLocked = !_cursorLocked;
        
        if(_cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!_cursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}