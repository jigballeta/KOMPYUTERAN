using UnityEngine;
using UnityEngine.Video;

public class MonitorController : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private Renderer screenRenderer;

    [SerializeField] private float shutdownDelay = 2f;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        screenRenderer = GetComponent<Renderer>();

        if (videoPlayer == null || screenRenderer == null)
        {
            Debug.LogError("Missing VideoPlayer or Renderer on " + gameObject.name);
        }

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void ActivateMonitor()
    {
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Optional: delay shutdown
        Invoke(nameof(ShutdownMonitor), shutdownDelay);
    }

    private void ShutdownMonitor()
    {
        screenRenderer.material.mainTexture = null;
    }

    // Optional if you want to reset manually
    public void DeactivateMonitor()
    {
        videoPlayer.Stop();
        screenRenderer.material.mainTexture = null;
    }
}

