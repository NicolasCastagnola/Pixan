using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadSceneAfterVideoPlayerFinished : MonoBehaviour
{
   [SerializeField] private VideoPlayer _videoPlayer;
   private void Awake() => _videoPlayer.loopPointReached += OnVideoFinished;
   private static void OnVideoFinished(VideoPlayer vp)
   {
      vp.loopPointReached -= OnVideoFinished;

      SceneManager.LoadScene("_Preload");
   }

   private void Update() {
      if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0)) VideoFinished(_videoPlayer);
   }

   private static void VideoFinished(VideoPlayer vp)
   {
      vp.frame = (long)(vp.frameCount - 5);
   }
}
