using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public GameObject pcPrefab; // Assign your PC prefab here
    public Transform spawnPoint; // Where the PC spawns near the upgrade station
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        // For mobile, use touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector2 touchPos = Input.GetTouch(0).position;
            HandleTouch(touchPos);
        }

#if UNITY_EDITOR
        // For testing in editor with mouse
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition);
        }
#endif
    }

    private void HandleTouch(Vector2 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("UpgradeStation"))
            {
                SpawnPC();
            }
        }
    }

    private void SpawnPC()
    {
        if (pcPrefab != null && spawnPoint != null)
        {
            Instantiate(pcPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("PC Prefab or Spawn Point is not assigned!");
        }
    }
}