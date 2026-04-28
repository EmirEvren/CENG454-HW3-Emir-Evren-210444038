using UnityEngine;

public class CameraSwitchManager : MonoBehaviour
{
    [Header("Auto Find By Tag")]
    [SerializeField] private Camera lobbyCamera;
    [SerializeField] private Camera playerCamera;

    private void Awake()
    {
        RefreshLobbyCamera();
        RefreshPlayerCamera();
    }

    private void Start()
    {
        ShowLobbyCamera();
    }

    private void RefreshLobbyCamera()
    {
        if (lobbyCamera == null)
        {
            GameObject lobbyCamObj = GameObject.FindGameObjectWithTag("LobbyCamera");
            if (lobbyCamObj != null)
                lobbyCamera = lobbyCamObj.GetComponent<Camera>();
        }
    }

    private void RefreshPlayerCamera()
    {
        if (playerCamera == null)
        {
            GameObject playerCamObj = GameObject.FindGameObjectWithTag("MainCamera");
            if (playerCamObj != null)
                playerCamera = playerCamObj.GetComponent<Camera>();
        }

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main;
    }

    public void ShowLobbyCamera()
    {
        RefreshLobbyCamera();
        RefreshPlayerCamera();

        if (lobbyCamera != null)
            lobbyCamera.gameObject.SetActive(true);

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);
    }

    public void ShowPlayerCamera()
    {
        RefreshLobbyCamera();
        RefreshPlayerCamera();

        if (lobbyCamera != null)
            lobbyCamera.gameObject.SetActive(false);

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);
    }
}