using UnityEngine;
using TMPro;
using System.Collections;

public class MainPowerSwitch : Interactable
{
    [SerializeField] private MonitorController[] monitors;
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private CustomerSpawner customerSpawner;
    [SerializeField] private TextMeshProUGUI dayStartText;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float resetDelay = 5f;

    private bool dayStarted = false;

    protected override void Interact()
    {
        if (dayStarted) return;

        dayStarted = true;
        Debug.Log("Switch Interacted: Starting Day");

        foreach (MonitorController monitor in monitors)
            monitor.PowerOn();

        dayNightCycle?.StartDay();

        if (customerSpawner != null)
        {
            customerSpawner.StopSpawning();  // Stop in case it was still running
            customerSpawner.StartSpawning(); // Always start fresh
        }

        if (dayStartText != null)
        {
            int dayNum = dayNightCycle != null ? dayNightCycle.currentDay : 1;
            StartCoroutine(FadeDayStartMessage($"Day {dayNum} Started!"));
        }

        StartCoroutine(WaitForDayEnd());
    }

    private void Start()
    {
        promptMessage = "Flip Main Power Switch";
        if (dayStartText != null)
            dayStartText.alpha = 0;
    }

    private IEnumerator FadeDayStartMessage(string message)
    {
        dayStartText.text = message;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            dayStartText.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        dayStartText.alpha = 1;

        yield return new WaitForSeconds(displayTime);

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            dayStartText.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        dayStartText.alpha = 0;
    }

    private IEnumerator WaitForDayEnd()
    {
        while (dayNightCycle != null && dayNightCycle.IsDayRunning)
            yield return null;

        Debug.Log("Day has ended. Preparing for next day...");

        yield return new WaitForSeconds(resetDelay);

        foreach (MonitorController monitor in monitors)
            monitor.PowerOff();

        dayStarted = false;

        if (dayStartText != null)
        {
            int nextDay = dayNightCycle != null ? dayNightCycle.currentDay : 2;
            StartCoroutine(FadeDayStartMessage($"Press switch to start Day {nextDay}"));
        }

        if (customerSpawner != null)
            customerSpawner.StopSpawning();
    }
}