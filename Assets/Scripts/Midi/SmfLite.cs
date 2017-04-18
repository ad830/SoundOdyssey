//
// SmfLite.cs - A minimal toolkit for handling standard MIDI files (SMF) on Unity
//
// Copyright (C) 2013 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System.Collections.Generic;
using UnityEngine;

using Midi;

namespace SmfLite
{
    // An alias for internal use.
    using DeltaEventPairList = System.Collections.Generic.List<SmfLite.MidiTrack.DeltaEventPair>;

    //
    // MIDI event
    //
    public struct MidiEvent
    {
        #region Public members

        public byte status;
        public byte data1;
        public byte data2;

        public MidiEvent (byte status, byte data1, byte data2)
        {
            this.status = status;
            this.data1 = data1;
            this.data2 = data2;
        }

        public override string ToString ()
        {
            return "[" + status.ToString ("X") + "," + data1 + "," + data2 + "]";
        }

        #endregion
    }

    //
    // MIDI track
    //
    // Stores only one track (usually a MIDI file contains one or more tracks).
    //
    public class MidiTrack
    {
        #region Internal data structure

        // Data pair storing a delta-time value and an event.
        public struct DeltaEventPair
        {
            public int delta;
            public MidiEvent midiEvent;

            public DeltaEventPair (int delta, MidiEvent midiEvent)
            {
                this.delta = delta;
                this.midiEvent = midiEvent;
            }

            public override string ToString ()
            {
                return "(" + delta + ":" + midiEvent + ")";
            }
        }

        public struct ProgramChangePair
        {
            public int channel;
            public byte programChange;

            public ProgramChangePair(int channel, byte programChange)
            {
                this.channel = channel;
                this.programChange = programChange;
            }

            public override string ToString()
            {
                return "(" + channel + ":" + programChange + ")";
            }
        }

        public class ChannelData
        {
            public List<Instrument> programChanges;
            public bool hasNotes;
            public int noteCount;

            public ChannelData(bool _hasNotes)
            {
                this.programChanges = new List<Instrument>();
                this.hasNotes = _hasNotes;
                this.noteCount = 0;
            }

            public override string ToString()
            {
                return string.Format("[ProgramChangeCount:{0} HasNotes:{1}", programChanges.Count, hasNotes);
            }
        }

        [System.Serializable]
        public class NoteDuration
        {
            public int delta;
            public byte channel;
            public byte pitch;
            public byte velocity;
            public int ticks;
            public int resolution;
            public int dottedNumber;
            public Duration.Value value;
            public bool beingPlayed;
            public bool isNoteEvent;
            public MidiEvent midiEvent;

            //public int m_dur;
            //public int m_num;
            //public int m_den;

            public NoteDuration(int delta, byte channel, byte pitch, byte velocity)
            {
                this.delta = delta;
                this.channel = channel;
                this.pitch = pitch;
                this.velocity = velocity;

                this.beingPlayed = false;
                this.dottedNumber = 0;

                // figure out the ratio between the result of ticks / resolution
                this.ticks = 0;
                this.resolution = 24;
                this.value = Duration.Value.Whole;
                this.isNoteEvent = true;
                this.midiEvent = new MidiEvent(0x90, pitch, velocity);
            }

            // Just for non-note events
            public void SetMidiEvent(MidiEvent ev)
            {
                this.isNoteEvent = false;
                this.midiEvent = ev;
            }

            static int GCD(int a, int b)
            {
                return b == 0 ? a : GCD(b, a % b);
            }

