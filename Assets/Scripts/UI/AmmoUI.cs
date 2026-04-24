using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private AmmoInventory ammoInventory;

    [Header("Texts")]
    [SerializeField] private TMP_Text redText;
    [SerializeField] private TMP_Text yellowText;
    [SerializeField] private TMP_Text greenText;
    [SerializeField] private TMP_Text blueText;

    private void OnEnable()
    {
        if (ammoInventory == null) return;

        ammoInventory.OnAmmoChanged += HandleAmmoChanged;
        ammoInventory.OnCurrentColorChanged += HandleCurrentColorChanged;
    }

    private void OnDisable()
    {
        if (ammoInventory == null) return;

        ammoInventory.OnAmmoChanged -= HandleAmmoChanged;
        ammoInventory.OnCurrentColorChanged -= HandleCurrentColorChanged;
    }

    private void Start()
    {
        RefreshAll();
    }

    private void HandleAmmoChanged(AmmoColor color, int amount)
    {
        RefreshAll();
    }

    private void HandleCurrentColorChanged(AmmoColor color)
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        if (ammoInventory == null) return;

        UpdateLine(redText, "Red", AmmoColor.Red);
        UpdateLine(yellowText, "Yellow", AmmoColor.Yellow);
        UpdateLine(greenText, "Green", AmmoColor.Green);
        UpdateLine(blueText, "Blue", AmmoColor.Blue);
    }

    private void UpdateLine(TMP_Text textField, string label, AmmoColor color)
    {
        if (textField == null) return;

        bool isSelected = ammoInventory.CurrentAmmoColor == color;
        string prefix = isSelected ? "> " : "  ";

        textField.text = $"{prefix}{label}: {ammoInventory.GetAmmo(color)}";
    }
}