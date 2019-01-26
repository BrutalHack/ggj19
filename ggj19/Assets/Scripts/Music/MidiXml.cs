
using System;
using System.Xml.Serialization;

namespace BrutalHack.ggj19.Music
{
    [Serializable()]
    [XmlRoot("MIDIFile")]
    public class MidiXml
    {
        [XmlElement("Track")]
        public Track midiTrack;
    }

    public class Track
    {
        
        [XmlArray("Track")]
        [XmlArrayItem("Event")]
        public Event midiEvent;
    }

    public class Event
    {
        [XmlElement("Absolute")]
        [XmlElement("NoteOn")]
        public int absoluteTimestamp;
    }
}