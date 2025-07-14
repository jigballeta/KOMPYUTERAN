using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject[] customerPrefabs; // If using multiple models
    public Transform[] spawnPoints;
    public Transform doorEntryTarget;
    public Transform doorExitTarget; // 🧠 ← add this in Inspector!
    public float spawnInterval = 5f;
    public CustomerManager customerManager;

    private bool canSpawn = false;

    public void StartSpawning()
    {
        canSpawn = true;
        InvokeRepeating(nameof(SpawnCustomer), 2f, spawnInterval);
    }

    void SpawnCustomer()
    {
        if (!canSpawn || customerManager == null || customerManager.IsQueueFull()) return;

        if (customerPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("CustomerSpawner: Missing prefabs or spawn points.");
            return;
        }

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        int prefabIndex = Random.Range(0, customerPrefabs.Length);

        GameObject newCustomer = Instantiate(customerPrefabs[prefabIndex], spawnPoints[spawnIndex].position, Quaternion.identity);
        CustomerAI ai = newCustomer.GetComponent<CustomerAI>();

        if (ai != null)
        {
            ai.doorEntryTarget = doorEntryTarget;
            ai.doorExitTarget = doorExitTarget;
        }
    }
}



