using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameObject gameSceneRoot;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Managers")]
    [SerializeField] private PauseMenuManager pauseMenuManager;
    [SerializeField] private LobbyMusicPlayer lobbyMusicPlayer;

    [Header("Optional Camera References")]
    [SerializeField] private Camera menuCamera;
    [SerializeField] private Camera gameplayCamera;

    private void Start()
    {
        ReturnToMainMenu();
    }

    public void OnPlayClicked()
    {
        if (gameSceneRoot != null)
            gameSceneRoot.SetActive(true);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        RefreshLobbyCamera();
        RefreshGameplayCamera();

        ShowPlayerCamera();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        if (pauseMenuManager != null)
            pauseMenuManager.EnablePause();

        if (lobbyMusicPlayer != null)
            lobbyMusicPlayer.StopLobbyMusic();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        if (gameSceneRoot != null)
            gameSceneRoot.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (pauseMenuManager != null)
            pauseMenuManager.DisablePause();

        RefreshLobbyCamera();
        RefreshGameplayCamera();

        ShowLobbyCamera();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (lobbyMusicPlayer != null)
            lobbyMusicPlayer.PlayLobbyMusic();
    }

    public void ReturnToMainMenuAndResetGame()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OnSettingsClicked()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void OnBackFromSettingsClicked()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    public void OnQuitClicked()
    {
        Time.timeScale = 1f;

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void RefreshLobbyCamera()
    {
        if (menuCamera == null)
        {
            GameObject lobbyCamObj = GameObject.FindGameObjectWithTag("LobbyCamera");
            if (lobbyCamObj != null)
                menuCamera = lobbyCamObj.GetComponent<Camera>();
        }
    }

    private void RefreshGameplayCamera()
    {
        if (gameplayCamera == null)
        {
            GameObject playerCamObj = GameObject.FindGameObjectWithTag("MainCamera");
            if (playerCamObj != null)
                gameplayCamera = playerCamObj.GetComponent<Camera>();
        }

        if (gameplayCamera == null && Camera.main != null)
            gameplayCamera = Camera.main;
    }

    private void ShowLobbyCamera()
    {
        if (menuCamera != null)
            menuCamera.gameObject.SetActive(true);

        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(false);
    }

    private void ShowPlayerCamera()
    {
        if (menuCamera != null)
            menuCamera.gameObject.SetActive(false);

        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(true);
    }
}