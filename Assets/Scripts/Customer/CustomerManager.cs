using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Transform[] queuePositions;
    public Transform[] pcStations;

    private Queue<CustomerAI> waitingQueue = new Queue<CustomerAI>();
    private List<Transform> availablePCs = new List<Transform>();

    void Start()
    {
        availablePCs.AddRange(pcStations);
    }

    public Transform GetNextQueueSpot()
    {
        return waitingQueue.Count < queuePositions.Length ? queuePositions[waitingQueue.Count] : null;
    }

    public bool IsQueueFull()
    {
        return waitingQueue.Count >= queuePositions.Length;
    }

    public void EnqueueCustomer(CustomerAI customer)
    {
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
        if (availablePCs.Count == 0) return;

        Transform pc = availablePCs[0];
        availablePCs.RemoveAt(0);

        customer.GoToPC(pc);
        DequeueCustomer(customer);
    }

    public void ReleasePC(Transform pc)
    {
        if (!availablePCs.Contains(pc))
        {
            availablePCs.Add(pc);
        }
    }

    public Transform GetLookTargetForPC(Transform pc)
    {
        return pc.Find("Monitor") ?? pc;
    }
}

