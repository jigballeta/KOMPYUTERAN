using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Cash UI")]
    public TextMeshProUGUI cashText;
    private int totalCash = 0;
    public int TotalCash => totalCash;

    [Header("Day Over Prompt")]
    public TextMeshProUGUI dayOverPrompt;

    [Header("Day Complete Prompt")]
    public TextMeshProUGUI dayCompletePrompt;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateCashDisplay();

        if (dayOverPrompt != null)
            dayOverPrompt.gameObject.SetActive(false);

        if (dayCompletePrompt != null)
            dayCompletePrompt.gameObject.SetActive(false);
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

    public void ShowDayOverPrompt()
    {
        if (dayOverPrompt != null)
        {
            dayOverPrompt.text = "Day Over!";
            dayOverPrompt.gameObject.SetActive(true);
        }
    }

    public void ShowDayCompleteMessage(int dayNumber)
    {
        if (dayCompletePrompt != null)
        {
            dayCompletePrompt.text = $"Day {dayNumber} Complete!";
            dayCompletePrompt.gameObject.SetActive(true);

            CancelInvoke(nameof(HideDayCompletePrompt));
            Invoke(nameof(HideDayCompletePrompt), 3f); // Hide after 3 seconds
        }
    }

    private void HideDayCompletePrompt()
    {
        if (dayCompletePrompt != null)
            dayCompletePrompt.gameObject.SetActive(false);
    }
}