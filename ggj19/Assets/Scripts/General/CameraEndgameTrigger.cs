using BrutalHack.ggj19.General.Music;
using UnityEngine;

namespace BrutalHack.ggj19
{
    public class CameraEndgameTrigger : MonoBehaviour
    {
        public MusicController musicController;

        private static readonly int End = Animator.StringToHash("end");

        // Start is called before the first frame update
        void Start()
        {
            musicController.OnSongFinished += () => GetComponent<Animator>().SetTrigger(End);
        }
    }
}