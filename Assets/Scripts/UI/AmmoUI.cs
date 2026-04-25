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

    [Header("Selector")]
    [SerializeField] private RectTransform selectorArrow;
    [SerializeField] private float selectorYOffset = -25f;

    [Header("Style")]
    [SerializeField] private float selectedFontSize = 34f;
    [SerializeField] private float normalFontSize = 28f;

    [SerializeField] private Color selectedRedColor = new Color(1f, 0.25f, 0.25f);
    [SerializeField] private Color selectedYellowColor = new Color(1f, 0.9f, 0.2f);
    [SerializeField] private Color selectedGreenColor = new Color(0.25f, 1f, 0.35f);
    [SerializeField] private Color selectedBlueColor = new Color(0.3f, 0.6f, 1f);

    [SerializeField] private Color unselectedColor = new Color(0.8f, 0.8f, 0.8f);

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

        UpdateLine(redText, "RED", AmmoColor.Red);
        UpdateLine(yellowText, "YELLOW", AmmoColor.Yellow);
        UpdateLine(greenText, "GREEN", AmmoColor.Green);
        UpdateLine(blueText, "BLUE", AmmoColor.Blue);

        MoveSelector();
    }

    private void UpdateLine(TMP_Text textField, string label, AmmoColor color)
    {
        if (textField == null) return;

        bool isSelected = ammoInventory.CurrentAmmoColor == color;
        int ammoCount = ammoInventory.GetAmmo(color);

        textField.text = $"{label}: {ammoCount}";

        if (isSelected)
        {
            textField.fontSize = selectedFontSize;
            textField.color = GetSelectedColor(color);
            textField.fontStyle = FontStyles.Bold;
        }
        else
        {
            textField.fontSize = normalFontSize;
            textField.color = unselectedColor;
            textField.fontStyle = FontStyles.Normal;
        }
    }

    private void MoveSelector()
    {
        if (selectorArrow == null) return;

        TMP_Text selectedText = GetSelectedText();
        if (selectedText == null) return;

        RectTransform selectedRect = selectedText.rectTransform;

        selectorArrow.anchoredPosition = new Vector2(
            selectedRect.anchoredPosition.x,
            selectedRect.anchoredPosition.y + selectorYOffset
        );

        TMP_Text arrowText = selectorArrow.GetComponent<TMP_Text>();
        if (arrowText != null)
        {
            arrowText.color = GetSelectedColor(ammoInventory.CurrentAmmoColor);
        }
    }

    private TMP_Text GetSelectedText()
    {
        switch (ammoInventory.CurrentAmmoColor)
        {
            case AmmoColor.Red:
                return redText;
            case AmmoColor.Yellow:
                return yellowText;
            case AmmoColor.Green:
                return greenText;
            case AmmoColor.Blue:
                return blueText;
            default:
                return redText;
        }
    }

    private Color GetSelectedColor(AmmoColor color)
    {
        switch (color)
        {
            case AmmoColor.Red:
                return selectedRedColor;
            case AmmoColor.Yellow:
                return selectedYellowColor;
            case AmmoColor.Green:
                return selectedGreenColor;
            case AmmoColor.Blue:
                return selectedBlueColor;
            default:
                return Color.white;
        }
    }
}