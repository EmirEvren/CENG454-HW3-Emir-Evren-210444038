using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LobbyMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip lobbyMusic;
    [SerializeField] private bool playOnStart = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;

        if (lobbyMusic != null)
            audioSource.clip = lobbyMusic;
    }

    private void Start()
    {
        Debug.Log($"LobbyMusic Start | Clip: {audioSource.clip} | Volume: {audioSource.volume} | Mute: {audioSource.mute} | Listener Count: {FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length}");

        if (playOnStart)
            PlayLobbyMusic();
    }

    public void PlayLobbyMusic()
    {
        if (audioSource.clip == null)
        {
            Debug.LogWarning("Lobby music clip is missing.");
            return;
        }

        audioSource.Play();

        Debug.Log($"Lobby music started. IsPlaying: {audioSource.isPlaying}");
    }

    public void StopLobbyMusic()
    {
        audioSource.Stop();
    }
}