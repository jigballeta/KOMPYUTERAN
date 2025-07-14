using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight;
    public Light[] indoorLights;

    public Gradient lightColor;
    public AnimationCurve lightIntensity;

    public float dayDurationMinutes = 12f; // real-time duration of the in-game day
    public TextMeshProUGUI timeDisplay;

    private float timePassed = 0f;
    private bool isDayRunning = false;

    [SerializeField] private MonitorController[] monitors;

    private float startHour = 8f;
    private float endHour = 22f; // 10:00 PM

    public static bool HasLoan = false;
    public static bool IsCafeOpen = false;
    public static bool IsSecondFloorUnlocked = false;

    public static DayNightCycle Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void StartDay()
    {
        timePassed = 0f;
        isDayRunning = true;
        SetIndoorLights(false);
        UpdateLighting(0f);
    }

    void Update()
    {
        if (!isDayRunning) return;

        timePassed += Time.deltaTime;
        float timePercent = timePassed / (dayDurationMinutes * 60f);
        float adjustedPercent = Mathf.Clamp01(timePercent * 0.75f + 0.25f);

        UpdateLighting(adjustedPercent);

        if (directionalLight.intensity < 0.3f)
            SetIndoorLights(true);

        UpdateClockUI(timePercent);

        if (timePercent >= 1f)
            EndDay();
    }

    void UpdateLighting(float percent)
    {
        float sunRotation = Mathf.Lerp(0f, 180f, percent);
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunRotation - 90f, 170f, 0f));
        directionalLight.color = lightColor.Evaluate(percent);
        directionalLight.intensity = lightIntensity.Evaluate(percent);
    }

    void SetIndoorLights(bool isOn)
    {
        foreach (Light light in indoorLights)
        {
            if (light != null)
                light.enabled = isOn;
        }
    }

    void UpdateClockUI(float percent)
    {
        float currentHour = Mathf.Lerp(startHour, endHour, percent);
        TimeSpan time = TimeSpan.FromHours(currentHour);
        if (timeDisplay != null)
            timeDisplay.text = time.ToString(@"hh\:mm");
    }

    void EndDay()
    {
        isDayRunning = false;
        SetIndoorLights(false);

        // All customers leave
        var allCustomers = FindObjectsByType<CustomerAI>(FindObjectsSortMode.None);
        foreach (var customer in allCustomers)
        {
            customer.StandUpAndLeave();
        }

        // Show "Day Over" UI prompt
        UIManager.Instance?.ShowDayOverPrompt();

        Debug.Log("Day ended. All customers are leaving.");
    }

    public void OpenCafe()
    {
        IsCafeOpen = true;
    }

    public void UnlockCafe()
    {
        IsCafeOpen = true;
    }

    public void UnlockSecondFloor()
    {
        IsSecondFloorUnlocked = true;
    }

    public void ReceiveLoanFromUncleBobby()
    {
        HasLoan = true;
    }
}
