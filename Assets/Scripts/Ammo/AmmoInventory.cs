using System;
using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    [Header("Starting Ammo")]
    [SerializeField] private int redAmmo = 0;
    [SerializeField] private int blueAmmo = 0;
    [SerializeField] private int greenAmmo = 0;
    [SerializeField] private int yellowAmmo = 0;

    [Header("Current Selection")]
    [SerializeField] private AmmoColor currentAmmoColor = AmmoColor.Red;

    public AmmoColor CurrentAmmoColor => currentAmmoColor;

    public event Action<AmmoColor, int> OnAmmoChanged;
    public event Action<AmmoColor> OnCurrentColorChanged;

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

        int current = GetAmmo(currentAmmoColor);
        if (current < amount)
            return false;

        SetAmmo(currentAmmoColor, current - amount);
        OnAmmoChanged?.Invoke(currentAmmoColor, GetAmmo(currentAmmoColor));
        return true;
    }

    public void SetCurrentColor(AmmoColor color)
    {
        currentAmmoColor = color;
        OnCurrentColorChanged?.Invoke(currentAmmoColor);
    }

    public void SelectRed() => SetCurrentColor(AmmoColor.Red);
    public void SelectBlue() => SetCurrentColor(AmmoColor.Blue);
    public void SelectGreen() => SetCurrentColor(AmmoColor.Green);
    public void SelectYellow() => SetCurrentColor(AmmoColor.Yellow);

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
}