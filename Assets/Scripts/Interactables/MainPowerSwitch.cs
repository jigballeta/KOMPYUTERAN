using System.Threading;

protected override void Interact()
{
    powerOn = !powerOn;
    foreach (MonitorController monitor in monitors)
    {
        if (powerOn)
            monitor.ActivateMonitor();
        else
            monitor.DeactivateMonitor();
    }
}


