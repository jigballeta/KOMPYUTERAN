using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator animator;
    public GameObject lockedText;

    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log("Player interacted with door.");

        DayNightCycle dayNight = FindFirstObjectByType<DayNightCycle>();
        if (dayNight != null)
        {
            Debug.Log("DayNightCycle found. isCafeUnlocked = " + dayNight.isCafeUnlocked);

            if (!dayNight.isCafeUnlocked)
            {
                Debug.Log("Cafe is locked. Prompt player to talk to Uncle Bobby.");
                if (lockedText != null)
                    lockedText.SetActive(true);
                return;
            }
        }
        else
        {
            Debug.LogWarning("DayNightCycle not found in scene.");
            return;
        }

        // If unlocked, open the door
        animator.SetTrigger("Open");
        Debug.Log("Door opened.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // Hide locked message when player walks away
            if (lockedText != null)
                lockedText.SetActive(false);
        }
    }

    public void Open()
    {
        // Your logic here, for example:
        Debug.Log("Door opened.");
        // Animate or disable collider, etc.
    }

}
