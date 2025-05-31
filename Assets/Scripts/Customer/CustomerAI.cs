using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class CustomerAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform doorEntryTarget;
    public int rentTime;

    private Animator animator;

    private CustomerManager manager;

    private enum CustomerState
    {
        WalkingToDoor,
        WalkingToCashier,
        WaitingInQueue,
        GoingToPC,
        UsingPC
    }

    private CustomerState state;

    void Start()
    {
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        manager = FindObjectOfType<CustomerManager>();

        rentTime = Random.Range(1, 4); // Random hours
        state = CustomerState.WalkingToDoor;

        agent.SetDestination(doorEntryTarget.position); // First go to the door
    }

    void Update()
    {
        if (agent != null && animator != null)
        {
            // Use threshold to determine true movement
            bool isMoving = agent.velocity.magnitude > 0.05f && agent.remainingDistance > agent.stoppingDistance;
            animator.SetBool("IsMoving", isMoving);
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            switch (state)
            {
                case CustomerState.WalkingToDoor:
                    GoToCashier();
                    break;

                case CustomerState.WalkingToCashier:
                    manager.EnqueueCustomer(this);
                    state = CustomerState.WaitingInQueue;
                    break;

                case CustomerState.GoingToPC:
                    state = CustomerState.UsingPC;
                    break;
            }
        }
    }



    public void GoToCashier()
    {
        Transform queueTarget = manager.GetNextQueueSpot();
        if (queueTarget != null)
        {
            agent.SetDestination(queueTarget.position);
            state = CustomerState.WalkingToCashier;
        }
    }

    public void GoToPC(Transform pcTarget)
    {
        if (pcTarget != null)
        {
            agent.SetDestination(pcTarget.position);
            state = CustomerState.GoingToPC;
        }
    }

    public void SitAtPC()
    {
        // TODO: Play sit animation, start usage timer, etc.
        Debug.Log("Customer is sitting at PC.");
    }

    public void WaitAndProceed()
    {
        StartCoroutine(WaitForDoorThenMove());
    }

    IEnumerator WaitForDoorThenMove()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(1f); // Wait for door to open
        agent.isStopped = false;
        agent.SetDestination(doorEntryTarget.position); // Resume movement
    }


}

