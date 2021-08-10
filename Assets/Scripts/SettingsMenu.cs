using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Audio;
using TMPro;
using System.Linq;
using Cinemachine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] TMP_Dropdown resolutionsDropdown;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensiXSlider;
    [SerializeField] Slider sensiYSlider;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] CinemachineFreeLook TPPcam;
    [SerializeField] CinemachineFreeLook robotCam;
    [SerializeField] CinemachineVirtualCamera FPPcam;

    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolutionsDropdown.ClearOptions();

        int currResIndex = 0;
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            //if (resolutions[i].Equals(Screen.currentResolution))
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currResIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(options);

        resolutionsDropdown.value = PlayerPrefs.GetInt("resolution", currResIndex);
        resolutionsDropdown.RefreshShownValue();

        qualityDropdown.value = PlayerPrefs.GetInt("quality", 3);
        qualityDropdown.RefreshShownValue();

        volumeSlider.value = PlayerPrefs.GetFloat("mvol", -5);
        sensiXSlider.value = PlayerPrefs.GetFloat("xSensi", 5);
        sensiYSlider.value = PlayerPrefs.GetFloat("ySensi", 0.02f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;

        LoadSavedSettings();

        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolution", resolutionIndex);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("mvol", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("quality", qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        if (isFullscreen)
            PlayerPrefs.SetInt("fullscreen", 1);
        else
            PlayerPrefs.SetInt("fullscreen", 0);
    }

    public void SetYSensitivity(float sensi)
    {
        if (FPPcam && TPPcam && robotCam)
        {
            TPPcam.m_YAxis.m_MaxSpeed = sensi;
            robotCam.m_YAxis.m_MaxSpeed = sensi;
            FPPcam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = sensi * 250f;
        }
    }

    public void SetXSensitivity(float sensi)
    {
        if (FPPcam && TPPcam && robotCam)
        {
            TPPcam.m_XAxis.m_MaxSpeed = sensi;
            robotCam.m_XAxis.m_MaxSpeed = sensi;
            FPPcam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = sensi;
        }
    }

    void LoadSavedSettings()
    {
        SetResolution(resolutionsDropdown.value);
        SetQuality(qualityDropdown.value);
        SetFullscreen(PlayerPrefs.GetInt("fullscreen", 1) == 1);
        SetVolume(PlayerPrefs.GetFloat("mvol", -5));
        SetXSensitivity(PlayerPrefs.GetFloat("xSensi", 4));
        SetYSensitivity(PlayerPrefs.GetFloat("ySensi", 0.02f));
    }
}
