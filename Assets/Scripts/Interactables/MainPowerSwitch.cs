using UnityEngine;

public class MainPowerSwitch : Interactable
{
    [SerializeField] private MonitorController[] monitors;

    protected override void Interact()
    {
        Debug.Log("Switch Interacted");
        foreach (MonitorController monitor in monitors)
        {
            monitor.PowerOn();
        }
    }

    private void Start()
    {
        promptMessage = "Flip Main Power Switch";
    }
}
