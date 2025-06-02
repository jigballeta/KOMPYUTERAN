using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro;

public class CustomerAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform doorEntryTarget;
    public int rentTime;

    public GameObject speechBubble;
    public TextMeshProUGUI speechText;

    private Animator animator;
    public CustomerManager manager;

    public bool isPaid = false;
    public bool isAssignedPC = false;

    public Transform pcTarget; // Store assigned PC

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
        manager = FindAnyObjectByType<CustomerManager>();

        rentTime = Random.Range(1, 4);
        state = CustomerState.WalkingToDoor;

        if (speechBubble != null)
            speechBubble.SetActive(false);

        agent.SetDestination(doorEntryTarget.position);
    }

    void Update()
    {
        if (agent != null && animator != null)
        {
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
                    if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    {
                        SitAtPC(); // Now only sit when fully arrived
                        state = CustomerState.UsingPC;
                    }
                    break;

            }
        }


        if (state == CustomerState.WaitingInQueue && manager.IsFirstInQueue(this) && !isPaid)
        {
            ShowRentSpeech();
        }

        if (isPaid && !isAssignedPC && state == CustomerState.WaitingInQueue)
        {
            isAssignedPC = true;
            HideSpeechBubble();
            manager.AssignPCToCustomer(this);
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

    public void GoToPC(Transform pcTransform)
    {
        if (pcTransform != null)
        {
            pcTarget = pcTransform;
            agent.SetDestination(pcTarget.position);
            state = CustomerState.GoingToPC;
        }
    }


    public void SitAtPC()
    {
        // Face monitor first
        Transform lookTarget = manager.GetLookTargetForPC(pcTarget);
        if (lookTarget != null)
        {
            Vector3 lookDirection = lookTarget.position - transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        // Then sit and type
        animator.SetBool("Sit", true);
        animator.SetBool("IsTyping", true);

        Debug.Log("Customer is sitting and typing.");
    }


    public void AcceptPayment()
    {
        if (isPaid) return;

        isPaid = true;
        Debug.Log($"Customer paid for {rentTime} hour(s).");
        UIManager.Instance.AddCash(rentTime * 10); // Update cash UI
    }

    public void WaitAndProceed()
    {
        StartCoroutine(WaitForDoorThenMove());
    }

    IEnumerator WaitForDoorThenMove()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;
        agent.SetDestination(doorEntryTarget.position);
    }

    private void ShowRentSpeech()
    {
        if (speechBubble != null && speechText != null && !speechBubble.activeSelf)
        {
            speechText.text = $"I want {rentTime} hour{(rentTime > 1 ? "s" : "")}!";
            speechBubble.SetActive(true);
        }
    }

    private void HideSpeechBubble()
    {
        if (speechBubble != null)
            speechBubble.SetActive(false);
    }
}



