using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage;

    public virtual void BaseInteract()

    {
        Interact();
    }

    protected virtual void Interact()
    {
        // Overridden in child classes
    }
}
