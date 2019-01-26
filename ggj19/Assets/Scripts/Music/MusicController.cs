using System;
using System.Collections;
using System.Collections.Generic;
using MusicXml;
using MusicXml.Domain;
using UnityEngine;
using Time = UnityEngine.Time;

namespace BrutalHack.ggj19.Music
{
    public class MusicController : MonoBehaviour
    {
        public double tempo = 90d;
        private AudioSource audioSource;

        public delegate void OnNote(TimedNote note);

        public event OnNote OnSnare;
        public event OnNote OnBass;
        public event OnNote OnRest;
        
        public float eventOffsetInSeconds = -0.1f;

        public Queue<TimedNote> notes = new Queue<TimedNote>();
        private float musicStartTimestamp;

        private void Start()
        {
            Score score = MusicXmlParser.GetScore("Assets/Music/Beat.xml");
            audioSource = GetComponent<AudioSource>();
            ProcessPart(score.Parts[0]);
            musicStartTimestamp = Time.time;
            audioSource.Play();
//            OnSnare += note => Debug.Log("Snare");
        }

        private void Update()
        {
            double relativeMusicTimestamp =  Time.time - musicStartTimestamp;
            if (notes.Peek().timestamp < (relativeMusicTimestamp - eventOffsetInSeconds))
            {
                TimedNote note = notes.Dequeue();
                FireNoteEvent(note);
            }
        }

        private void ProcessPart(Part musicTrack)
        {
            Debug.Log("Processing Music Track: " + musicTrack.Name);
            // QuarterNoteDivisions is only present at change or in first measurement 
            int quarterNoteDivisions = 0;
            double noteTimestamp = 0f;

            foreach (Measure measure in musicTrack.Measures)
            {
                Debug.Log("Measure");
                //The number of divisions, in which a quarter note is split up
                if (measure.Attributes != null && measure.Attributes.Divisions != 0)
                {
                    quarterNoteDivisions = measure.Attributes.Divisions;
                }

                // the duration of a quarter note in seconds
                double quarterNoteDurationInSeconds = 60d / tempo;

                foreach (MeasureElement measureElement in measure.MeasureElements)
                {
                    if (measureElement.Element is Note note)
                    {
//                        Debug.Log("Note");
                        NoteType noteType = ProcessNoteType(note);
                        double noteDurationInSeconds = GetNoteDurationInSeconds(note.Duration, quarterNoteDivisions,
                            quarterNoteDurationInSeconds);
                        
                        notes.Enqueue(new TimedNote{type = noteType, timestamp = noteTimestamp});
                        noteTimestamp += noteDurationInSeconds;
                    }
                }
            }
        }

        private static double GetNoteDurationInSeconds(int noteDurationInDivisions, int quarterNoteDivisions,
            double quarterNoteDurationInSeconds)
        {
            double quarterNotePercent = (double) noteDurationInDivisions / quarterNoteDivisions;
//            Debug.Log("Percent of a quarter: " + quarterNotePercent * 100 + "%");
            return quarterNotePercent * quarterNoteDurationInSeconds;
        }

        private static NoteType ProcessNoteType(Note note)
        {
            NoteType type = NoteType.Rest;
            if (note.Pitch != null)
            {
                switch (note.Pitch.Step)
                {
                    case 'C':
                        type = NoteType.Bass;
                        break;
                    case 'D':
                        type = NoteType.Snare;
                        break;
                }
            }

            return type;
        }

        private void FireNoteEvent(TimedNote note)
        {
            switch (note.type)
            {
                case NoteType.Bass:
                    OnBass?.Invoke(note);
                    break;
                case NoteType.Rest:
                    OnRest?.Invoke(note);
                    break;
                case NoteType.Snare:
                    OnSnare?.Invoke(note);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}