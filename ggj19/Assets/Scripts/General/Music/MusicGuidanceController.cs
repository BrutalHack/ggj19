using System.Collections.Generic;
using UnityEngine;

namespace BrutalHack.ggj19.General.Music
{
    public class MusicGuidanceController : MonoBehaviour
    {
        public GameObject snarePrefab;
        public GameObject bassPrefab;
        public Transform guidanceSpawnPosition;
        public MusicController musicController;
        private List<Guidance> activeGuidance;

        private TimedNote activeNote;

        private void Start()
        {
            musicController.OnBassGuidance += BassGuidance;
            musicController.OnSnareGuidance += SnareGuidance;
            musicController.OnSnare += OnNote;
            musicController.OnBass += OnNote;
            activeGuidance = new List<Guidance>();
        }

        private void SnareGuidance(TimedNote note)
        {
            Guidance snareGuidance = Instantiate(snarePrefab).GetComponent<Guidance>();
            snareGuidance.transform.SetParent(guidanceSpawnPosition.transform, false);
            snareGuidance.OnMiss += Miss;
            activeGuidance.Add(snareGuidance);
        }

        public void OnMove()
        {
            Guidance usedGuidance = activeGuidance.Find(guidance => guidance.note == activeNote);
            if (usedGuidance != null)
            {
                usedGuidance.Hit();
            }
        }

        private void BassGuidance(TimedNote note)
        {
            Debug.Log("Bass Guide");
            Guidance bassGuidance = Instantiate(bassPrefab, guidanceSpawnPosition.transform).GetComponent<Guidance>();
            bassGuidance.OnMiss += Miss;
            activeGuidance.Add(bassGuidance);
        }

        public void OnNote(TimedNote note)
        {
            activeNote = note;
        }

        public void Miss(Guidance missedGuidance)
        {
            // NOP?
        }
    }
}