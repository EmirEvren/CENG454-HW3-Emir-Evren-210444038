using UnityEngine;

public class PlayerScope : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;

    [Header("Zoom Settings")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float scopedFOV = 35f;
    [SerializeField] private float zoomSpeed = 12f;

    [Header("Optional UI")]
    [SerializeField] private GameObject normalCrosshairRoot;
    [SerializeField] private GameObject scopeCrosshair;

    public bool IsScoping { get; private set; }

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        if (mainCamera != null)
            mainCamera.fieldOfView = normalFOV;

        if (normalCrosshairRoot != null)
            normalCrosshairRoot.SetActive(true);

        if (scopeCrosshair != null)
            scopeCrosshair.SetActive(false);
    }

    private void Update()
    {
        HandleInput();
        UpdateZoom();
        UpdateUI();
    }

    private void HandleInput()
    {
        IsScoping = Input.GetMouseButton(1);
    }

    private void UpdateZoom()
    {
        if (mainCamera == null) return;

        float targetFOV = IsScoping ? scopedFOV : normalFOV;
        mainCamera.fieldOfView = Mathf.Lerp(
            mainCamera.fieldOfView,
            targetFOV,
            zoomSpeed * Time.deltaTime
        );
    }

    private void UpdateUI()
    {
        if (normalCrosshairRoot != null)
            normalCrosshairRoot.SetActive(!IsScoping);

        if (scopeCrosshair != null)
            scopeCrosshair.SetActive(IsScoping);
    }
}