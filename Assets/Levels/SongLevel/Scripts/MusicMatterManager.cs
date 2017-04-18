using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SmfLite;
using Midi;
using MidiJack;

namespace SoundOdyssey
{
    public class MusicMatterManager : MonoBehaviour
    {

        private bool[] isAccidental = {
                                            false,
                                                true,
                                            false,
                                                true,
                                            false,
                                            false,
                                                true,
                                            false,
                                                true,
                                            false,
                                                true,
                                            false
                                       };
        private List<Vector2> stavePositions;

        #region Public colour constants
        public static Color SoftVolumeColour = new Color(0.1f, 0.6f, 0.8f);
        public static Color ModerateVolumeColour = new Color(0.1f, 0.8f, 0.4f);
        public static Color LoudVolumeColour = new Color(0.8f, 0.4f, 0.1f);
        #endregion

        #region Public methods
        public void SyncWithGameSettings()
        {
            showNoteAuras = MenuSession.Instance.gameSettings.AuraVisible;
            showNoteLabels = MenuSession.Instance.gameSettings.NoteLabels;

            UpdateNoteGameSettings();
        }
        #endregion

        #region Private methods
        private void UpdateNoteGameSettings()
        {
            for (int i = 0; i < poolObjects.Count; i++)
            {
                NoteDataHolder noteDataHolder = poolObjects[i].GetComponent<NoteDataHolder>();

                if (noteDataHolder.isPlayer)
                {
                    GameObject label = poolObjects[i].GetComponent<Transform>().Find("SongNoteLabel").gameObject;
                    label.SetActive(showNoteLabels);

                    GameObject aura = poolObjects[i].GetComponent<Transform>().Find("SongParticles").gameObject;
                    ParticleSystem particles = aura.GetComponent<ParticleSystem>();
                    particles.enableEmission = showNoteAuras;
                    if (!showNoteAuras)
                    {
                        particles.Clear();
                    }
                }
            }
        }

        Vector3 GetStavePosition(byte pitch)
        {
            //const byte lowestNoteNum = 21;      // on the grand staff
            //const byte highestNoteNum = 108;

            return stavePositions[pitch];
        }

        void SetupStavePositions()
        {
            const float spaceHeight = 0.5f;
            float y = -19f;
            for (int i = 0; i < 128; i++)
            {
                bool isSharp = isAccidental[i % 12];
                if (!isSharp)
                {
                    y += spaceHeight;
                }
                stavePositions.Add(new Vector2(0, y));
            }
        }

