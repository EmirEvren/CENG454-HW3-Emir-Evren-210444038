using System;
using UnityEngine;
using UnityEngine.Audio;

public class AmmoInventory : MonoBehaviour
{
    [Header("Starting Ammo")]
    [SerializeField] private int redAmmo = 0;
    [SerializeField] private int blueAmmo = 0;
    [SerializeField] private int greenAmmo = 0;
    [SerializeField] private int yellowAmmo = 0;

    [Header("Current Selection")]
    [SerializeField] private AmmoColor currentAmmoColor = AmmoColor.None;

    [Header("Selection Audio")]
    [SerializeField] private AudioClip ammoSwitchSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField, Range(0f, 1f)] private float switchSoundVolume = 1f;

    private AudioSource audioSource;

    public AmmoColor CurrentAmmoColor => currentAmmoColor;

    public event Action<AmmoColor, int> OnAmmoChanged;
    public event Action<AmmoColor> OnCurrentColorChanged;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        if (sfxMixerGroup != null)
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
    }

    private void Start()
    {
        NotifyAllAmmo();
        OnCurrentColorChanged?.Invoke(currentAmmoColor);
    }

    public int GetAmmo(AmmoColor color)
    {
        return color switch
        {
            AmmoColor.Red => redAmmo,
            AmmoColor.Blue => blueAmmo,
            AmmoColor.Green => greenAmmo,
            AmmoColor.Yellow => yellowAmmo,
            _ => 0
        };
    }

    public void AddAmmo(AmmoColor color, int amount)
    {
        if (amount <= 0) return;

        switch (color)
        {
            case AmmoColor.Red:
                redAmmo += amount;
                break;
            case AmmoColor.Blue:
                blueAmmo += amount;
                break;
            case AmmoColor.Green:
                greenAmmo += amount;
                break;
            case AmmoColor.Yellow:
                yellowAmmo += amount;
                break;
        }

        OnAmmoChanged?.Invoke(color, GetAmmo(color));
    }

    public bool TryConsumeCurrentAmmo(int amount)
    {
        if (amount <= 0) return false;
        if (currentAmmoColor == AmmoColor.None) return false;

        int current = GetAmmo(currentAmmoColor);
        if (current < amount)
            return false;

        SetAmmo(currentAmmoColor, current - amount);
        OnAmmoChanged?.Invoke(currentAmmoColor, GetAmmo(currentAmmoColor));
        return true;
    }

    public void SetCurrentColor(AmmoColor color)
    {
        if (currentAmmoColor == color) return;

        currentAmmoColor = color;

        PlayAmmoSwitchSound();

        OnCurrentColorChanged?.Invoke(currentAmmoColor);
    }

    public void ClearSelection()
    {
        SetCurrentColor(AmmoColor.None);
    }

    public void SelectRed() => SetCurrentColor(AmmoColor.Red);
    public void SelectYellow() => SetCurrentColor(AmmoColor.Yellow);
    public void SelectGreen() => SetCurrentColor(AmmoColor.Green);
    public void SelectBlue() => SetCurrentColor(AmmoColor.Blue);

    private void SetAmmo(AmmoColor color, int value)
    {
        value = Mathf.Max(0, value);

        switch (color)
        {
            case AmmoColor.Red:
                redAmmo = value;
                break;
            case AmmoColor.Blue:
                blueAmmo = value;
                break;
            case AmmoColor.Green:
                greenAmmo = value;
                break;
            case AmmoColor.Yellow:
                yellowAmmo = value;
                break;
        }
    }

    private void NotifyAllAmmo()
    {
        OnAmmoChanged?.Invoke(AmmoColor.Red, redAmmo);
        OnAmmoChanged?.Invoke(AmmoColor.Blue, blueAmmo);
        OnAmmoChanged?.Invoke(AmmoColor.Green, greenAmmo);
        OnAmmoChanged?.Invoke(AmmoColor.Yellow, yellowAmmo);
    }

    private void PlayAmmoSwitchSound()
    {
        if (audioSource == null || ammoSwitchSound == null)
            return;

        audioSource.PlayOneShot(ammoSwitchSound, switchSoundVolume);
    }
}