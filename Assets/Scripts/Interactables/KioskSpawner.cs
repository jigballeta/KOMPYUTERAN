using UnityEngine;

public class KioskSpawner : MonoBehaviour
{
    public GameObject pcPrefab; // The PC prefab to spawn
    public Transform[] spawnPoints; // All possible spawn points
    private int nextSpawnIndex = 0; // Track where to spawn next

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SpawnPC();
        }
    }

    private void SpawnPC()
    {
        if (nextSpawnIndex >= spawnPoints.Length)
        {
            Debug.Log("All PC spots are occupied!");
            return;
        }

        Instantiate(pcPrefab, spawnPoints[nextSpawnIndex].position, spawnPoints[nextSpawnIndex].rotation);
        nextSpawnIndex++;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Press E to upgrade/spawn PC");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