        void HandleNoteSpawn(MidiTrack.NoteDuration note, bool isPlayer)
        {
            GameObject obj = GetMatter();
            if (obj != null)
            {
                if (!note.isNoteEvent)
                {
                    //Debug.LogFormat("NON MIDI {0} @ {1}", note.midiEvent, note.delta);
                }

                // reset object
                Vector3 spawnPosition = GetStavePosition(note.pitch) + spawnPoint;
                obj.transform.position = spawnPosition;
                obj.transform.rotation = Quaternion.identity;

                if (!note.isNoteEvent)
                {
                    if ((note.midiEvent.status & 0xf0) == 0xB0)
                    {
                        Control ctrl = (Control)note.midiEvent.data1;
                        obj.name = string.Format("NON MIDI [{0}, {1}, {2}]", "CC", ctrl.Name(), note.midiEvent.data2);
                    }
                    else if ((note.midiEvent.status & 0xf0) == 0xC0)
                    {
                        Instrument ins = (Instrument)note.midiEvent.data1;
                        obj.name = string.Format("NON MIDI [{0}, {1}]", "PC", ins.Name());
                    }
                }
                else
                {
                    obj.name = Duration.ValueExtensionMethods.Name(note.value);
                }
                NoteDataHolder noteData = obj.GetComponent<NoteDataHolder>();
                noteData.data = note;
                noteData.ResetState();
                noteData.isPlayer = isPlayer;

                Sprite whichSprite = null;
                //Color softColor = new Color(0.1f, 0.6f, 0.8f);
                //Color moderateColor = new Color(0.1f, 0.8f, 0.4f);
                //Color loudColor = new Color(0.8f, 0.4f, 0.1f);
                Color whichColor = ModerateVolumeColour;//moderateColor;

                int band = GetVelocityBand(note.velocity, 3);
                switch (band)
                {
                    case 0: whichColor = SoftVolumeColour;
                        break;
                    case 1: whichColor = ModerateVolumeColour;
                        break;
                    case 2: 
                    case 3: whichColor = LoudVolumeColour;
                        break;
                    default:
                        break;
                }
                //Debug.LogFormat("Band {0} Vel {1}", band, note.velocity);
                

                if (note.value == Duration.Value.Whole)
                {
                    whichSprite = semibreveSprite;
                    //whichColor = Color.green;
                }
                if (note.value == Duration.Value.Half)
                {
                    whichSprite = minimSprite;
                    //whichColor = Color.yellow;
                }
                if (note.value == Duration.Value.Quarter)
                {
                    whichSprite = crotchetSprite;
                    //whichColor = Color.cyan;
                }
                if (note.value == Duration.Value.Eighth)
                {
                    whichSprite = quaverSprite;
                    //whichColor = Color.red;
                }
                if (note.value == Duration.Value.Sixteenth)
                {
                    whichSprite = semiquaverSprite;
                    //whichColor = Color.magenta;
                }
                if (note.value == Duration.Value.ThirtySecond)
                {
                    whichSprite = demisemiquaverSprite;
                    //whichColor = Color.blue;
                }

                SpriteRenderer sprRender = obj.GetComponent<Transform>().Find("SongSprite").GetComponent<SpriteRenderer>();
                if (whichSprite != null)
                {
                    sprRender.sprite = whichSprite;
                }
                sprRender.color = Color.white;

                if (!isPlayer)
                {
                    sprRender.color = Color.clear;
                }

                // set the accidental
                SpriteRenderer accidentalSprite = obj.transform.Find("SongAccidental").GetComponent<SpriteRenderer>();
                Sprite whichAccidental = null;
                if (isAccidental[note.pitch % 12] && isPlayer)
                {
                    if (preferAccidentals == PreferAccidental.Sharp)
                    {
                        whichAccidental = sharpNote;
                    }
                    else if (preferAccidentals == PreferAccidental.Flats)
                    {
                        whichAccidental = flatNote;
                    }
                }
                accidentalSprite.sprite = whichAccidental;


                //float numericalNoteValue = ((float)note.value + 8.0f) / 11.0f;
                //Hsv hsv = new Hsv(numericalNoteValue * 360f, 0.5, 1);
                //ColorSpace.Hsv hsv = new ColorSpace.Hsv((((int)note.pitch) / 127.0f) * 360f, 0.5, 1);


                //ColorSpace.Rgb rgb = ColorSpace.hsv2rgb(hsv);
                ParticleSystem particles = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
                particles.Clear();
                particles.enableEmission = false;
                //particles.startColor = new Color((float)rgb.r, (float)rgb.g, (float)rgb.b);
                if (showNoteAuras && isPlayer)
                {
                    particles.enableEmission = true;
                }

                if (showNoteAuras && note.isNoteEvent && isPlayer)
                {
                    particles.Stop();
                    particles.Play();
                }
                particles.startColor = whichColor;

                SpriteRenderer sprite = obj.transform.GetChild(0).GetComponent<SpriteRenderer>();
                sprite.transform.localScale = Vector3.one;
                obj.SetActive(true);

                TextMesh meshLabel = obj.transform.GetChild(2).GetComponent<TextMesh>();
                Pitch musicPitch = (Pitch)note.pitch;
                Note musicNote = musicPitch.NotePreferringSharps();
                
                if (preferAccidentals == PreferAccidental.Sharp)
                {
                    musicNote = musicPitch.NotePreferringSharps();
                }
                else if (preferAccidentals == PreferAccidental.Flats)
                {
                    musicNote = musicPitch.NotePreferringFlats();
                }
                
                meshLabel.text = "+";
                if (note.isNoteEvent && isPlayer)
                {
                    meshLabel.text = musicNote.ToString();
                }
                obj.transform.GetChild(2).gameObject.SetActive(showNoteLabels && isPlayer);

                LineRenderer durationGuide = obj.transform.GetChild(3).GetComponent<LineRenderer>();
                durationGuide.SetColors(whichColor, whichColor);

                shouldUpdate = true;
            }
        }

        // TODO: Write a handler for other MIDI Events like ProgramChange, ControlChange, PitchBend, etc.

        GameObject GetMatter()
        {
            for (int i = 0; i < poolObjects.Count; i++)
            {
                if (!poolObjects[i].activeInHierarchy)
                {
                    return poolObjects[i];
                }
            }

            if (willGrow)
            {
                GameObject obj = NewObject();
                poolObjects.Add(obj);
                return obj;
            }

            return null;
        }

