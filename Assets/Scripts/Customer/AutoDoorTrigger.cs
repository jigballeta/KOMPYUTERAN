using UnityEngine;

public class AutoDoorTrigger : MonoBehaviour
{
    [SerializeField] private Door door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Customer"))
        {
            if (door != null)
                door.BaseInteract(); // calls the door’s interact behavior
        }
    }
}

