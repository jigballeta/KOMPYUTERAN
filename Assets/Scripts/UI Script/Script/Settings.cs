using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public Toggle musicToggle;
    public Slider soundSlider;
    public Button helpButton;
    public GameObject helpPanel;

    public AudioSource musicSource;
    public AudioMixer audioMixer;

    void Start()
    {
        // Load music setting
        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        if (musicToggle != null)
            musicToggle.isOn = true;
        else
            Debug.LogWarning("Music Toggle not assigned in Inspector!");
        musicSource.mute = !isMusicOn;

        // Load sound volume
        float savedVolume = PlayerPrefs.GetFloat("SoundVolume", 0.75f);
        soundSlider.value = savedVolume;
        SetSoundVolume(savedVolume);


    }

    public void OnMusicToggle(bool isOn)
    {
        musicSource.mute = !isOn;
        PlayerPrefs.SetInt("MusicOn", isOn ? 1 : 0);
    }

    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }

    public void ToggleHelpPanel()
    {
        helpPanel.SetActive(!helpPanel.activeSelf);
    }
}