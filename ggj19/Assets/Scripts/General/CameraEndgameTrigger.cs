using BrutalHack.ggj19.General;
using BrutalHack.ggj19.General.Music;
using UnityEngine;

namespace BrutalHack.ggj19
{
    public class CameraEndgameTrigger : MonoBehaviour
    {
        public MusicController musicController;
        public PlayerManager playerManager;

        private static readonly int End = Animator.StringToHash("end");
        public AudioClip outroClip;

        // Start is called before the first frame update
        void Start()
        {
//            musicController.OnSongFinished += EndGameCameraAndVoice;
            playerManager.OnEndGame += EndGameCameraAndVoice;
        }

        private void EndGameCameraAndVoice()
        {
            GetComponent<Animator>().SetTrigger(End);
            AudioSource audioSource = musicController.GetComponent<AudioSource>();
            audioSource.clip = outroClip;
            audioSource.Play();
        }
    }
}