        GameObject NewObject()
        {
            GameObject obj = Instantiate(prefab) as GameObject;
            obj.SetActive(false);
            obj.transform.SetParent(base.transform);
            return obj;
        }

        int GetVelocityBand(int value, int numBands = 5)
        {
            const int maxScore = 127;
            float bandStep = maxScore / numBands;
            int band = Mathf.RoundToInt((float)value / bandStep);
            //int bandedValue = Mathf.Round(value / bandStep) * bandStep;
            //int band = value / bandStep;
            //Debug.LogFormat("BandVelocity {0} max {1} bandNum {2} actual {3}", value, maxScore, numBands, bandedValue);
            return band;
        }

        void HandleLevelRestart()
        {
            ClearAllCurrentNotes();
        }

        void ClearAllCurrentNotes()
        {
            for (int i = 0; i < poolObjects.Count; i++) {
                if (poolObjects[i].activeInHierarchy)
                {
                    RecycleNote(i);
                }
            }
            shouldUpdate = false;
        }

        void RecycleNote(int idx)
        {
            LineRenderer durationGuide = poolObjects[idx].transform.GetChild(3).GetComponent<LineRenderer>();
            durationGuide.enabled = false;
            poolObjects[idx].SetActive(false);
        }
        #endregion

        #region Monobehaviour methods
        void OnEnable()
        {
            SongLevelDirector.OnNoteSpawn += HandleNoteSpawn;
            SongLevelDirector.OnLevelRestart += HandleLevelRestart;
        }
        void OnDisable()
        {
            SongLevelDirector.OnNoteSpawn -= HandleNoteSpawn;
            SongLevelDirector.OnLevelRestart -= HandleLevelRestart;
        }

        // Use this for initialization
        void Start()
        {
            poolObjects = new List<GameObject>(pooledAmount);
            for (int i = 0; i < pooledAmount; i++)
            {
                poolObjects.Add(NewObject());
            }
            spawnPoint = base.transform.GetChild(0).transform.position;
            GameObject director = GameObject.Find("Director");
            mixer = director.GetComponent<OutputMidiMixer>();
            songDirector = director.GetComponent<SongLevelDirector>();
            midiKeyboardTimeStamp = new float[128];
            for (int i = 0; i < 128; i++)
            {
                midiKeyboardTimeStamp[i] = 0.0f;
            }
            stavePositions = new List<Vector2>(128);
            SetupStavePositions();

            SyncWithGameSettings();

            //mixer.SendControlChange(Channel.Channel2, Control.Volume, 127);
            //mixer.SendControlChange(Channel.Channel1, (Control)122, 127);
            int debugCount = debugToggles.childCount;
            toggles = new List<Toggle>();
            for (int i = 0; i < debugCount; i++)
            {
                toggles.Add(debugToggles.GetChild(i).GetComponent<Toggle>());
            }
        }

