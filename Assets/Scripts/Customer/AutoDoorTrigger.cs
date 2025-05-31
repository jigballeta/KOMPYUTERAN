using UnityEngine;

public class AutoDoorTrigger : MonoBehaviour
{
    public Door door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Customer") && door != null)
        {
            door.Open();
        }
    }
}
