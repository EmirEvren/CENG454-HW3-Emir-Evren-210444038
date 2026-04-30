using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class GameWorldMusicPlayer : MonoBehaviour
{
    [Header("Playlist")]
    [SerializeField] private AudioClip[] musicPlaylist;
    [SerializeField] private bool playOnEnable = true;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    private AudioSource audioSource;
    private Coroutine playlistRoutine;
    private int currentMusicIndex;
    private bool isPaused;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        if (musicMixerGroup != null)
            audioSource.outputAudioMixerGroup = musicMixerGroup;
    }

    private void OnEnable()
    {
        if (playOnEnable)
            StartPlaylist();
    }

    private void OnDisable()
    {
        StopPlaylist();
    }

    public void StartPlaylist()
    {
        if (musicPlaylist == null || musicPlaylist.Length == 0)
        {
            Debug.LogWarning("GameWorldMusicPlayer: Music playlist is empty.");
            return;
        }

        if (playlistRoutine != null)
            StopCoroutine(playlistRoutine);

        isPaused = false;
        playlistRoutine = StartCoroutine(PlaylistRoutine());
    }

    public void StopPlaylist()
    {
        if (playlistRoutine != null)
        {
            StopCoroutine(playlistRoutine);
            playlistRoutine = null;
        }

        if (audioSource != null)
            audioSource.Stop();

        currentMusicIndex = 0;
        isPaused = false;
    }

    public void PauseMusic()
    {
        if (audioSource == null)
            return;

        isPaused = true;
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        if (audioSource == null)
            return;

        isPaused = false;
        audioSource.UnPause();
    }

    private IEnumerator PlaylistRoutine()
    {
        while (true)
        {
            if (musicPlaylist == null || musicPlaylist.Length == 0)
                yield break;

            AudioClip selectedClip = musicPlaylist[currentMusicIndex];

            if (selectedClip == null)
            {
                GoToNextMusic();
                yield return null;
                continue;
            }

            audioSource.clip = selectedClip;
            audioSource.Play();

            while (audioSource.isPlaying || isPaused)
            {
                yield return null;
            }

            GoToNextMusic();
        }
    }

    private void GoToNextMusic()
    {
        currentMusicIndex++;

        if (currentMusicIndex >= musicPlaylist.Length)
            currentMusicIndex = 0;
    }
}