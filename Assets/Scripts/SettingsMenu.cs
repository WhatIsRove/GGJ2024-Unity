using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider sensitivitySlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("soundVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSoundVolume();
        }

        if (PlayerPrefs.HasKey("sensitivity"))
        {
            LoadSensitivity();
        } else
        {
            SetSensitivity();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSoundVolume()
    {
        float volume = soundSlider.value;
        mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("soundVolume", volume);
    }

    public void SetSensitivity()
    {
        float sensitivity = sensitivitySlider.value;
        FindObjectOfType<PlayerController>().mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        soundSlider.value = PlayerPrefs.GetFloat("soundVolume");
        SetMusicVolume();
        SetSoundVolume();
    }

    public void LoadSensitivity()
    {
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity");
        SetSensitivity();
    }
}
