using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MyPlayerInput : MonoBehaviour
{
    [Header("Input States")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool shoot;

    [Header("Movement Options")]
    public bool analogMovement;

    [Header("Cursor Configuration")]
    public bool cursorLocked = true;
    public bool cursorAffectsLook = true;

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    public void OnLook(InputValue value)
    {
        if (cursorAffectsLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnShoot(InputValue value)
    {
        ShootInput(value.isPressed);
    }

    // Pause action: I'll map this in Input Actions (ESC key or Start button on controller)
    public void OnPause(InputValue value)
    {
        // if I pressed pause, just quit the game (works in build, not in editor)
        if (value.isPressed)
        {
            Application.Quit();
        }
    }
#endif

    public void MoveInput(Vector2 direction)
    {
        move = direction;
    }

    public void LookInput(Vector2 direction)
    {
        look = direction;
    }

    public void JumpInput(bool state)
    {
        jump = state;
    }

    public void SprintInput(bool state)
    {
        sprint = state;
    }

    public void ShootInput(bool state)
    {
        shoot = state;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void Update()
    {
        // fallback: if I hit ESC on keyboard, quit immediately
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }
}
