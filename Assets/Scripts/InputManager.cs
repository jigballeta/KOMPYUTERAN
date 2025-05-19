using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    private PlayerLook look;
    public InputAction interact;

    public Joystick moveJoystick;
    public Button jumpButton;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        interact = onFoot.Interact;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        // Keyboard jump
        onFoot.Jump.performed += ctx => motor.Jump();

        // Mobile jump button
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(() => motor.Jump());
        }
    }

    void FixedUpdate()
    {
        Vector2 moveInput = onFoot.Movement.ReadValue<Vector2>();

#if UNITY_ANDROID || UNITY_IOS
        if (moveJoystick != null && (Mathf.Abs(moveJoystick.Horizontal) > 0.1f || Mathf.Abs(moveJoystick.Vertical) > 0.1f))
        {
            moveInput = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);
        }
#endif

        motor.ProcessMove(moveInput);
    }

    void LateUpdate()
    {
        Vector2 lookInput = onFoot.Look.ReadValue<Vector2>();

#if UNITY_ANDROID || UNITY_IOS
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == UnityEngine.TouchPhase.Moved)

            {
                if (IsPointerOverUIObject(touch.position)) continue;

                // Ignore left-side (joystick area)
                if (touch.position.x < Screen.width * 0.4f) continue;

                lookInput = touch.deltaPosition * 0.1f;
                break;
            }
        }
#endif

        look.ProcessLook(lookInput);
    }

    private void OnEnable()
    {
        playerInput.Enable();
        onFoot.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
        onFoot.Disable();
    }

    private bool IsPointerOverUIObject(Vector2 screenPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}


