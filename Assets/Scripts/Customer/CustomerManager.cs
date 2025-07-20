using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Transform[] queuePositions;
    public Transform[] pcStations;

    private Queue<CustomerAI> waitingQueue = new Queue<CustomerAI>();
    private HashSet<Transform> assignedPCs = new HashSet<Transform>();

    void Start()
    {
        foreach (Transform pc in pcStations)
        {
            assignedPCs.Add(pc);
        }
    }

    public Transform GetNextQueueSpot()
    {
        return waitingQueue.Count < queuePositions.Length ? queuePositions[waitingQueue.Count] : null;
    }

    public bool IsQueueFull() => waitingQueue.Count >= queuePositions.Length;

    public void EnqueueCustomer(CustomerAI customer)
    {
        if (!DayNightCycle.Instance.IsDayRunning)
        {
            Debug.Log("Customer arrived but day is not running. Destroying...");
            Destroy(customer.gameObject);
            return;
        }

        if (IsQueueFull())
        {
            Debug.Log("Queue is full. Destroying customer.");
            Destroy(customer.gameObject);
            return;
        }

        waitingQueue.Enqueue(customer);
        UpdateQueuePositions();
    }

    public void DequeueCustomer(CustomerAI customer)
    {
        if (waitingQueue.Count > 0 && waitingQueue.Peek() == customer)
        {
            waitingQueue.Dequeue();
            UpdateQueuePositions();
        }
    }

    private void UpdateQueuePositions()
    {
        CustomerAI[] customers = waitingQueue.ToArray();
        for (int i = 0; i < customers.Length && i < queuePositions.Length; i++)
        {
            customers[i].agent.SetDestination(queuePositions[i].position);
        }
    }

    public bool IsFirstInQueue(CustomerAI customer)
    {
        return waitingQueue.Count > 0 && waitingQueue.Peek() == customer;
    }

    public void AssignPCToCustomer(CustomerAI customer)
    {
        if (!DayNightCycle.Instance.IsDayRunning)
        {
            Debug.Log("Attempted to assign PC outside daytime.");
            return;
        }

        List<Transform> availablePCs = new List<Transform>();
        foreach (Transform pc in assignedPCs)
        {
            if (!IsPCInUse(pc))
            {
                availablePCs.Add(pc);
            }
        }

        if (availablePCs.Count > 0)
        {
            Transform selectedPC = availablePCs[Random.Range(0, availablePCs.Count)];
            customer.GoToPC(selectedPC);
            DequeueCustomer(customer);
        }
        else
        {
            Debug.LogWarning("No available PC to assign.");
        }
    }

    public void FreePC(Transform pc)
    {
        // Optional: You could track which are "in use" separately if needed.
    }

    public Transform GetLookTargetForPC(Transform pc)
    {
        return pc.Find("Monitor") ?? pc;
    }

    private bool IsPCInUse(Transform pc)
    {
        foreach (CustomerAI customer in FindObjectsByType<CustomerAI>(FindObjectsSortMode.None))
        {
            if (customer != null && customer.pcTarget == pc)
                return true;
        }
        return false;
    }

    public void RegisterNewPC(Transform pc)
    {
        if (!assignedPCs.Contains(pc))
        {
            assignedPCs.Add(pc);
            Debug.Log("Registered new PC station: " + pc.name);
        }
    }
}
