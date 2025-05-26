using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
{
    private CanvasGroup fadegroup;
    private float fadeinspeed = 0.33f;

    private void Start()
    {
        // Grab the only CanvasGroup in the scene
        fadegroup = FindAnyObjectByType<CanvasGroup>();

        // Start with a white screen;
        fadegroup.alpha = 1;
    }

    private void Update()
    {
        // Fade in
        fadegroup.alpha = 1 - Time.timeSinceLevelLoad * fadeinspeed;
    }

    // Buttons
    public void OnPlayClick()
    {
        Debug.Log("Play button has been clicked!");

        // Load the gameplay scene (replace "GameScene" with your actual scene name)
        SceneManager.LoadScene("SampleScene");
    }
}