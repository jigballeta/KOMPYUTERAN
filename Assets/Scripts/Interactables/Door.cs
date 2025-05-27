using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private Transform doorHinge;
    [SerializeField] private Vector3 closedRotation = Vector3.zero;
    [SerializeField] private Vector3 openRotation = new Vector3(0f, 90f, 0f);

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioSource audioSource;

    private bool isOpen = false;

    private void Start()
    {
        SetDoorState(false);
        UpdatePromptMessage();
    }

    protected override void Interact()
    {
        isOpen = !isOpen;
        SetDoorState(isOpen);
        PlaySound(isOpen);
        UpdatePromptMessage();
    }

    private void SetDoorState(bool open)
    {
        if (doorHinge != null)
        {
            doorHinge.localEulerAngles = open ? openRotation : closedRotation;
        }
    }

    private void PlaySound(bool opening)
    {
        if (audioSource == null) return;

        AudioClip clip = opening ? openSound : closeSound;
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void UpdatePromptMessage()
    {
        promptMessage = isOpen ? "Close Door" : "Open Door";
    }
}

