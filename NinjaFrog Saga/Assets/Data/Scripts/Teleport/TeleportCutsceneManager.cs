using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class TeleportCutsceneManager : MonoBehaviour
{
    public static TeleportCutsceneManager Instance;

    [Header("Vídeo UI")]
    [SerializeField] private CanvasGroup videoCanvas;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoDisplay;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void PlayVideo(VideoClip clip)
    {
        if (videoPlayer == null || videoCanvas == null) return;

        videoCanvas.alpha = 1f;
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    public void StopVideo()
    {
        if (videoPlayer != null)
            videoPlayer.Stop();

        if (videoCanvas != null)
            videoCanvas.alpha = 0f;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        StopVideo();
    }
}