            public void CalculateValue(int resolution)
            {
                /*
                int [] allowed_durs = {1, 2, 4, 8, 16, 32, 64, 128};

                this.m_dur = 0;
                this.m_num = 1;
                this.m_den = 1;

                var wholeNoteResolution = (resolution / 8) * 4;

                var g = GCD(this.ticks, wholeNoteResolution);

                if (g != 0)
                {
                    this.m_dur = wholeNoteResolution / g;
                    this.m_num = this.ticks / g;
                }

                bool inDur = false;
                for (int i = 0; !inDur && i < allowed_durs.Length; i++)
                {
                    if (this.m_dur == allowed_durs[i])
                    {
                        inDur = true;
                    }
                }
                if (!inDur)
                {
                    this.m_dur = 4;
                    this.m_num = this.ticks;
                    this.m_den = wholeNoteResolution / 4;
                }
                */

                //Debug.LogFormat("Dur: {0} Num: {1} Den: {2}", this.m_dur, this.m_num, this.m_den);

                /*
                for i in range (len (allowed_tuplet_clocks)):
                    if clocks == allowed_tuplet_clocks[i]:
                        return global_options.allowed_tuplets[i]

                dur = 0; num = 1; den = 1;
                g = gcd (clocks, clocks_per_1)
                if g:
                    (dur, num) = (clocks_per_1 / g, clocks / g)
                if not dur in self.allowed_durs:
                    dur = 4; num = clocks; den = clocks_per_4
                return (dur, num, den)
                */

                this.resolution = resolution;
                float ratio = (float)(this.ticks) / this.resolution;
                float power = Mathf.Log10(ratio) / Mathf.Log10(2.0f);
                int ratioPowerTwo = Mathf.RoundToInt(power);
                this.value = (Duration.Value) ratioPowerTwo;

                if (power % 1 != 0)
                {
                    // dotted notes or needs quantising
                    float nearestBaseValue = Mathf.Pow(2.0f, ratioPowerTwo);

                    while (dottedNumber < 2 && ratio != 0f)
                    {
                        ratio -= nearestBaseValue;
                        nearestBaseValue /= 2f;
                        dottedNumber++;
                    }

                    if (ratio != 0f)
                    {
                        // not quite dotted...

                    }
                }


            }
        }

        public struct TimeSignature
        {
            public byte numerator;
            public byte denominator;       // negative power of two
            public byte clocksPerClick;
            public byte noted32ndNotesPerQuarterNote;

            // 4/4 | 24 MIDI Clocks | 8 32nd notes per Quarter note

            public TimeSignature (byte numerator, byte denominator, byte clocksPerClick, byte perQuarterNote)
            {
                this.numerator = numerator;
                this.denominator = denominator;
                this.clocksPerClick = clocksPerClick;
                this.noted32ndNotesPerQuarterNote = perQuarterNote;
            }

            public bool isDefault()
            {
                return (
                    this.numerator == 4 &&
                    this.denominator == 4 &&
                    this.clocksPerClick == 24 &&
                    this.noted32ndNotesPerQuarterNote == 8
                    );
            }
        }

        #endregion

        #region Public members

        public MidiTrack ()
        {
            deltaSinceLastNoteOn = 0;
            tickResolution = 0;
            noteStarted = false;

            sequence = new List<DeltaEventPair> ();
            programChanges = new List<ProgramChangePair> ();
            noteDurations = new List<NoteDuration>();
            channelData = new ChannelData[16];
            for (int i = 0; i < channelData.Length; i++)
            {
                channelData[i] = new ChannelData(false);
            }

            timeSignature = new TimeSignature();
            textInTrack = new List<string>();
            noteCount = 0;
        }

        // Returns an enumerator which enumerates the all delta-event pairs.
        public List<DeltaEventPair>.Enumerator GetEnumerator ()
        {
            return sequence.GetEnumerator ();
        }

        public DeltaEventPair GetAtIndex (int index)
        {
            return sequence [index];
        }

        public override string ToString ()
        {
            var s = "";
            foreach (var pair in sequence)
                s += pair;
            return s;
        }

        #endregion

        #region Public properties

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public int TickResolution
        {
            get { return tickResolution; }
            set { tickResolution = value; }
        }
        public string [] TextInTrack
        {
            get { return textInTrack.ToArray(); }
        }
        public TimeSignature CurrentTimeSignature
        {
            get { return timeSignature; }
            set { timeSignature = value; }
        }
        public ChannelData[] InstrumentChannels
        {
            get { return channelData; }
        }
        #endregion

        #region Private and internal members

        int tickResolution;
        List<DeltaEventPair> sequence;
        string description;
        List<ProgramChangePair> programChanges;
        List<NoteDuration> noteDurations;
        ChannelData[] channelData;                  // will only be 16 of them
        int noteCount;
        List<string> textInTrack;

        TimeSignature timeSignature;

        int deltaSinceLastNoteOn;
        bool noteStarted;

        #endregion

        #region Public methods

        public void AddText(string str)
        {
            textInTrack.Add(str);
        }

