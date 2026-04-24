using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private AmmoColor ammoColor;
    [SerializeField] private int ammoAmount = 20;

    private void OnTriggerEnter(Collider other)
    {
        AmmoInventory inventory = other.GetComponent<AmmoInventory>();
        if (inventory == null) return;

        inventory.AddAmmo(ammoColor, ammoAmount);
        Destroy(gameObject);
    }
}