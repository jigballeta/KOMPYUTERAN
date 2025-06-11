using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    private CanvasGroup fadegroup;
    private float fadeinspeed = 0.33f;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Toggles (Optional)")]
    public Toggle musicToggle;
    public Toggle soundToggle;

    private void Start()
    {
        fadegroup = FindAnyObjectByType<CanvasGroup>();
        fadegroup.alpha = 1;

        ShowMainMenu();

        // Load previous toggle states (optional)
        if (musicToggle != null)
            musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        if (soundToggle != null)
            soundToggle.isOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
    }

    private void Update()
    {
        fadegroup.alpha = 1 - Time.timeSinceLevelLoad * fadeinspeed;
    }

    // Called by Play button
    public void OnPlayClick()
    {
        Debug.Log("Play button clicked!");
        SceneManager.LoadScene("SampleScene"); // Change to your actual scene name
    }

    // Called by Settings button
    public void OnSettingsClick()
    {
        Debug.Log("Settings button clicked");
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Called by Back button in settings
    public void OnBackClick()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Called by Quit button
    public void OnQuitClick()
    {
        Debug.Log("Quit button clicked");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Called when music toggle is changed
    public void OnMusicToggle(bool isOn)
    {
        Debug.Log("Music Toggle: " + isOn);
        PlayerPrefs.SetInt("MusicEnabled", isOn ? 1 : 0);
        // TODO: Add your music on/off logic here
    }

    // Called when sound toggle is changed
    public void OnSoundToggle(bool isOn)
    {
        Debug.Log("Sound Toggle: " + isOn);
        PlayerPrefs.SetInt("SoundEnabled", isOn ? 1 : 0);
        // TODO: Add your sound on/off logic here
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
}