        public List<NoteDuration>.Enumerator GetDurationEnumerator()
        {
            return noteDurations.GetEnumerator();
        }

        public int GetDurationNoteCount()
        {
            return noteCount;
        }

        public void AddEvent (int delta, MidiEvent midiEvent, int trackId)
        {
            sequence.Add (new DeltaEventPair (delta, midiEvent));
            //Debug.LogFormat("{0} : {1}", delta, midiEvent.ToString());

            // increment duration of each note
            for (int i = 0; i < noteDurations.Count; ++i)
            {
                NoteDuration dur = noteDurations[i];
                if (dur.beingPlayed)
                {
                    dur.ticks += delta;
                }
            }


            //if (noteStarted)
            //{
                deltaSinceLastNoteOn += delta;
            //}

            // calculate the ticks of note durations
            int status = midiEvent.status & 0xf0;
            int channel = midiEvent.status & 0x0f;
            int velocity = midiEvent.data2;

            // add program changes
            if ((midiEvent.status & 0xf0) == 0xc0)
            {
                Instrument instrument = (Instrument)midiEvent.data1;
                if (instrument.IsValid())
                {
                    channelData[channel].programChanges.Add(instrument);
                }
            }

            if (status == 0x90 && velocity > 0)
            {
                // set that channel has at least one note
                if (!channelData[channel].hasNotes)
                {
                    channelData[channel].hasNotes = true;
                }
                
                channelData[channel].noteCount++;

                // note on
                if (!noteStarted)
                {
                    noteStarted = true;
                }
                //Debug.LogFormat("dsln {0}", deltaSinceLastNoteOn);
                NoteDuration duration = new NoteDuration(deltaSinceLastNoteOn, (byte)channel, midiEvent.data1, midiEvent.data2);
                duration.beingPlayed = true;
                noteDurations.Add(duration);

                deltaSinceLastNoteOn = 0;
                noteCount++;
                // turn on a boolean to add future delta times to this until we get a noteoff event for this pitch
            }
            else if (status == 0x80 || status == 0x90 && velocity == 0)
            {
                // note off
                // turn off the boolean and calculate tick duration / note value
                NoteDuration duration = noteDurations.FindLast(item => item.channel == channel && item.pitch == midiEvent.data1);
                duration.beingPlayed = false;
                //Debug.Log("TICKS: " + duration.ticks);

                int testResolution = tickResolution * 4;

                //Debug.Log("RESOLUTION: " + testResolution);
                duration.CalculateValue(testResolution);
                //Debug.Log("DURATION: " + duration.value);
            }
            else
            {
                // probably a non midi event
                
                NoteDuration duration = new NoteDuration(deltaSinceLastNoteOn, (byte)channel, 0, 0);
                duration.SetMidiEvent(midiEvent);
                duration.ticks = tickResolution;
                noteDurations.Add(duration);
                deltaSinceLastNoteOn = 0;
                
            }

        }

        public void AddProgramChange(int channel, byte program)
        {
            programChanges.Add(new ProgramChangePair(channel, program));
        }

        public List<ProgramChangePair>.Enumerator GetProgramChangeEnumerator()
        {
            return programChanges.GetEnumerator();
        }

        public ProgramChangePair GetProgramChangeAtIndex(int index)
        {
            return programChanges[index];
        }

        public int GetProgramChangeCount()
        {
            return programChanges.Count;
        }

        #endregion
    }

    //
    // MIDI file container
    //
    public struct MidiFileContainer
    {
        #region Public members

        // Division number == PPQN for this song.
        public int division;

        // A single BPM from the song's tracks
        public int bpm;

        // Track list contained in this file.
        public List<MidiTrack> tracks;

        public MidiTrack.ChannelData[] channelData;

        public MidiFileContainer (int division, int bpm, List<MidiTrack> tracks)
        {
            this.division = division;
            this.tracks = tracks;
            this.bpm = bpm;

            this.channelData = new MidiTrack.ChannelData[16];
            for (int i = 0; i < channelData.Length; i++)
            {
                channelData[i] = new MidiTrack.ChannelData(false);
                for (int b = 0; b < tracks.Count; b++)
                {
                    // go through each track and collect each instrument change
                    if (tracks[b].InstrumentChannels[i].programChanges.Count > 0)
                    {
                        channelData[i].programChanges.AddRange(tracks[b].InstrumentChannels[i].programChanges);
                    }
                    // and has notes status    
                    if (tracks[b].InstrumentChannels[i].hasNotes)
                    {
                        channelData[i].hasNotes = true;
                    }
                    // and note count
                    //Debug.LogFormat("Channel {0} has {1} notes", i, tracks[b].InstrumentChannels[i].noteCount);
                    channelData[i].noteCount += tracks[b].InstrumentChannels[i].noteCount;
                }
            }
        }

