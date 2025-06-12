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

    public float dayDurationMinutes = 12f; // 12 minutes = 1 day
    public TextMeshProUGUI timeDisplay;
    public TextMeshProUGUI countdownDisplay;

    private float timePassed = 0f;
    private bool isDayRunning = false;

    public bool hasLoanFromUncleBobby = false;
    public GameObject lockedDoorUI; // assign a "Locked - Talk to Uncle Bobby" UI prompt
    public GameObject[] doors; // doors to lock/unlock


    [SerializeField] private MonitorController[] monitors;

    public Action OnDayEnd;

    private float startHour = 8f; // 8:00 AM
    private float endHour = 20f;  // 8:00 PM

    public static bool HasLoan = false;
    public static bool IsCafeOpen = false;

    public bool isCafeUnlocked = false;



    public void StartDay()
    {
        timePassed = 0f;
        isDayRunning = true;
        SetIndoorLights(false);

        UpdateLighting(0f); // Set to initial morning lighting
    }

    void Update()
    {
        if (!isDayRunning) return;

        timePassed += Time.deltaTime;
        float timePercent = timePassed / (dayDurationMinutes * 60f);

        float adjustedPercent = Mathf.Clamp01(timePercent * 0.75f + 0.25f); // Shift day to start from 8AM
        UpdateLighting(adjustedPercent);

        if (directionalLight.intensity < 0.3f)
        {
            SetIndoorLights(true);
        }

        if (timePercent >= 1f)
        {
            isDayRunning = false;
            SetIndoorLights(false);
            OnDayEnd?.Invoke();
        }

        UpdateClockUI(timePercent);
    }

    void UpdateLighting(float percent)
    {
        float sunRotation = Mathf.Lerp(0f, 180f, percent); // 0 = morning, 180 = evening
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunRotation - 90f, 170f, 0f));
        directionalLight.color = lightColor.Evaluate(percent);
        directionalLight.intensity = lightIntensity.Evaluate(percent);
    }

    void SetIndoorLights(bool isOn)
    {
        foreach (Light light in indoorLights)
        {
            light.enabled = isOn;
        }
    }

    void UpdateClockUI(float percent)
    {
        float currentHour = Mathf.Lerp(startHour, endHour, percent);
        TimeSpan time = TimeSpan.FromHours(currentHour);
        timeDisplay.text = time.ToString(@"hh\:mm");

        float remainingTime = (1f - percent) * dayDurationMinutes;
        countdownDisplay.text = $"{remainingTime:F1} mins left";
    }


   

    public void OpenCafe()
    {
        IsCafeOpen = true;
        Debug.Log("Cafe is now officially open!");
    }

    public bool IsCafeUnlocked()
    {
        return isCafeUnlocked;
    }

    public void UnlockCafe()
    {
        isCafeUnlocked = true;
        Debug.Log("Cafe is now unlocked!");
    }



}


