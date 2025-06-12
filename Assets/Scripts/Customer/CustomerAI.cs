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

    public Transform pcTarget;

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
                    SitAtPC();
                    state = CustomerState.UsingPC;
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

    public void GoToPC(Transform target)
    {
        if (target != null)
        {
            pcTarget = target;
            agent.SetDestination(pcTarget.position);
            state = CustomerState.GoingToPC;
        }
    }

    public void SitAtPC()
    {
        if (animator == null || pcTarget == null) return;

        agent.isStopped = true;
        agent.ResetPath();

        Transform lookTarget = manager.GetLookTargetForPC(pcTarget);
        if (lookTarget != null)
        {
            Vector3 direction = lookTarget.position - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }

        animator.SetBool("IsMoving", false);
        animator.SetBool("Sit", true);
        animator.SetBool("IsTyping", true);
    }

    private IEnumerator HandleSitDelay()
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.05f)
            yield return null;

        yield return new WaitForSeconds(0.1f);

        Transform lookTarget = manager.GetLookTargetForPC(pcTarget);
        if (lookTarget != null)
        {
            Vector3 lookDirection = lookTarget.position - transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        animator.SetBool("Sit", true);
        animator.SetBool("IsTyping", true);

        Debug.Log("Customer is now sitting and typing.");
    }

    public void AcceptPayment()
    {
        if (isPaid) return;

        isPaid = true;
        Debug.Log($"Customer paid for {rentTime} hour(s).");
        UIManager.Instance.AddCash(rentTime * 10);
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

