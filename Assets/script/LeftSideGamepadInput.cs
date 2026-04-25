using UnityEngine;
using UnityEngine.InputSystem;

public class LeftSideGamepadInput : MonoBehaviour
{
    public Vector2 handMove;
    public Vector2 footMove;
    public bool gripHeld;

    [Header("Gamepad Index")]
    public int gamepadIndex = 0;

    [Header("Dead Zone")]
    public float deadZone = 0.2f;

    void Update()
    {
        if (Gamepad.all.Count <= gamepadIndex)
        {
            handMove = Vector2.zero;
            footMove = Vector2.zero;
            gripHeld = false;
            return;
        }

        Gamepad pad = Gamepad.all[gamepadIndex];

        handMove = pad.leftStick.ReadValue();
        footMove = pad.rightStick.ReadValue();
        gripHeld = pad.rightTrigger.isPressed;

        if (handMove.magnitude < deadZone) handMove = Vector2.zero;
        if (footMove.magnitude < deadZone) footMove = Vector2.zero;
    }
}