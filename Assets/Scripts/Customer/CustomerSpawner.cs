using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public CustomerManager customerManager;
    public Transform doorEntryTarget;

    private bool canSpawn = false;

    public void StartSpawning()
    {
        canSpawn = true;
        InvokeRepeating(nameof(SpawnCustomer), 2f, spawnInterval);
    }

    void SpawnCustomer()
    {
        if (!canSpawn || customerManager == null || customerManager.IsQueueFull())
        {
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        GameObject newCustomer = Instantiate(customerPrefab, spawnPoints[index].position, spawnPoints[index].rotation);

        CustomerAI ai = newCustomer.GetComponent<CustomerAI>();
        if (ai != null)
        {
            ai.doorEntryTarget = doorEntryTarget;
        }
    }
}


