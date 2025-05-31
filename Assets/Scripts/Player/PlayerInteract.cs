using UnityEngine;
using UnityEngine.InputSystem;

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

        playerInput = new PlayerInput(); // Your generated input class
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
                // Handle customer speech prompt
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

            if (touchStart.x < Screen.width * joystickZone)
                continue;

            if (touch.delta.ReadValue().magnitude > maxTapMovement)
                continue;

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
}





