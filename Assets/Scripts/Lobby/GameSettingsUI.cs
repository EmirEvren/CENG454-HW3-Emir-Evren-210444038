using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettingsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolumeParameter = "MasterVolume";
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";

    private readonly List<Resolution> availableResolutions = new List<Resolution>();

    private const string ResolutionWidthKey = "ResolutionWidth";
    private const string ResolutionHeightKey = "ResolutionHeight";
    private const string DisplayModeKey = "DisplayMode";
    private const string MasterVolumeKey = "MasterVolumeValue";
    private const string MusicVolumeKey = "MusicVolumeValue";
    private const string SfxVolumeKey = "SfxVolumeValue";

    private void Start()
    {
        SetupResolutionDropdown();
        SetupDisplayModeDropdown();
        LoadSettings();
    }

    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null)
            return;

        resolutionDropdown.ClearOptions();
        availableResolutions.Clear();

        List<string> options = new List<string>();
        HashSet<string> seen = new HashSet<string>();

        Resolution[] resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string key = resolutions[i].width + "x" + resolutions[i].height;
            if (seen.Contains(key))
                continue;

            seen.Add(key);
            availableResolutions.Add(resolutions[i]);
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
        }

        resolutionDropdown.AddOptions(options);
    }

    private void SetupDisplayModeDropdown()
    {
        if (displayModeDropdown == null)
            return;

        displayModeDropdown.ClearOptions();
        displayModeDropdown.AddOptions(new List<string>
        {
            "Fullscreen",
            "Borderless",
            "Windowed"
        });
    }

    private void LoadSettings()
    {
        int savedWidth = PlayerPrefs.GetInt(ResolutionWidthKey, Screen.width);
        int savedHeight = PlayerPrefs.GetInt(ResolutionHeightKey, Screen.height);
        int savedDisplayMode = PlayerPrefs.GetInt(DisplayModeKey, GetDisplayModeIndex(Screen.fullScreenMode));

        float savedMaster = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float savedMusic = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float savedSfx = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

        int resolutionIndex = GetResolutionIndex(savedWidth, savedHeight);
        if (resolutionDropdown != null && resolutionIndex >= 0)
            resolutionDropdown.SetValueWithoutNotify(resolutionIndex);

        if (displayModeDropdown != null)
            displayModeDropdown.SetValueWithoutNotify(savedDisplayMode);

        if (masterSlider != null)
            masterSlider.SetValueWithoutNotify(savedMaster);

        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(savedMusic);

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(savedSfx);

        ApplyResolution(resolutionIndex >= 0 ? resolutionIndex : 0, savedDisplayMode);
        ApplyMasterVolume(savedMaster);
        ApplyMusicVolume(savedMusic);
        ApplySfxVolume(savedSfx);
    }

    public void OnResolutionChanged(int index)
    {
        int displayModeIndex = displayModeDropdown != null ? displayModeDropdown.value : 0;
        ApplyResolution(index, displayModeIndex);
        SaveResolution(index);
    }

    public void OnDisplayModeChanged(int index)
    {
        int resolutionIndex = resolutionDropdown != null ? resolutionDropdown.value : 0;
        ApplyResolution(resolutionIndex, index);
        PlayerPrefs.SetInt(DisplayModeKey, index);
        PlayerPrefs.Save();
    }

    public void OnMasterVolumeChanged(float value)
    {
        ApplyMasterVolume(value);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();
    }

    public void OnMusicVolumeChanged(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();
    }

    public void OnSfxVolumeChanged(float value)
    {
        ApplySfxVolume(value);
        PlayerPrefs.SetFloat(SfxVolumeKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyResolution(int resolutionIndex, int displayModeIndex)
    {
        if (availableResolutions.Count == 0)
            return;

        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, availableResolutions.Count - 1);
        Resolution selectedResolution = availableResolutions[resolutionIndex];

        FullScreenMode mode = GetFullScreenMode(displayModeIndex);

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, mode);
    }

    private void SaveResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= availableResolutions.Count)
            return;

        Resolution selectedResolution = availableResolutions[resolutionIndex];

        PlayerPrefs.SetInt(ResolutionWidthKey, selectedResolution.width);
        PlayerPrefs.SetInt(ResolutionHeightKey, selectedResolution.height);
        PlayerPrefs.Save();
    }

    private void ApplyMasterVolume(float value)
    {
        SetMixerVolume(masterVolumeParameter, value);
    }

    private void ApplyMusicVolume(float value)
    {
        SetMixerVolume(musicVolumeParameter, value);
    }

    private void ApplySfxVolume(float value)
    {
        SetMixerVolume(sfxVolumeParameter, value);
    }

    private void SetMixerVolume(string parameterName, float value)
    {
        if (audioMixer == null)
            return;

        float clampedValue = Mathf.Clamp(value, 0.0001f, 1f);
        float dbValue = Mathf.Log10(clampedValue) * 20f;

        if (value <= 0.0001f)
            dbValue = -80f;

        audioMixer.SetFloat(parameterName, dbValue);
    }

    private int GetResolutionIndex(int width, int height)
    {
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            if (availableResolutions[i].width == width && availableResolutions[i].height == height)
                return i;
        }

        return 0;
    }

    private FullScreenMode GetFullScreenMode(int index)
    {
        switch (index)
        {
            case 0:
                return FullScreenMode.ExclusiveFullScreen;
            case 1:
                return FullScreenMode.FullScreenWindow;
            case 2:
                return FullScreenMode.Windowed;
            default:
                return FullScreenMode.ExclusiveFullScreen;
        }
    }

    private int GetDisplayModeIndex(FullScreenMode mode)
    {
        switch (mode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                return 0;
            case FullScreenMode.FullScreenWindow:
                return 1;
            case FullScreenMode.Windowed:
                return 2;
            default:
                return 0;
        }
    }
}