using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using BrutalHack.ggj19.Music;
using UnityEngine;
using Time = UnityEngine.Time;

namespace BrutalHack.ggj19.General.Music
{
    public class MusicController : MonoBehaviour
    {
        public double tempo = 90d;
        private AudioSource audioSource;

        public delegate void OnNote(TimedNote note);

        public delegate void OnTime();

        public event OnNote OnSnare;
        public event OnNote OnSnareGuidance;
        public event OnNote OnBass;
        public event OnNote OnBassGuidance;
        public event OnTime OnSongFinished;

        public float eventOffsetInSeconds = -0.1f;

        public Queue<TimedNote> notes = new Queue<TimedNote>();
        public Queue<TimedNote> noteGuidance = new Queue<TimedNote>();

        private float musicStartTimestamp;
        private bool musicIsPlaying;
        private bool musicIsFinished;

        public bool autoplay;
        public float autoplayDelay = 3f;
        public double guidanceOffset = -1f;

        private void Start()
        {
//            Score score = MusicXmlParser.GetScore("Assets/Music/Gute_Laune/Beat.xml");
//            ProcessPart(score.Parts[0]);
            audioSource = GetComponent<AudioSource>();
            ProcessMidiXml();
            if (autoplay)
            {
                StartCoroutine(WaitAndPlayMusic());
            }

//            OnSnare += note => Debug.Log("Snare");
        }

        private IEnumerator WaitAndPlayMusic()
        {
            yield return new WaitForSeconds(autoplayDelay);
//            audioSource.time = 0f;
            PlayMusic();
        }

        public void PlayMusic()
        {
            musicStartTimestamp = Time.time;
            audioSource.Play();
            musicIsPlaying = true;
        }

        private void Update()
        {
            UpdateNotes();
            UpdateNoteGuidance();
        }

        private void UpdateNotes()
        {
            if (!musicIsPlaying || musicIsFinished)
            {
                return;
            }

            double relativeMusicTimestamp = Time.time - musicStartTimestamp;

            if (notes.Count > 0)
            {
                double noteTimestamp = notes.Peek().timestamp + eventOffsetInSeconds;
                if (!(noteTimestamp < relativeMusicTimestamp)) return;
                TimedNote note = notes.Dequeue();
                FireNoteEvent(note);

                if (notes.Count == 0)
                {
                    musicIsFinished = true;
                    OnSongFinished?.Invoke();
                }
            }
        }

        private void UpdateNoteGuidance()
        {
            if (!musicIsPlaying || musicIsFinished)
            {
                return;
            }

            double relativeMusicTimestamp = Time.time - musicStartTimestamp;

            if (noteGuidance.Count > 0)
            {
                double noteTimestamp = noteGuidance.Peek().timestamp;
                if (!(noteTimestamp < relativeMusicTimestamp)) return;
                TimedNote note = noteGuidance.Dequeue();
                FireNoteGuidanceEvent(note);
            }
        }

        private void ProcessMidiXml()
        {
            XmlDocument xmlDocument = new XmlDocument();
            TextAsset text = Resources.Load<TextAsset>("Beat-midi");
            xmlDocument.LoadXml(text.text);
//            FileStream fileStream =
//                new FileStream("Assets/Music/Gute_Laune/Beat-midi.xml", FileMode.Open, FileAccess.Read);
//            xmlDocument.Load(fileStream);

            XmlNode ticksPerBeatElement = xmlDocument.SelectSingleNode("//MIDIFile/TicksPerBeat");
            int ticksPerBeat = int.Parse(ticksPerBeatElement.InnerText);
            Debug.Log("ticks per Beat: " + ticksPerBeat);
            XmlNode firstTrack = xmlDocument.SelectSingleNode("//MIDIFile/Track");

            // the duration of a quarter note in seconds
            double quarterNoteDurationInSeconds = 60d / tempo;

            if (firstTrack != null)
            {
                foreach (XmlElement eventNode in firstTrack)
                {
                    if (eventNode["NoteOn"] != null)
                    {
                        if (eventNode["Absolute"] != null)
                        {
                            //Timestamps in MIDI are milliseconds as Integer
                            int durationInTicks = int.Parse(eventNode["Absolute"].InnerText);
                            double timeStamp = GetNoteDurationInSeconds(durationInTicks, ticksPerBeat,
                                quarterNoteDurationInSeconds);
                            NoteType noteType =
                                ProcessMidiNoteType(int.Parse(eventNode["NoteOn"].Attributes["Note"].InnerText));
                            notes.Enqueue(new TimedNote {type = noteType, timestamp = timeStamp});
                            noteGuidance.Enqueue(
                                new TimedNote {type = noteType, timestamp = timeStamp - guidanceOffset});
                        }
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

        private static double GetNoteTimestamp(int noteTimeStampInDivisions, int quarterNoteDivisions,
            double quarterNoteDurationInSeconds)
        {
            double quarterNotePercent = (double) noteTimeStampInDivisions / quarterNoteDivisions;
            return quarterNotePercent * quarterNoteDurationInSeconds;
        }

        private static NoteType ProcessMidiNoteType(int midiSignal)
        {
            NoteType type = NoteType.Rest;
            if (midiSignal == 48)
            {
                type = NoteType.Bass;
            }
            else if (midiSignal == 50)
            {
                type = NoteType.Snare;
            }
            else
            {
                throw new InvalidOperationException("Midi signal " + midiSignal + " is not supported.");
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
                    break;
                case NoteType.Snare:
                    OnSnare?.Invoke(note);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void FireNoteGuidanceEvent(TimedNote note)
        {
            switch (note.type)
            {
                case NoteType.Bass:
                    OnBassGuidance?.Invoke(note);
                    break;
                case NoteType.Rest:
                    break;
                case NoteType.Snare:
                    OnSnareGuidance?.Invoke(note);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}