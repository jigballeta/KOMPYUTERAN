using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject[] customerPrefabs;
    public Transform[] spawnPoints;
    public Transform doorEntryTarget;
    public Transform doorExitTarget;
    public float spawnInterval = 5f;
    public CustomerManager customerManager;

    private bool canSpawn = false;
    private float timer = 0f;

    private void OnEnable()
    {
        DayNightCycle.OnDayStarted += StartSpawning;
        DayNightCycle.OnDayEnded += StopSpawning;
    }

    private void OnDisable()
    {
        DayNightCycle.OnDayStarted -= StartSpawning;
        DayNightCycle.OnDayEnded -= StopSpawning;
    }

    public void StartSpawning()
    {
        Debug.Log($"CustomerSpawner: StartSpawning() on Day {DayNightCycle.Instance.currentDay}");
        canSpawn = true;
        timer = 0f;
    }

    public void StopSpawning()
    {
        Debug.Log("CustomerSpawner: StopSpawning()");
        canSpawn = false;
    }

    void Update()
    {
        if (!canSpawn || !DayNightCycle.Instance.IsDayRunning) return;
        if (customerManager == null || customerManager.IsQueueFull()) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnCustomer();
            timer = 0f;
        }
    }

    void SpawnCustomer()
    {
        if (customerPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("CustomerSpawner: No prefabs or spawn points.");
            return;
        }

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        int prefabIndex = Random.Range(0, customerPrefabs.Length);

        GameObject customer = Instantiate(customerPrefabs[prefabIndex], spawnPoints[spawnIndex].position, Quaternion.identity);

        var ai = customer.GetComponent<CustomerAI>();
        if (ai != null)
        {
            ai.doorEntryTarget = doorEntryTarget;
            ai.doorExitTarget = doorExitTarget;
        }

        Debug.Log("CustomerSpawner: Spawned customer");
    }
}