using UnityEngine;

public class GameLoseHandler : MonoBehaviour
{
    [SerializeField] private ChestHealth chestHealth;
    [SerializeField] private GameObject losePanel;

    private void OnEnable()
    {
        if (chestHealth != null)
        {
            chestHealth.OnChestDestroyed += HandleLose;
        }
    }

    private void OnDisable()
    {
        if (chestHealth != null)
        {
            chestHealth.OnChestDestroyed -= HandleLose;
        }
    }

    private void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);
    }

    private void HandleLose()
    {
        Debug.Log("LOSE STATE TRIGGERED");

        if (losePanel != null)
            losePanel.SetActive(true);

        Time.timeScale = 0f;
    }
}