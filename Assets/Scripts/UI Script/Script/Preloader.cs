using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{ 
    private CanvasGroup fadegroup;
    private float loadtime;
    private float minimumLogoTime = 3.0f; // Minimum time of that scene

    private void Start()
    {
        // Grab the only CanvasGroup in the scene
        fadegroup = FindAnyObjectByType<CanvasGroup>();

        // Start with a white screen;
        fadegroup.alpha = 1;

        // Pre load the game
        // $$
            // Get the timestamp of the completion time 
        // if load time is super, give it a small buffer time
        if (Time.time < minimumLogoTime)
            loadtime = minimumLogoTime;
        else 
            loadtime = Time.time;
    }

    private void Update()
    {
        // Fade in
        if (Time.time < minimumLogoTime)
        {
            fadegroup.alpha = 1 - Time.time;
        }

        // Fade out
        if (Time.time > minimumLogoTime && loadtime != 0)
        {
            fadegroup.alpha = Time.time - minimumLogoTime;
            if (fadegroup.alpha >= 1)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}