using UnityEngine;

public class BuyCafeInteract : Interactable
{
    public int price = 300;
    public string unlockedPrompt = "Cafe is now open!";
    private bool isPurchased = false;

    private void Start()
    {
        promptMessage = "Buy Café (₱" + price + ")";
    }

    public override void BaseInteract()
    {
        if (isPurchased)
        {
            promptMessage = unlockedPrompt;
            return;
        }

        if (!DayNightCycle.HasLoan)
        {
            promptMessage = "Locked - Talk to Uncle Bobby";
            return;
        }

        if (UIManager.Instance.TotalCash < price)
        {
            promptMessage = "Not enough money!";
            return;
        }

        UIManager.Instance.DeductCash(price);
        isPurchased = true;
        promptMessage = unlockedPrompt;

        // Unlock the café now
        FindFirstObjectByType<DayNightCycle>().UnlockCafe();


        Debug.Log("Café purchased!");
    }
}
