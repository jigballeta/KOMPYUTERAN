using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Transform[] queuePositions;
    public Transform[] pcStations;

    private Queue<CustomerAI> waitingQueue = new Queue<CustomerAI>();
    private HashSet<Transform> assignedPCs = new HashSet<Transform>();

    public Transform GetNextQueueSpot()
    {
        return waitingQueue.Count < queuePositions.Length ? queuePositions[waitingQueue.Count] : null;
    }

    public bool IsQueueFull() => waitingQueue.Count >= queuePositions.Length;

    public void EnqueueCustomer(CustomerAI customer)
    {
        if (!DayNightCycle.Instance.IsDayRunning)
        {
            Destroy(customer.gameObject);
            return;
        }

        if (IsQueueFull())
        {
            Debug.Log("Queue is full.");
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
        if (!DayNightCycle.Instance.IsDayRunning) return;

        foreach (Transform pc in pcStations)
        {
            if (!assignedPCs.Contains(pc))
            {
                assignedPCs.Add(pc);
                customer.GoToPC(pc);
                DequeueCustomer(customer);
                return;
            }
        }

        Debug.LogWarning("No available PC to assign.");
    }

    public void FreePC(Transform pc)
    {
        if (pc != null && assignedPCs.Contains(pc))
        {
            assignedPCs.Remove(pc);
        }
    }

    public Transform GetLookTargetForPC(Transform pc)
    {
        return pc.Find("Monitor") ?? pc;
    }
}