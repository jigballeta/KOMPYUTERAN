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

    // Mobile Controls
    public Joystick moveJoystick;
    public Button jumpButton;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

#if !UNITY_ANDROID && !UNITY_IOS
        onFoot.Jump.performed += ctx => motor.Jump();
#endif

        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(() => motor.Jump());
        }
    }

    void FixedUpdate()
    {
        Vector2 moveInput = onFoot.Movement.ReadValue<Vector2>();

#if UNITY_ANDROID || UNITY_IOS
        if (moveJoystick != null)
        {
            moveInput = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);
        }
#endif

        motor.ProcessMove(moveInput);
    }

    void LateUpdate()
    {
        Vector2 lookInput = Vector2.zero;

#if UNITY_ANDROID || UNITY_IOS
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == UnityEngine.TouchPhase.Moved && !IsPointerOverUIObject(touch))
            {
                lookInput = touch.deltaPosition * 0.1f;
                break; // Use the first valid non-UI touch
            }
        }
#else
        lookInput = onFoot.Look.ReadValue<Vector2>();
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
