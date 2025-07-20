using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;

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
    private float endPercent = -1f;

    private enum CustomerState
    {
        WalkingToDoor, WalkingToCashier, WaitingInQueue, GoingToPC, UsingPC, Leaving
    }

    private CustomerState state;

    private Transform[] secondFloorWaypoints;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        manager = FindAnyObjectByType<CustomerManager>();

        rentTime = Random.Range(1, 4);
        state = CustomerState.WalkingToDoor;

        if (speechBubble != null) speechBubble.SetActive(false);
        agent.SetDestination(doorEntryTarget.position);

        // Automatically assign stair waypoints if not set
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("SecondFloorWaypoint");
        System.Array.Sort(waypointObjects, (a, b) => a.name.CompareTo(b.name));
        secondFloorWaypoints = new Transform[waypointObjects.Length];
        for (int i = 0; i < waypointObjects.Length; i++)
            secondFloorWaypoints[i] = waypointObjects[i].transform;
    }

    void Update()
    {
        UpdateAnimation();

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
                    SetLeaveTime();
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

        if (state == CustomerState.UsingPC && !hasLeft)
        {
            if (!DayNightCycle.Instance.IsDayRunning || DayNightCycle.Instance.CurrentTimePercent >= endPercent)
            {
                StandUpAndLeave();
            }
        }
    }

    void UpdateAnimation()
    {
        if (agent && animator)
        {
            bool isMoving = agent.velocity.magnitude > 0.05f && agent.remainingDistance > agent.stoppingDistance;
            animator.SetBool("IsMoving", isMoving);
        }
    }

    void SetLeaveTime()
    {
        float total = DayNightCycle.Instance.endHour - DayNightCycle.Instance.startHour;
        float now = DayNightCycle.Instance.CurrentTimePercent;
        float rentPercent = rentTime / total;
        endPercent = Mathf.Min(1f, now + rentPercent);
    }

    public void GoToCashier()
    {
        Transform target = manager.GetNextQueueSpot();
        if (target != null)
        {
            agent.SetDestination(target.position);
            state = CustomerState.WalkingToCashier;
        }
    }

    public void GoToPC(Transform target)
    {
        pcTarget = target;

        if (IsSecondFloorPC(target))
        {
            StartCoroutine(GoThroughWaypointsToPC());
        }
        else
        {
            agent.SetDestination(target.position);
        }

        state = CustomerState.GoingToPC;
    }

    private bool IsSecondFloorPC(Transform pc)
    {
        return pc.position.y > 3f; // adjust this Y threshold based on your second floor height
    }

    IEnumerator GoThroughWaypointsToPC()
    {
        foreach (Transform waypoint in secondFloorWaypoints)
        {
            agent.SetDestination(waypoint.position);
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f);
        }

        agent.SetDestination(pcTarget.position);
    }

    public void SitAtPC()
    {
        if (animator == null || pcTarget == null) return;

        agent.isStopped = true;
        agent.ResetPath();

        Transform lookTarget = manager.GetLookTargetForPC(pcTarget);
        if (lookTarget != null)
        {
            Vector3 dir = lookTarget.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        animator.SetBool("IsMoving", false);
        animator.SetBool("Sit", true);
        animator.SetBool("IsTyping", true);
    }

    public void StandUpAndLeave()
    {
        if (hasLeft) return;
        hasLeft = true;

        manager?.FreePC(pcTarget);

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

        manager?.FreePC(pcTarget);
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
        if (speechBubble && speechText && !speechBubble.activeSelf)
        {
            speechText.text = $"I want {rentTime} hour{(rentTime > 1 ? "s" : "")}!";
            speechBubble.SetActive(true);
        }
    }

    private void HideSpeechBubble()
    {
        if (speechBubble) speechBubble.SetActive(false);
    }
}
