using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private const string AXIS_SPELL = "Spell";
    private const float AXIS_MIN = 0.3f;

    public AudioMixer audioMixer;
    public Slider sfxVolumeSlider;
    public Text sfxVolumeText;
    public Slider bgmVolumeSlider;
    public Text bgmVolumeText;
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private AudioSource m_audioSource;
    private Resolution[] resolutions;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        LoadSetting();

        //audioMixer.GetFloat("SFXVolume", out float volume);
        //sfxVolumeSlider.value = Mathf.Pow(10f, (volume / 20f));
        //sfxVolumeText.text = $"{sfxVolumeSlider.value * 100f:F0}%";
        sfxVolumeSlider.GetComponent<SFXVolumeTester>().StopCoroutine("TestSFX");

        //audioMixer.GetFloat("BGMVolume", out volume);
        //bgmVolumeSlider.value = Mathf.Pow(10f, (volume / 20f));
        //bgmVolumeText.text = $"{bgmVolumeSlider.value * 100f:F0}%";

        InitializeResolution();
        fullscreenToggle.isOn = Screen.fullScreen;

        this.gameObject.SetActive(false);
    }

    void Update()
    {
        InputBack();
    }

    private void InitializeResolution()
    {
        resolutions = Screen.resolutions.Distinct().Where(r => r.width >= 1024).ToArray();  // remove duplicates and filter by width
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " × " + resolutions[i].height + " (" + resolutions[i].refreshRate + "Hz)";
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                currentResolutionIndex = i;
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetSFXVolume(float value)
    {
        Debug.Log("SFX" + value);

        float volume = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("SFXVolume", volume);
        sfxVolumeText.text = $"{value * 100f:F0}%";
    }

    public void SetBGMVolume(float value)
    {
        Debug.Log("BGM" + value);

        float volume = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("BGMVolume", volume);
        bgmVolumeText.text = $"{value * 100f:F0}%";
    }

    public void SetResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        m_audioSource.Play();
    }

    public void SetFullscreen(bool isChecked)
    {
        Screen.fullScreen = isChecked;
        m_audioSource.Play();
    }

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs reset");
        m_audioSource.Play();
    }

    public void InputBack()
    {
        float back = Input.GetAxis(AXIS_SPELL);

        if (back > AXIS_MIN)
        {
            transform.GetChild(transform.childCount-1).GetComponent<Button>().onClick.Invoke();
        }
    }

    public void SaveSetting()
    {
        Debug.Log("Setting Saved...!");

        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolumeSlider.value);
    }
    public void LoadSetting()
    {
        Debug.Log("Setting Loaded...!");

        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume");

        SetSFXVolume(sfxVolumeSlider.value);
        SetBGMVolume(bgmVolumeSlider.value);
    }
}