        // Update is called once per frame
        void Update()
        {
            // secret key to toggle autoplay
            if (Input.GetKeyDown(KeyCode.P))
            {
                autoPlay = !autoPlay;
                secretAutoplayObject.SetActive(autoPlay);
            }
            if (!shouldUpdate) { return; }
            bool allInactive = true;
            float secPerTick = 60000f / (songDirector.Bpm * songDirector.Ppqn);
            for (int i = 0; i < poolObjects.Count; i++)
            {
                if (poolObjects[i].activeInHierarchy)
                {
                    allInactive = false;

                    var xPosition = poolObjects[i].transform.position.x;
                    poolObjects[i].transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

                    NoteDataHolder noteDataHolder = poolObjects[i].GetComponent<NoteDataHolder>();
                    MidiTrack.NoteDuration data = noteDataHolder.data;
                    ParticleSystem particles = poolObjects[i].transform.GetChild(1).GetComponent<ParticleSystem>();
                    SpriteRenderer sprite = poolObjects[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
                    LineRenderer durationGuide = poolObjects[i].transform.GetChild(3).GetComponent<LineRenderer>();
                    TextMesh meshLabel = poolObjects[i].transform.GetChild(2).GetComponent<TextMesh>();

                    var keyVelocity = MidiMaster.GetKey(data.pitch);

                    // lerp colour til expected
                    if (noteDataHolder.isPlayer)
                    {
                        if (xPosition > -5f && xPosition <= 3f)
                        {
                            sprite.color = Color.Lerp(sprite.color, Color.white, Time.deltaTime * 10.0f);
                        }
                    }

                    //  if the key has been held in prior to the note enterring the zone then ignore it
                    //  handles the case when keys are just held in
                    if (xPosition > -5f && xPosition < -4f)
                    {
                        if(MidiMaster.GetKey(data.pitch) > 0f)
                        {
                            noteDataHolder.acceptKeyPress = false;
                        }
                        else
                        {
                            noteDataHolder.acceptKeyPress = true;
                        }

                    }

                    // replace -7f with the xposition that the note should end
                    // v = d / t ... -> d = v * t
                    // d = 8 * (noteDurationInSeconds)
                    // -> delta distance
                    // going to the left
                    // expected start note position - delta distance = where the note should stop playing
                    // compare expected duration to duration played by player
                    /*
                    float secPerTick = 60000f / (songDirector.Bpm * songDirector.Ppqn);
                    float duration = (data.ticks * secPerTick) / 1000f;
                    */

                    float duration = (data.ticks * secPerTick) / 1000f;
                    float deltaDistance = (moveSpeed - 1) * duration;
                    float stopPosition = -5f - deltaDistance;
                    int mistakeChance = Random.Range(0, 100);
                    bool mistakeMade = mistakeChance >= 0 && mistakeChance <= aiMistakePercentage;

                    

                    if (xPosition <= -5f && xPosition > stopPosition)
                    {

                        //Might be in the zone and the key has gone up
                        if (!noteDataHolder.acceptKeyPress && keyVelocity == 0f)
                            noteDataHolder.acceptKeyPress = true;


                        if(noteDataHolder.acceptKeyPress)
                        {
                            if (!noteDataHolder.inZone)
                            {
                                sprite.transform.localScale = Vector3.one * 2f;
                                noteDataHolder.inZone = true;
                            }
                            if (keyVelocity > 0f && noteDataHolder.started)
                            {
                                // pressed the note after the note was expected to be played
                                //songDirector.ResetMultiplier();
                            }

                            if (!noteDataHolder.started)
                            {
                                //float secPerTick = 60000f / (songDirector.Bpm * songDirector.Ppqn);
                                //float duration = (data.ticks * secPerTick) / 1000f;
                                //mixer.SendNoteDuration((Channel)data.channel, (Pitch)data.pitch, data.velocity, duration);
                                if (!data.isNoteEvent)
                                {
                                    HandleNonMidiEvent(data);
                                    particles.Stop();
                                    noteDataHolder.started = true;
                                    meshLabel.text = "";
                                }
                                else if (!noteDataHolder.isPlayer)
                                {
                                    //Debug.LogFormat("Play non player note {0} Channel {1}", data.pitch, data.channel);
                                    noteDataHolder.started = true;
                                    particles.Stop();
                                    mixer.SendNoteOn((Channel)data.channel, (Pitch)data.pitch, data.velocity);
                                }
                                else if (keyVelocity > 0f || (autoPlay && !aiMakesMistakes || (autoPlay && aiMakesMistakes && mistakeMade)))
                                {
                                    meshLabel.text = "";
                                    /*
                                    Velocity Bands:
                                    ff
                                    f
                                    mf
                                    p
                                    pp

                                    25.6 each
                                    pp: 1 - 24.6
                                    p:  24.6 -> 49.2
                                    mf: 49.2 -> 73.8
                                    f: 73.8 -> 98.4
                                    ff: 98.4 -> 127
                                    */
                                    int actualVelocity = (int)(keyVelocity * 127);
                                    if (autoPlay)
                                    {
                                        actualVelocity = data.velocity;
                                    }
                                    //Debug.LogFormat("Expected Velocity: {0} Actual Velocity {1}", data.velocity, actualVelocity);
                                    //Debug.LogFormat("E Band {0}, K Band {1}", GetVelocityBand(data.velocity), GetVelocityBand(actualVelocity));
                                    bool velocityMatches = GetVelocityBand(data.velocity) == GetVelocityBand(actualVelocity);
                                    //Debug.LogFormat("Velocity Band {0} <- {1} You hit {2} <- {3}", GetVelocityBand(data.velocity), data.velocity, GetVelocityBand(actualVelocity), actualVelocity);
                                    if (!songDirector.LevelParameters.ignoreVelocity && velocityMatches)
                                    {
                                        //Debug.LogFormat("Velocity Matches");
                                        songDirector.ChangeExpressionScore(1);

                                    }
                                    
                                    songDirector.ChangePitchScore(1);
                                    
                                    noteDataHolder.started = true;
                                    
                                    particles.Stop();
                                    // if (!autoPlay)
                                    // {
                                    //     mixer.SendNoteOn((Channel)data.channel, (Pitch)data.pitch, data.velocity);
                                    // }
                                    // else
                                    // {
                                    //     mixer.SendNoteDuration((Channel)data.channel, (Pitch)data.pitch, data.velocity, duration);
                                    // }

                                    mixer.SendNoteOn((Channel)data.channel, (Pitch)data.pitch, data.velocity);

                                    if (OnCaughtNote != null)
                                    {
                                        OnCaughtNote((Pitch)data.pitch, data.velocity);
                                    }
                                    sprite.color = Color.white;

                                    //Debug.LogFormat("Channel {0}", (Channel)data.channel);
                                    float distance = xPosition - (-5f);
                                    float fluency = (1 - (Mathf.Abs(distance) / 2f)) * 100f;
                                    //Debug.LogFormat("DISTANCE FROM EXPECTED: {0}, Actual xpos {1} Fluency {2}", distance, xPosition, fluency);
                                    songDirector.AddFluency(fluency);

                                    if (data.isNoteEvent && !songDirector.LevelParameters.pitchOnly)
                                    {
                                        Vector3 normalPos = new Vector3(-5f, poolObjects[i].transform.position.y, poolObjects[i].transform.position.z);
                                        durationGuide.SetPosition(0, normalPos);
                                        durationGuide.SetPosition(1, normalPos);
                                        durationGuide.enabled = true;
                                    }

                                    noteDataHolder.startPosition = xPosition;
                                }
                            }
                            else
                            {
                                if (data.isNoteEvent && !songDirector.LevelParameters.pitchOnly && noteDataHolder.isPlayer)
                                {
                                    Vector3 startPos = new Vector3(stopPosition, poolObjects[i].transform.position.y, poolObjects[i].transform.position.z);
                                    Vector3 endPos = poolObjects[i].transform.position;
                                    durationGuide.SetPosition(0, startPos);
                                    durationGuide.SetPosition(1, endPos);
                                }
                            }

                            //toggles[0].isOn = noteDataHolder.isReleased;
                            //toggles[1].isOn = noteDataHolder.started;
                            //toggles[2].isOn = keyVelocity == 0f;
                            //toggles[3].isOn = autoPlay;
                            //toggles[4].isOn = data.isNoteEvent;
                            //toggles[5].isOn = noteDataHolder.isPlayer;
                            if (!noteDataHolder.isReleased && noteDataHolder.started && keyVelocity == 0f && !autoPlay && data.isNoteEvent && noteDataHolder.isPlayer)
                            {
                                //Debug.Log("CHECK DUR");
                                mixer.SendNoteOff((Channel)data.channel, (Pitch)data.pitch, data.velocity);

                                //check duration here
                                //float deltaDistance = (moveSpeed - 1) * duration;
                                //float stopPosition = -5f - deltaDistance;

                                CheckDuration(noteDataHolder.startPosition, xPosition, deltaDistance);

                                noteDataHolder.isReleased = true;
                            }
                        }

                    }
                    else if(xPosition <= stopPosition)
                    {
                        // if (!autoPlay)
                        // {
                        //     mixer.SendNoteOff((Channel)data.channel, (Pitch)data.pitch, data.velocity);
                        // }
                        if (data.isNoteEvent)
                        {
                            mixer.SendNoteOff((Channel)data.channel, (Pitch)data.pitch, data.velocity);
                            if (noteDataHolder.isPlayer)
                            {
                                if (!noteDataHolder.isReleased && noteDataHolder.started)
                                {
                                    if ((keyVelocity == 0f && !autoPlay) || autoPlay)
                                    {
                                        CheckDuration(noteDataHolder.startPosition, xPosition, deltaDistance);
                                        noteDataHolder.isReleased = true;
                                    }
                                }
                                else
                                {
                                    // songDirector.ResetMultiplier();
                                    if (OnMissedNote != null)
                                    {
                                        OnMissedNote((Pitch)data.pitch, data.velocity);
                                    }
                                }
                            }
                        }

                        durationGuide.enabled = false;
                        // recycle object
                        poolObjects[i].SetActive(false);
                    }
                }
            }

            if (allInactive && songDirector.Started && songDirector.AllNotesSpawned)
            {
                songDirector.FinishLevel();
                shouldUpdate = false;
            }
        }

        private void HandleNonMidiEvent(MidiTrack.NoteDuration data)
        {
            MidiStatusType eventType = (MidiStatusType)(data.midiEvent.status & 0xf0);
            Channel channel = (Channel)(data.midiEvent.status & 0x0f);

            switch (eventType)
            {
                case MidiStatusType.ProgramChange:
                    Instrument instrument = (Instrument)data.midiEvent.data1;
                    if (instrument.IsValid())
                    {
                        mixer.SendProgramChange(channel, instrument);
                    }
                    //Debug.LogFormat("PC {0} -> {1}", channel.Name(), instrument.Name());
                    break;
                case MidiStatusType.PitchBend:
                    int lsb = data.midiEvent.data1;
                    int msb = data.midiEvent.data2;
                    int value = msb << 7 | lsb;
                    mixer.SendPitchBend(channel, value);
                    break;
                case MidiStatusType.ControlChange:
                    Control control = (Control)data.midiEvent.data1;
                    bool valueIsValid = data.midiEvent.data2 >= 0 && data.midiEvent.data2 <= 127;
                    if (control.IsValid() && valueIsValid)
                    {
                        mixer.SendControlChange(channel, control, data.midiEvent.data2);
                    }
                    //Debug.LogFormat("CC {0} : {1} {2}", channel.Name(), control.Name(), data.midiEvent.data2);
                    break;
                default:
                    Debug.LogFormat("Unknown event type {0}", eventType);
                    break;
            }
        }

        private void CheckDuration(float startPosition, float xPosition, float deltaDistance)
        {
            float distanceTravelled = Mathf.Abs(xPosition - startPosition);//-xPosition + startPosition;

            float percentagePlayed = distanceTravelled / deltaDistance;
            percentagePlayed = Mathf.Min(percentagePlayed, 1.0f);

            //Debug.LogFormat("Played {0}% dist {1}", percentagePlayed, distanceTravelled);

            if (percentagePlayed >= 0.5 && !songDirector.LevelParameters.pitchOnly)
            {
                //Debug.Log("you played more than 50%");
                songDirector.ChangeDurationScore(1);
            }
        }
        #endregion

        #region Public delegates
        public delegate void MissedNoteHandler(Pitch pitch, int vel);
        public static event MissedNoteHandler OnMissedNote;

        public delegate void CaughtNoteHandler(Pitch pitch, int vel);
        public static event CaughtNoteHandler OnCaughtNote;
        #endregion

        #region Serialised variables
        public enum PreferAccidental { Sharp, Flats };

        public int pooledAmount = 100;
        [SerializeField]
        private bool willGrow = false;
        [SerializeField]
        private GameObject prefab;
        public float moveSpeed = 8f;
        [SerializeField]
        private bool autoPlay = false;
        [SerializeField]
        private bool aiMakesMistakes = false;
        [SerializeField]
        private float aiMistakePercentage = 10f;
        [SerializeField]
        private PreferAccidental preferAccidentals = PreferAccidental.Sharp;

        [SerializeField]
        private Sprite normalNote;
        [SerializeField]
        private Sprite flatNote;
        [SerializeField]
        private Sprite sharpNote;
        [SerializeField]
        private Sprite naturalNote;
        [SerializeField]
        private Sprite semibreveSprite;
        [SerializeField]
        private Sprite minimSprite;
        [SerializeField]
        private Sprite crotchetSprite;
        [SerializeField]
        private Sprite quaverSprite;
        [SerializeField]
        private Sprite semiquaverSprite;
        [SerializeField]
        private Sprite demisemiquaverSprite;
        [SerializeField]
        private GameObject secretAutoplayObject;
        #endregion

        #region Private variables
        bool shouldUpdate = true;
        List<GameObject> poolObjects;
        Vector3 spawnPoint;
        OutputMidiMixer mixer;
        SongLevelDirector songDirector;
        float[] midiKeyboardTimeStamp;
        [SerializeField]
        bool showNoteLabels = true;
        [SerializeField]
        bool showNoteAuras = true;

        [SerializeField]
        Transform debugToggles;
        List<Toggle> toggles;
        #endregion
    }
}
