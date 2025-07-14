using UnityEngine;
using TMPro;

public class UpgradeSystem : MonoBehaviour
{
    public GameObject pcPrefab;                     // Assign your PC prefab
    public Transform spawnPoint;                    // Where to spawn the PC
    public TextMeshProUGUI upgradePrompt;           // UI text prompt reference

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        if (upgradePrompt != null)
            upgradePrompt.gameObject.SetActive(false); // Hide prompt on start
    }

    private void Update()
    {
        ShowPromptIfLookingAtUpgradeStation();

        // Mobile touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector2 touchPos = Input.GetTouch(0).position;
            HandleTouch(touchPos);
        }

#if UNITY_EDITOR
        // Editor mouse input
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition);
        }
#endif
    }

    private void ShowPromptIfLookingAtUpgradeStation()
    {
        if (mainCam == null || upgradePrompt == null) return;

#if UNITY_EDITOR
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
#else
        // Use first finger on mobile to check raycast for UI prompt
        if (Input.touchCount == 0)
        {
            upgradePrompt.gameObject.SetActive(false);
            return;
        }
        Ray ray = mainCam.ScreenPointToRay(Input.GetTouch(0).position);
#endif

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("UpgradeStation"))
            {
                upgradePrompt.gameObject.SetActive(true);
                return;
            }
        }

        upgradePrompt.gameObject.SetActive(false);
    }

    private void HandleTouch(Vector2 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
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