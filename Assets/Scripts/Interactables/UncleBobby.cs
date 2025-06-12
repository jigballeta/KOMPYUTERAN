using UnityEngine;

public class UncleBobbyInteract : Interactable
{
    public int loanAmount = 500; // Example amount
    public GameObject thoughtBubble;
    public GameObject dialogueTextObject; // TextMeshProUGUI object
    private TMPro.TextMeshProUGUI dialogueText;
    private bool hasLentMoney = false;

    private void Start()
    {
        if (dialogueTextObject != null)
            dialogueText = dialogueTextObject.GetComponent<TMPro.TextMeshProUGUI>();

        promptMessage = "Talk to Uncle Bobby";
        if (thoughtBubble != null)
            thoughtBubble.SetActive(false);
    }

    public override void BaseInteract()
    {
        if (hasLentMoney) return;

        hasLentMoney = true;

        // Show thought bubble with dialogue
        if (thoughtBubble != null)
            thoughtBubble.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = "Here’s ₱" + loanAmount + ". Use it to start your cafe!";

        // Add money to player
        UIManager.Instance.AddCash(loanAmount);

        // Unlock café
        DayNightCycle cycle = Object.FindFirstObjectByType<DayNightCycle>();

        Debug.Log("Uncle Bobby has lent the player money.");
    }
}
