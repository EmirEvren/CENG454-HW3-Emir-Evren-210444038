using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameObject gameSceneRoot;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Optional Camera References")]
    [SerializeField] private Camera menuCamera;
    [SerializeField] private Camera gameplayCamera;

    private void Start()
    {
        if (gameSceneRoot != null)
            gameSceneRoot.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        RefreshLobbyCamera();
        RefreshGameplayCamera();

        ShowLobbyCamera();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f;
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