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
        if (waitingQueue.Count < queuePositions.Length)
            return queuePositions[waitingQueue.Count];
        return null;
    }

    public bool IsQueueFull()
    {
        return waitingQueue.Count >= queuePositions.Length;
    }

    public void EnqueueCustomer(CustomerAI customer)
    {
        if (IsQueueFull())
        {
            Debug.Log("Queue is full. Customer not added.");
            Destroy(customer.gameObject); // Or handle as needed
            return;
        }

        waitingQueue.Enqueue(customer);
        UpdateQueuePositions();
    }

    private void UpdateQueuePositions()
    {
        CustomerAI[] customers = waitingQueue.ToArray();
        for (int i = 0; i < customers.Length && i < queuePositions.Length; i++)
        {
            customers[i].agent.SetDestination(queuePositions[i].position);
        }
    }

    public void AssignPCToCustomer(CustomerAI customer)
    {
        if (availablePCs.Count == 0) return;

        Transform pc = availablePCs[0];
        availablePCs.RemoveAt(0);
        customer.GoToPC(pc);
        customer.SitAtPC();
    }

    public void ReleasePC(Transform pc)
    {
        if (!availablePCs.Contains(pc))
        {
            availablePCs.Add(pc);
        }
    }
}

