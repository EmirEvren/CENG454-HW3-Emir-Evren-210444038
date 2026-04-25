using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private AmmoInventory ammoInventory;

    [Header("Crosshairs")]
    [SerializeField] private GameObject defaultCrosshair;
    [SerializeField] private GameObject redCrosshair;
    [SerializeField] private GameObject yellowCrosshair;
    [SerializeField] private GameObject greenCrosshair;
    [SerializeField] private GameObject blueCrosshair;

    private void OnEnable()
    {
        if (ammoInventory != null)
            ammoInventory.OnCurrentColorChanged += UpdateCrosshair;
    }

    private void OnDisable()
    {
        if (ammoInventory != null)
            ammoInventory.OnCurrentColorChanged -= UpdateCrosshair;
    }

    private void Start()
    {
        if (ammoInventory != null)
            UpdateCrosshair(ammoInventory.CurrentAmmoColor);
    }

    private void UpdateCrosshair(AmmoColor color)
    {
        if (defaultCrosshair != null) defaultCrosshair.SetActive(false);
        if (redCrosshair != null) redCrosshair.SetActive(false);
        if (yellowCrosshair != null) yellowCrosshair.SetActive(false);
        if (greenCrosshair != null) greenCrosshair.SetActive(false);
        if (blueCrosshair != null) blueCrosshair.SetActive(false);

        switch (color)
        {
            case AmmoColor.Red:
                if (redCrosshair != null) redCrosshair.SetActive(true);
                break;

            case AmmoColor.Yellow:
                if (yellowCrosshair != null) yellowCrosshair.SetActive(true);
                break;

            case AmmoColor.Green:
                if (greenCrosshair != null) greenCrosshair.SetActive(true);
                break;

            case AmmoColor.Blue:
                if (blueCrosshair != null) blueCrosshair.SetActive(true);
                break;

            default:
                if (defaultCrosshair != null) defaultCrosshair.SetActive(true);
                break;
        }
    }
}