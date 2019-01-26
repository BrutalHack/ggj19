using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
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
//            Score score = MusicXmlParser.GetScore("Assets/Music/Gute_Laune/Beat.xml");
//            ProcessPart(score.Parts[0]);
            audioSource = GetComponent<AudioSource>();
            ProcessMidiXml();
            StartCoroutine(WaitAndPlayMusic());
//            OnSnare += note => Debug.Log("Snare");
        }

        private IEnumerator WaitAndPlayMusic()
        {
            yield return new WaitForSeconds(2f);
//            audioSource.time = 0f;
            musicStartTimestamp = Time.time;
            audioSource.Play();
        }

        private void Update()
        {
            double relativeMusicTimestamp = Time.time - musicStartTimestamp;
            if (notes.Count > 0)
            {
                double noteTimestamp = notes.Peek().timestamp + eventOffsetInSeconds;
                if (noteTimestamp < relativeMusicTimestamp)
                {
                    TimedNote note = notes.Dequeue();
                    FireNoteEvent(note);
                }
            }
        }

        private void ProcessMidiXml()
        {
            XmlDocument xmlDocument = new XmlDocument();
            FileStream fileStream =
                new FileStream("Assets/Music/Gute_Laune/Beat-midi.xml", FileMode.Open, FileAccess.Read);
            xmlDocument.Load(fileStream);
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
                        }
                    }
                }
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

                        notes.Enqueue(new TimedNote {type = noteType, timestamp = noteTimestamp});
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

        private static double GetNoteTimestamp(int noteTimeStampInDivisions, int quarterNoteDivisions,
            double quarterNoteDurationInSeconds)
        {
            double quarterNotePercent = (double) noteTimeStampInDivisions / quarterNoteDivisions;
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