        public override string ToString ()
        {
            var temp = division.ToString () + ",";
            foreach (var track in tracks) {
                temp += track;
            }
            return temp;
        }

        #endregion
    }

    //
    // Sequencer for MIDI tracks
    //
    // Works like an enumerator for MIDI events.
    // Note that not only Advance() but also Start() can return MIDI events.
    //
    public class MidiTrackSequencer
    {
        #region Public members

        public bool Playing {
            get { return playing; }
        }

        // Constructor
        //   "ppqn" stands for Pulse Per Quater Note,
        //   which is usually provided with a MIDI header.
        public MidiTrackSequencer (MidiTrack track, int ppqn, float bpm)
        {
            ChangeBpm(ppqn, bpm);
            enumerator = track.GetEnumerator ();

            /*
            DeltaEventPairList.Enumerator testEnum = track.GetEnumerator();
            int i = 0;
            while (testEnum.MoveNext() && i < 20)
            {
                MidiTrack.DeltaEventPair pair = testEnum.Current;
                Debug.LogFormat("SEQUENCER EVENT {0} : {1}", i++, pair.ToString());
            }
            */
        }

        public void ChangeBpm (int ppqn, float bpm)
        {
            pulsePerSecond = bpm / 60.0f * ppqn;
        }

        // Start the sequence.
        // Returns a list of events at the beginning of the track.
        public List<MidiEvent> Start (float startTime = 0.0f)
        {
            if (enumerator.MoveNext ()) {
                pulseToNext = enumerator.Current.delta;
                playing = true;
                return Advance (startTime);
            } else {
                playing = false;
                return null;
            }
        }

        // Advance the song position.
        // Returns a list of events between the current position and the next one.
        public List<MidiEvent> Advance (float deltaTime)
        {
            if (!playing) {
                return null;
            }

            pulseCounter += pulsePerSecond * deltaTime;

            if (pulseCounter < pulseToNext) {
                return null;
            }

            var messages = new List<MidiEvent> ();

            while (pulseCounter >= pulseToNext) {
                var pair = enumerator.Current;
                messages.Add (pair.midiEvent);
                if (!enumerator.MoveNext ()) {
                    playing = false;
                    break;
                }

                pulseCounter -= pulseToNext;
                pulseToNext = enumerator.Current.delta;
            }

            return messages;
        }

        #endregion

        #region Private members

        DeltaEventPairList.Enumerator enumerator;
        bool playing;
        float pulsePerSecond;
        float pulseToNext;
        float pulseCounter;

        #endregion
    }

    public class MidiSheetSequencer
    {
        #region Public members

        public bool Playing
        {
            get { return playing; }
        }

        // Constructor
        //   "ppqn" stands for Pulse Per Quater Note,
        //   which is usually provided with a MIDI header.
        public MidiSheetSequencer(MidiTrack track, int ppqn, float bpm)
        {
            ChangeBpm(ppqn, bpm);
            enumerator = track.GetDurationEnumerator();

            /*
            Debug.LogFormat("SEQUENCING TRACK");
            var testEnum = track.GetDurationEnumerator();
            int i = 0;
            while (testEnum.MoveNext() && i < 5)
            {
                var current = testEnum.Current;
                Debug.LogFormat("Delta {0}", current.delta);
            }
            */
            //Debug.LogFormat("GET ALL CHANNEL DATA");
            //foreach (var channel in track.InstrumentChannels)
            //{
            //    if (channel.hasNotes)
            //    {
            //        Debug.Log("Track has notes");
            //        foreach (var progChange in channel.programChanges)
            //        {
            //            Debug.LogFormat("PC {0}", progChange.Name());
            //        }
            //    }
            //}
        }

        public void ChangeBpm(int ppqn, float bpm)
        {
            pulsePerSecond = bpm / 60.0f * ppqn;
        }

