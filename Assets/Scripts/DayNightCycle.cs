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

    public float dayDurationMinutes = 12f;
    public TextMeshProUGUI timeDisplay;

    private float timePassed = 0f;
    private bool isDayRunning = false;

    public float startHour = 8f;
    public float endHour = 22f;

    public static bool HasLoan = false;
    public static bool IsCafeOpen = false;
    public static bool IsSecondFloorUnlocked = false;

    public int currentDay = 1;
    public static DayNightCycle Instance;

    public bool IsDayRunning => isDayRunning;
    public float CurrentTimePercent => Mathf.Clamp01(timePassed / (dayDurationMinutes * 60f));

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
        float percent = CurrentTimePercent;
        float adjusted = Mathf.Clamp01(percent * 0.75f + 0.25f);

        UpdateLighting(adjusted);

        if (directionalLight.intensity < 0.3f)
            SetIndoorLights(true);

        UpdateClockUI(percent);

        if (percent >= 1f)
            EndDay();
    }

    void UpdateLighting(float percent)
    {
        float sunRotation = Mathf.Lerp(0f, 180f, percent);
        directionalLight.transform.rotation = Quaternion.Euler(sunRotation - 90f, 170f, 0f);
        directionalLight.color = lightColor.Evaluate(percent);
        directionalLight.intensity = lightIntensity.Evaluate(percent);
    }

    void SetIndoorLights(bool on)
    {
        foreach (Light light in indoorLights)
            if (light != null) light.enabled = on;
    }

    void UpdateClockUI(float percent)
    {
        float hour = Mathf.Lerp(startHour, endHour, percent);
        TimeSpan time = TimeSpan.FromHours(hour);
        if (timeDisplay != null)
            timeDisplay.text = time.ToString(@"hh\:mm");
    }

    void EndDay()
    {
        isDayRunning = false;
        SetIndoorLights(false);

        var allCustomers = FindObjectsByType<CustomerAI>(FindObjectsSortMode.None);
        foreach (var c in allCustomers)
            c.StandUpAndLeave();

        UIManager.Instance?.ShowDayOverPrompt();
        UIManager.Instance?.ShowDayCompleteMessage(currentDay);

        Debug.Log($"Day {currentDay} ended.");
        currentDay++;
    }

    public void OpenCafe() => IsCafeOpen = true;
    public void UnlockCafe() => IsCafeOpen = true;
    public void UnlockSecondFloor() => IsSecondFloorUnlocked = true;
    public void ReceiveLoanFromUncleBobby() => HasLoan = true;
}