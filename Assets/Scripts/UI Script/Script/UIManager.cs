using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI cashText;
    private int totalCash = 0;

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
        if (cashText != null)
            cashText.text = $"₱ {totalCash}";
    }
}