        // Start the sequence.
        // Returns a list of events at the beginning of the track.
        public List<MidiTrack.NoteDuration> Start(float startTime = 0.0f)
        {
            if (enumerator.MoveNext())
            {
                Debug.Log("START SEQUENCER");
                pulseToNext = enumerator.Current.delta;
                playing = true;
                return Advance(startTime);
            }
            else
            {
                playing = false;
                return null;
            }
        }

        // Advance the song position.
        // Returns a list of events between the current position and the next one.
        public List<MidiTrack.NoteDuration> Advance(float deltaTime)
        {
            if (!playing)
            {
                return null;
            }

            pulseCounter += pulsePerSecond * deltaTime;

            if (pulseCounter < pulseToNext)
            {
                return null;
            }

            var messages = new List<MidiTrack.NoteDuration>();

            while (pulseCounter >= pulseToNext)
            {
                var pair = enumerator.Current;
                messages.Add(pair);
                if (!enumerator.MoveNext())
                {
                    playing = false;
                    break;
                }

                pulseCounter -= pulseToNext;
                pulseToNext = enumerator.Current.delta;
            }

            return messages;
        }

        #endregion

        #region Private members

        List<MidiTrack.NoteDuration>.Enumerator enumerator;
        bool playing;
        float pulsePerSecond;
        float pulseToNext;
        float pulseCounter;

        #endregion
    }

    //
    // MIDI file loader
    //
    // Loads an SMF and returns a file container object.
    //
    public static class MidiFileLoader
    {
        #region Public members

        public static MidiFileContainer Load (byte[] data)
        {
            var tracks = new List<MidiTrack> ();
            var reader = new MidiDataStreamReader (data);

            // Chunk type.
            if (new string (reader.ReadChars (4)) != "MThd") {
                throw new System.FormatException ("Can't find header chunk.");
            }

            // Chunk length.
            if (reader.ReadBEInt32 () != 6) {
                throw new System.FormatException ("Length of header chunk must be 6.");
            }

            // Format (unused).
            reader.Advance (2);

            // Number of tracks.
            var trackCount = reader.ReadBEInt16 ();

            // Delta-time divisions.
            var division = reader.ReadBEInt16 ();
            if ((division & 0x8000) != 0) {
                throw new System.FormatException ("SMPTE time code is not supported.");
            }

            Debug.Log("Division: " + division);

            int bpm = 120;

            // Read the tracks.
            for (var trackIndex = 0; trackIndex < trackCount; trackIndex++) {
                Debug.Log("Reading Track " + trackIndex + " ======================");
                tracks.Add (ReadTrack (reader, ref bpm, division, trackIndex));
            }

            Debug.Log("Bpm: " + bpm);

            return new MidiFileContainer (division, bpm, tracks);
        }

        #endregion

        #region Private members

        static bool isInstrumentChange(byte ev)
        {
            return (ev >= 0xc0 && ev <= 0xcf);
        }

