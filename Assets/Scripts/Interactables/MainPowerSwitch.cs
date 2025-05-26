using UnityEngine;

public class MainPowerSwitch : Interactable
{
    [SerializeField] private MonitorController[] monitors;
    [SerializeField] private DayNightCycle dayNightCycle; 

    protected override void Interact()
    {
        Debug.Log("Switch Interacted");

        foreach (MonitorController monitor in monitors)
        {
            monitor.PowerOn();
        }

        if (dayNightCycle != null)
        {
            dayNightCycle.StartDay(); 
        }
        else
        {
            Debug.LogWarning("DayNightCycle reference not assigned!");
        }
    }

    private void Start()
    {
        promptMessage = "Flip Main Power Switch";
    }
}