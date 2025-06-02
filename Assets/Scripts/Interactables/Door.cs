using UnityEngine;
using UnityEngine.AI;

public class Door : Interactable
{
    [SerializeField] private Transform doorHinge;
    [SerializeField] private Vector3 closedRotation = Vector3.zero;
    [SerializeField] private Vector3 openRotation = new Vector3(0f, 90f, 0f);

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private NavMeshObstacle navObstacle;

    private bool isOpen = false;
    private Coroutine autoCloseCoroutine;

    private void Start()
    {
        SetDoorState(false);
        UpdatePromptMessage();

        if (navObstacle != null)
            navObstacle.enabled = true;
    }

    protected override void Interact()
    {
        if (!isOpen)
            Open();
        else
            Close();
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            SetDoorState(true);
            PlaySound(openSound);
            if (navObstacle != null) navObstacle.enabled = false;
            UpdatePromptMessage();
        }

        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);

        autoCloseCoroutine = StartCoroutine(CloseAfterDelay(3f));
    }

    public void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            SetDoorState(false);
            PlaySound(closeSound);
            if (navObstacle != null) navObstacle.enabled = true;
            UpdatePromptMessage();
        }
    }

    private System.Collections.IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Close();
    }

    private void SetDoorState(bool open)
    {
        if (doorHinge != null)
            doorHinge.localEulerAngles = open ? openRotation : closedRotation;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void UpdatePromptMessage()
    {
        promptMessage = isOpen ? "Close Door" : "Open Door";
    }
}
