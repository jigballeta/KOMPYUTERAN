using UnityEngine;

public class GPU : Interactable
{
    void Start()
    {
        promptMessage = "Pickup RTX 6969";
    }

    protected override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}

