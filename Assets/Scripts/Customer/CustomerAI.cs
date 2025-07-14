using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro;

public class CustomerAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform doorEntryTarget;
    public Transform doorExitTarget;
    public int rentTime;

    public GameObject speechBubble;
    public TextMeshProUGUI speechText;

    private Animator animator;
    public CustomerManager manager;

    public bool isPaid = false;
    public bool isAssignedPC = false;
    private bool hasLeft = false;

    public Transform pcTarget;

    private enum CustomerState
    {
        WalkingToDoor,
        WalkingToCashier,
        WaitingInQueue,
        GoingToPC,
        UsingPC,
        Leaving
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
                    StartCoroutine(RentTimer());
                    break;

                case CustomerState.Leaving:
                    // handled by coroutine
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

    IEnumerator RentTimer()
    {
        yield return new WaitForSeconds(rentTime * 60f); // rentTime in minutes
        StandUpAndLeave();
    }

    public void StandUpAndLeave()
    {
        if (hasLeft) return;
        hasLeft = true;

        agent.isStopped = false;
        animator.SetBool("IsTyping", false);
        animator.SetBool("Sit", false);
        animator.SetTrigger("Stand");

        StartCoroutine(LeaveAfterStanding());
    }

    IEnumerator LeaveAfterStanding()
    {
        yield return new WaitForSeconds(1f);
        agent.SetDestination(doorExitTarget.position);
        state = CustomerState.Leaving;

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.1f)
            yield return null;

        Destroy(gameObject);
    }

    public void AcceptPayment()
    {
        if (isPaid) return;

        isPaid = true;
        UIManager.Instance.AddCash(rentTime * 10);
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

