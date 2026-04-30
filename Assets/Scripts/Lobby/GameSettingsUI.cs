using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettingsUI : MonoBehaviour
{
    [Serializable]
    private class SettingsPanelReferences
    {
        [Header("Dropdowns")]
        public TMP_Dropdown resolutionDropdown;
        public TMP_Dropdown displayModeDropdown;

        [Header("Audio Sliders")]
        public Slider masterSlider;
        public Slider musicSlider;
        public Slider sfxSlider;
    }

    [Header("Main Menu Settings Panel")]
    [SerializeField] private SettingsPanelReferences mainMenuSettings;

    [Header("Pause Settings Panel")]
    [SerializeField] private SettingsPanelReferences pauseSettings;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolumeParameter = "MasterVolume";
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";

    [Header("Fallback Resolutions")]
    [SerializeField] private bool addCommonFallbackResolutions = true;

    private readonly List<Resolution> availableResolutions = new List<Resolution>();

    private const string ResolutionWidthKey = "ResolutionWidth";
    private const string ResolutionHeightKey = "ResolutionHeight";
    private const string DisplayModeKey = "DisplayMode";

    private const string MasterVolumeKey = "MasterVolumeValue";
    private const string MusicVolumeKey = "MusicVolumeValue";
    private const string SfxVolumeKey = "SfxVolumeValue";

    private bool isRefreshing;

    private void Awake()
    {
        InitializeSettings();
    }

    private void OnEnable()
    {
        RefreshAllUIFromSavedSettings();
    }

    private void OnDestroy()
    {
        RemoveListeners(mainMenuSettings);
        RemoveListeners(pauseSettings);
    }

    private void InitializeSettings()
    {
        BuildResolutionList();

        SetupPanel(mainMenuSettings);
        SetupPanel(pauseSettings);

        RemoveListeners(mainMenuSettings);
        RemoveListeners(pauseSettings);

        AddListeners(mainMenuSettings);
        AddListeners(pauseSettings);

        RefreshAllUIFromSavedSettings();
    }

    public void RefreshAllUIFromSavedSettings()
    {
        BuildResolutionList();

        SetupPanel(mainMenuSettings);
        SetupPanel(pauseSettings);

        LoadSavedSettingsAndApply();
    }

    private void BuildResolutionList()
    {
        availableResolutions.Clear();

        HashSet<string> seen = new HashSet<string>();

        Resolution[] resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i++)
        {
            AddResolution(resolutions[i].width, resolutions[i].height, seen);
        }

        AddResolution(Screen.currentResolution.width, Screen.currentResolution.height, seen);
        AddResolution(Screen.width, Screen.height, seen);

        if (addCommonFallbackResolutions)
        {
            AddResolution(1920, 1080, seen);
            AddResolution(1600, 900, seen);
            AddResolution(1366, 768, seen);
            AddResolution(1280, 720, seen);
        }

        if (availableResolutions.Count == 0)
            AddResolution(1920, 1080, seen);
    }

    private void AddResolution(int width, int height, HashSet<string> seen)
    {
        if (width <= 0 || height <= 0)
            return;

        string key = width + "x" + height;

        if (seen.Contains(key))
            return;

        seen.Add(key);

        Resolution resolution = new Resolution
        {
            width = width,
            height = height
        };

        availableResolutions.Add(resolution);
    }

    private void SetupPanel(SettingsPanelReferences panel)
    {
        if (panel == null)
            return;

        SetupResolutionDropdown(panel.resolutionDropdown);
        SetupDisplayModeDropdown(panel.displayModeDropdown);

        SetupSlider(panel.masterSlider);
        SetupSlider(panel.musicSlider);
        SetupSlider(panel.sfxSlider);
    }

    private void SetupResolutionDropdown(TMP_Dropdown dropdown)
    {
        if (dropdown == null)
            return;

        dropdown.ClearOptions();
        dropdown.options.Clear();

        List<string> options = new List<string>();

        for (int i = 0; i < availableResolutions.Count; i++)
        {
            Resolution resolution = availableResolutions[i];
            options.Add(resolution.width + " x " + resolution.height);
        }

        dropdown.AddOptions(options);

        if (dropdown.options.Count > 0)
            dropdown.SetValueWithoutNotify(0);

        dropdown.RefreshShownValue();

        if (dropdown.captionText != null && dropdown.options.Count > 0)
            dropdown.captionText.text = dropdown.options[dropdown.value].text;
    }

    private void SetupDisplayModeDropdown(TMP_Dropdown dropdown)
    {
        if (dropdown == null)
            return;

        dropdown.ClearOptions();
        dropdown.options.Clear();

        dropdown.AddOptions(new List<string>
        {
            "Fullscreen",
            "Borderless",
            "Windowed"
        });

        dropdown.SetValueWithoutNotify(0);
        dropdown.RefreshShownValue();

        if (dropdown.captionText != null && dropdown.options.Count > 0)
            dropdown.captionText.text = dropdown.options[dropdown.value].text;
    }

    private void SetupSlider(Slider slider)
    {
        if (slider == null)
            return;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
    }

    private void AddListeners(SettingsPanelReferences panel)
    {
        if (panel == null)
            return;

        if (panel.resolutionDropdown != null)
            panel.resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        if (panel.displayModeDropdown != null)
            panel.displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);

        if (panel.masterSlider != null)
            panel.masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (panel.musicSlider != null)
            panel.musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (panel.sfxSlider != null)
            panel.sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    private void RemoveListeners(SettingsPanelReferences panel)
    {
        if (panel == null)
            return;

        if (panel.resolutionDropdown != null)
            panel.resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);

        if (panel.displayModeDropdown != null)
            panel.displayModeDropdown.onValueChanged.RemoveListener(OnDisplayModeChanged);

        if (panel.masterSlider != null)
            panel.masterSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);

        if (panel.musicSlider != null)
            panel.musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);

        if (panel.sfxSlider != null)
            panel.sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
    }

    private void LoadSavedSettingsAndApply()
    {
        isRefreshing = true;

        int savedWidth = PlayerPrefs.GetInt(ResolutionWidthKey, Screen.width);
        int savedHeight = PlayerPrefs.GetInt(ResolutionHeightKey, Screen.height);
        int savedDisplayMode = PlayerPrefs.GetInt(DisplayModeKey, GetDisplayModeIndex(Screen.fullScreenMode));

        savedDisplayMode = Mathf.Clamp(savedDisplayMode, 0, 2);

        float savedMaster = Mathf.Clamp01(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
        float savedMusic = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
        float savedSfx = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxVolumeKey, 1f));

        int resolutionIndex = GetResolutionIndex(savedWidth, savedHeight);

        RefreshPanelUI(mainMenuSettings, resolutionIndex, savedDisplayMode, savedMaster, savedMusic, savedSfx);
        RefreshPanelUI(pauseSettings, resolutionIndex, savedDisplayMode, savedMaster, savedMusic, savedSfx);

        isRefreshing = false;

        ApplyResolution(resolutionIndex, savedDisplayMode);
        ApplyMasterVolume(savedMaster);
        ApplyMusicVolume(savedMusic);
        ApplySfxVolume(savedSfx);
    }

    private void RefreshPanelUI(
        SettingsPanelReferences panel,
        int resolutionIndex,
        int displayModeIndex,
        float master,
        float music,
        float sfx)
    {
        if (panel == null)
            return;

        SetDropdownValue(panel.resolutionDropdown, resolutionIndex);
        SetDropdownValue(panel.displayModeDropdown, displayModeIndex);

        if (panel.masterSlider != null)
            panel.masterSlider.SetValueWithoutNotify(master);

        if (panel.musicSlider != null)
            panel.musicSlider.SetValueWithoutNotify(music);

        if (panel.sfxSlider != null)
            panel.sfxSlider.SetValueWithoutNotify(sfx);
    }

    private void SetDropdownValue(TMP_Dropdown dropdown, int index)
    {
        if (dropdown == null)
            return;

        if (dropdown.options == null || dropdown.options.Count == 0)
            return;

        index = Mathf.Clamp(index, 0, dropdown.options.Count - 1);

        dropdown.SetValueWithoutNotify(index);
        dropdown.RefreshShownValue();

        if (dropdown.captionText != null)
            dropdown.captionText.text = dropdown.options[index].text;
    }

    public void OnResolutionChanged(int index)
    {
        if (isRefreshing)
            return;

        int displayModeIndex = GetActiveDisplayModeIndex();

        ApplyResolution(index, displayModeIndex);
        SaveResolution(index);

        PlayerPrefs.SetInt(DisplayModeKey, displayModeIndex);
        PlayerPrefs.Save();

        RefreshAllUIFromSavedSettings();
    }

    public void OnDisplayModeChanged(int index)
    {
        if (isRefreshing)
            return;

        int resolutionIndex = GetActiveResolutionIndex();

        ApplyResolution(resolutionIndex, index);
        SaveResolution(resolutionIndex);

        PlayerPrefs.SetInt(DisplayModeKey, index);
        PlayerPrefs.Save();

        RefreshAllUIFromSavedSettings();
    }

    public void OnMasterVolumeChanged(float value)
    {
        if (isRefreshing)
            return;

        value = Mathf.Clamp01(value);

        ApplyMasterVolume(value);

        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();

        RefreshAllUIFromSavedSettings();
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (isRefreshing)
            return;

        value = Mathf.Clamp01(value);

        ApplyMusicVolume(value);

        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();

        RefreshAllUIFromSavedSettings();
    }

    public void OnSfxVolumeChanged(float value)
    {
        if (isRefreshing)
            return;

        value = Mathf.Clamp01(value);

        ApplySfxVolume(value);

        PlayerPrefs.SetFloat(SfxVolumeKey, value);
        PlayerPrefs.Save();

        RefreshAllUIFromSavedSettings();
    }

    private int GetActiveResolutionIndex()
    {
        if (pauseSettings != null &&
            pauseSettings.resolutionDropdown != null &&
            pauseSettings.resolutionDropdown.gameObject.activeInHierarchy)
        {
            return pauseSettings.resolutionDropdown.value;
        }

        if (mainMenuSettings != null &&
            mainMenuSettings.resolutionDropdown != null)
        {
            return mainMenuSettings.resolutionDropdown.value;
        }

        return GetResolutionIndex(Screen.width, Screen.height);
    }

    private int GetActiveDisplayModeIndex()
    {
        if (pauseSettings != null &&
            pauseSettings.displayModeDropdown != null &&
            pauseSettings.displayModeDropdown.gameObject.activeInHierarchy)
        {
            return pauseSettings.displayModeDropdown.value;
        }

        if (mainMenuSettings != null &&
            mainMenuSettings.displayModeDropdown != null)
        {
            return mainMenuSettings.displayModeDropdown.value;
        }

        return GetDisplayModeIndex(Screen.fullScreenMode);
    }

    private void ApplyResolution(int resolutionIndex, int displayModeIndex)
    {
        if (availableResolutions.Count == 0)
            BuildResolutionList();

        if (availableResolutions.Count == 0)
            return;

        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, availableResolutions.Count - 1);
        displayModeIndex = Mathf.Clamp(displayModeIndex, 0, 2);

        Resolution selectedResolution = availableResolutions[resolutionIndex];
        FullScreenMode mode = GetFullScreenMode(displayModeIndex);

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, mode);
    }

    private void SaveResolution(int resolutionIndex)
    {
        if (availableResolutions.Count == 0)
            return;

        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, availableResolutions.Count - 1);

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
        {
            Debug.LogWarning($"{gameObject.name}: AudioMixer is not assigned.");
            return;
        }

        value = Mathf.Clamp01(value);

        float dbValue = value <= 0.0001f
            ? -80f
            : Mathf.Log10(value) * 20f;

        bool success = audioMixer.SetFloat(parameterName, dbValue);

        if (!success)
            Debug.LogWarning($"{gameObject.name}: AudioMixer parameter not found: {parameterName}");
    }

    private int GetResolutionIndex(int width, int height)
    {
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            if (availableResolutions[i].width == width &&
                availableResolutions[i].height == height)
            {
                return i;
            }
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