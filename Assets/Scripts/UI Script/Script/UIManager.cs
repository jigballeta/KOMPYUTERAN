using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI cashText;
    private int totalCash = 0;

    public int TotalCash => totalCash;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCash(int amount)
    {
        totalCash += amount;
        UpdateCashDisplay();
    }

    public void DeductCash(int amount)
    {
        totalCash = Mathf.Max(0, totalCash - amount);
        UpdateCashDisplay();
    }

    public void SetCash(int amount)
    {
        totalCash = Mathf.Max(0, amount);
        UpdateCashDisplay();
    }

    private void UpdateCashDisplay()
    {
        if (cashText != null)
            cashText.text = $"₱ {totalCash}";
    }
}


