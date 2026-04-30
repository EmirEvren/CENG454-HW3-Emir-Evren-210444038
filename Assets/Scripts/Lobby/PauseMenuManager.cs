using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private int normalSortingOrder = 0;
    [SerializeField] private int pauseSortingOrder = 9999;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseSettingsPanel;

    [Header("References")]
    [SerializeField] private MainMenuUIManager mainMenuUIManager;

    private bool isPaused;
    private bool canPause;

    private void Start()
    {
        ForceClosePauseMenu();
        SetMenuCanvasOrder(false);
    }

    private void Update()
    {
        if (!canPause)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void EnablePause()
    {
        canPause = true;
    }

    public void DisablePause()
    {
        canPause = false;
        ForceClosePauseMenu();
        SetMenuCanvasOrder(false);
    }

    public void PauseGame()
    {
        if (!canPause)
            return;

        isPaused = true;

        SetMenuCanvasOrder(true);

        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            pausePanel.transform.SetAsLastSibling();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearSelectedUI();

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);

        SetMenuCanvasOrder(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ClearSelectedUI();

        Time.timeScale = 1f;
    }

    public void OnSettingsClicked()
    {
        if (!isPaused)
            return;

        SetMenuCanvasOrder(true);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseSettingsPanel != null)
        {
            pauseSettingsPanel.SetActive(true);
            pauseSettingsPanel.transform.SetAsLastSibling();
        }

        ClearSelectedUI();
    }

    public void OnBackFromSettingsClicked()
    {
        if (!isPaused)
            return;

        SetMenuCanvasOrder(true);

        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            pausePanel.transform.SetAsLastSibling();
        }

        ClearSelectedUI();
    }

    public void OnBackToMainMenuClicked()
    {
        Time.timeScale = 1f;

        isPaused = false;
        canPause = false;

        ForceClosePauseMenu();
        SetMenuCanvasOrder(false);
        ClearSelectedUI();

        if (mainMenuUIManager != null)
            mainMenuUIManager.ReturnToMainMenuAndResetGame();
    }

    public void OnQuitClicked()
    {
        Time.timeScale = 1f;

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ForceClosePauseMenu()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);
    }

    private void SetMenuCanvasOrder(bool pauseOrder)
    {
        if (menuCanvas == null)
            return;

        menuCanvas.overrideSorting = true;
        menuCanvas.sortingOrder = pauseOrder ? pauseSortingOrder : normalSortingOrder;
    }

    private void ClearSelectedUI()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}