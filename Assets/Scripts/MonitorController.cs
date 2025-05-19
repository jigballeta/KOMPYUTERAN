using UnityEngine;
using UnityEngine.Video;

public class MonitorController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer.targetTexture != null)
            videoPlayer.targetTexture.Release(); // show black at start

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void PowerOn()
    {
        Debug.Log("🎥 PowerOn() called on: " + gameObject.name);

        if (!videoPlayer.isPrepared)
        {
            Debug.Log("Preparing video...");
            videoPlayer.Prepare();
        }
        else
        {
            Debug.Log("Already prepared — playing...");
            videoPlayer.Play();
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("✅ Video prepared, now playing...");
        vp.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("🛑 Video finished on " + gameObject.name);
        if (vp.targetTexture != null)
            vp.targetTexture.Release();
    }
}