        static MidiTrack ReadTrack (MidiDataStreamReader reader, ref int bpm, int tickResolution, int trackId)
        {
            var track = new MidiTrack ();
            track.TickResolution = tickResolution;

            // Chunk type.
            if (new string (reader.ReadChars (4)) != "MTrk") {
                throw new System.FormatException ("Can't find track chunk.");
            }

            // Chunk length.
            var chunkEnd = reader.ReadBEInt32 ();
            chunkEnd += reader.Offset;

            // Time signature
            byte numerator = 4;
            byte denominator = 2;       // negative power of two
            byte clocksPerClick = 24;
            byte noted32ndNotesPerQuarterNote = 8;

            // Read delta-time and event pairs.
            byte ev = 0;
            while (reader.Offset < chunkEnd) {
                // Delta time.
                var delta = reader.ReadMultiByteValue ();

                // Event type.
                if ((reader.PeekByte () & 0x80) != 0) {
                    ev = reader.ReadByte ();
                }

                if (ev == 0xff) {
                    // 0xff: Meta event
                    //reader.Advance (1);
                    //reader.Advance (reader.ReadMultiByteValue ());

                    var eventType = reader.ReadByte();
                    var length = reader.ReadMultiByteValue();

                    if (eventType == 0x51 && length == 3)
                    {
                        byte first, second, third;
                        first = reader.ReadByte();
                        second = reader.ReadByte();
                        third = reader.ReadByte();

                        int lengthOfCrotchet = first << 8 * 2 | second << 8 * 1 | third << 8 * 0;

                        bpm = 60000000 / lengthOfCrotchet;
                    }
                    else if (eventType == 0x03)
                    {
                        // Text event
                        string text = new string(reader.ReadChars(length));
                        Debug.Log("Text event: Length " + length + " Text: " + text);
                        track.AddText(text);
                    }
                    else if (eventType == 0x58 && length == 4)
                    {
                        numerator = reader.ReadByte();
                        denominator = reader.ReadByte();
                        //clocksPerClick = reader.ReadByte();
                        byte hexClocksPerClick = reader.ReadByte();
                        clocksPerClick = byte.Parse(hexClocksPerClick.ToString("X"), System.Globalization.NumberStyles.HexNumber);
                        noted32ndNotesPerQuarterNote = reader.ReadByte();


                        Debug.Log("New time signature!");
                    }
                    else if (eventType == 0x59 && length == 2)
                    {
                        Debug.Log("Key Signature!");
                        sbyte sf, mi;
                        sf = (sbyte)reader.ReadByte();
                        mi = (sbyte)reader.ReadByte();
                        Debug.Log("SF (number of flats): " + sf + " | MI (major or minor): " + (mi == 0 ? "major" : "minor"));
                    }
                    else
                    {
                        reader.Advance(length);
                    }
                /*
                } else if ((ev & 0xf0) == 0xC0) {
                    // instrument or program change
                    int channel = ev & 0x0f;

                    var instrument = reader.ReadByte();
                    Debug.Log("Instrument Change: " + instrument + " (" + ((Instrument)instrument).Name() + ") Channel: " + channel);

                    if (!track.ProgramChangeExistsForChannel(channel))
                    {
                        Debug.Log("channel has not already got an instrument");
                        track.AddProgramChange(channel, instrument);
                    }
                */
                } else if (ev == 0xf0) {
                    // 0xf0: SysEx (unused).
                    while (reader.ReadByte() != 0xf7) {
                    }
                } else {
                    // MIDI event
                    byte data1 = reader.ReadByte ();
                    byte data2 = ((ev & 0xe0) == 0xc0) ? (byte)0 : reader.ReadByte ();
                    track.AddEvent (delta, new MidiEvent (ev, data1, data2), trackId);
                }
            }

            Debug.Log("Time Signature: " + numerator + "/" + Mathf.Pow(2, denominator) + " " + clocksPerClick + " MIDI clocks per click");
            Debug.Log(noted32ndNotesPerQuarterNote + " 32nd notes per quarter note");
            track.CurrentTimeSignature = new MidiTrack.TimeSignature(numerator, denominator, clocksPerClick, noted32ndNotesPerQuarterNote);

            return track;
        }

        #endregion
    }

    //
    // Binary data stream reader (for internal use)
    //
    class MidiDataStreamReader
    {
        byte[] data;
        int offset;

        public int Offset {
            get { return offset; }
        }

        public MidiDataStreamReader (byte[] data)
        {
            this.data = data;
        }

        public void Advance (int length)
        {
            offset += length;
        }

        public byte PeekByte ()
        {
            return data [offset];
        }

        public byte ReadByte ()
        {
            return data [offset++];
        }

        public char[] ReadChars (int length)
        {
            var temp = new char[length];
            for (var i = 0; i < length; i++) {
                temp [i] = (char)ReadByte ();
            }
            return temp;
        }

        public int ReadBEInt32 ()
        {
            int b1 = ReadByte ();
            int b2 = ReadByte ();
            int b3 = ReadByte ();
            int b4 = ReadByte ();
            return b4 + (b3 << 8) + (b2 << 16) + (b1 << 24);
        }

        public int ReadBEInt16 ()
        {
            int b1 = ReadByte ();
            int b2 = ReadByte ();
            return b2 + (b1 << 8);
        }

        public int ReadMultiByteValue ()
        {
            int value = 0;
            while (true) {
                int b = ReadByte ();
                value += b & 0x7f;
                if (b < 0x80)
                    break;
                value <<= 7;
            }
            return value;
        }
    }
}
