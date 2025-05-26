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

    [SerializeField] private MonitorController[] monitors;


    public Action OnDayEnd;

    public void StartDay()
    {
        timePassed = 0f;
        isDayRunning = true;
        SetIndoorLights(false);
    }

    void Update()
    {
        if (!isDayRunning) return;

        timePassed += Time.deltaTime;
        float timePercent = timePassed / (dayDurationMinutes * 60f);

        // Rotate light like sun
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0f));
        directionalLight.color = lightColor.Evaluate(timePercent);
        directionalLight.intensity = lightIntensity.Evaluate(timePercent);

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

    void SetIndoorLights(bool isOn)
    {
        foreach (Light light in indoorLights)
        {
            light.enabled = isOn;
        }
    }

    void UpdateClockUI(float percent)
    {
        int startHour = 8;
        int endHour = 20;
        float currentHour = Mathf.Lerp(startHour, endHour, percent);

        TimeSpan time = TimeSpan.FromHours(currentHour);
        timeDisplay.text = time.ToString(@"hh\:mm");

        float remainingTime = (1f - percent) * dayDurationMinutes;
        countdownDisplay.text = $"{remainingTime:F1} mins left";
    }


}
