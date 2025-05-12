using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;


public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    private PlayerLook look;

    // FOR MOBILE
    public Joystick moveJoystick;
    public Button jumpButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        onFoot.Jump.performed += ctx => motor.Jump();
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(() => motor.Jump());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moveInput = onFoot.Movement.ReadValue<Vector2>();

        // Add joystick input (if assigned)
        if (moveJoystick != null)
        {
            moveInput += new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);
        }

        motor.ProcessMove(moveInput);
    }

    void LateUpdate()
    {
        Vector2 lookInput = onFoot.Look.ReadValue<Vector2>();

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == UnityEngine.TouchPhase.Moved && !IsPointerOverUIObject(touch))
            {
                lookInput += touch.deltaPosition * 0.1f;
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

    private bool IsPointerOverUIObject(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
