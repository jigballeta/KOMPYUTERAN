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

    // Mobile UI references
    public Joystick moveJoystick;
    public Button jumpButton;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        // Keyboard jump support
        onFoot.Jump.performed += ctx => motor.Jump();

        // Mobile button jump support
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(() => motor.Jump());
        }
    }

    void FixedUpdate()
    {
        Vector2 moveInput = onFoot.Movement.ReadValue<Vector2>();

#if UNITY_ANDROID || UNITY_IOS
        // Use joystick on mobile if in use
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
            // Prevent conflict if dragging over UI (joystick, buttons)
            if (touch.phase == UnityEngine.TouchPhase.Moved && !IsPointerOverUIObject(touch))
            {
                lookInput = touch.deltaPosition * 0.1f;
                break; // Use the first valid finger for camera look
            }
        }
#endif

        look.ProcessLook(lookInput);
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }

    /// <summary>
    /// Prevents input conflicts with on-screen UI buttons (jump/joystick).
    /// </summary>
    private bool IsPointerOverUIObject(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = touch.position
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
