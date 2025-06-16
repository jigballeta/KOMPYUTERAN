using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;

    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask mask;
    [SerializeField] private PlayerUI playerUI;

    [Tooltip("Percentage of screen width reserved for joystick (e.g., 0.4 = 40%)")]
    [SerializeField] private float joystickZone = 0.4f;

    [Tooltip("Max movement (pixels) to count as tap, not swipe")]
    [SerializeField] private float maxTapMovement = 20f;

    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    void Awake()
    {
        cam = GetComponent<PlayerLook>().cam;

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        onFoot.Interact.performed += ctx => TryInteractCenter();
    }

    void OnEnable()
    {
        onFoot.Enable();
    }

    void OnDisable()
    {
        onFoot.Disable();
    }

    void Update()
    {
        ShowPromptFromCenter();
        HandleMouseInput();
        HandleTouchInput();
    }

    void ShowPromptFromCenter()
    {
        playerUI?.UpdateText(string.Empty);

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, mask))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                playerUI?.UpdateText(interactable.promptMessage);
            }
            else
            {
                CustomerAI customer = hitInfo.collider.GetComponent<CustomerAI>();
                if (customer != null && customer.manager.IsFirstInQueue(customer) && !customer.isPaid)
                {
                    playerUI?.UpdateText("Accept Payment");
                }
            }
        }
    }

    void TryInteractCenter()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, mask))
        {
            if (TryInteractWithCustomer(hitInfo)) return;

            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.BaseInteract();
            }
        }
    }

    void HandleMouseInput()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, mask))
            {
                if (TryInteractWithCustomer(hitInfo)) return;

                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }

    void HandleTouchInput()
    {
        if (Touchscreen.current == null) return;

        foreach (var touch in Touchscreen.current.touches)
        {
            if (!touch.press.wasPressedThisFrame) continue;

            Vector2 touchStart = touch.position.ReadValue();

            // Ignore UI
            if (IsPointerOverUIObject(touchStart)) continue;

            // Ignore joystick area
            if (touchStart.x < Screen.width * joystickZone) continue;

            if (touch.delta.ReadValue().magnitude > maxTapMovement) continue;

            Ray ray = cam.ScreenPointToRay(touchStart);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, mask))
            {
                if (TryInteractWithCustomer(hitInfo)) return;

                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }

    bool TryInteractWithCustomer(RaycastHit hitInfo)
    {
        CustomerAI customer = hitInfo.collider.GetComponent<CustomerAI>();
        if (customer != null && customer.manager.IsFirstInQueue(customer) && !customer.isPaid)
        {
            customer.AcceptPayment();
            return true;
        }

        return false;
    }

    bool IsPointerOverUIObject(Vector2 screenPos)
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






