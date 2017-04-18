using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Midi;
using MidiJack;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class PlayScales : MonoBehaviour
    {
        Rigidbody[] bodies;
        MeshRenderer[] renderers;
        TextMesh[] labels;

        // state
        int noteIdx;
        bool ascended = false;
        bool finished = false;
        int correctNotes = 0;

        // params
        Scale scale;
        [SerializeField]
        int octaveNum = 1;
        [SerializeField]
        int startOctave = 4;
        [SerializeField]
        bool useSoundEffects = true;
        
        float keyPressDeadZone = 0.225f;
        [SerializeField]
        bool showLabels = true;

        Pitch[] notes;
        OutputMidiMixer mixer;

        // References
        [SerializeField]
        ScaleDirector scaleDirector;
        [SerializeField]
        GameObject scalesNotePrefab;
        [SerializeField]
        float spacing = 0.125f;
        [SerializeField]
        Text infoText;

        #region Methods to handle bodies
        private void SpawnScaleBodies(int bodyCount)
        {
            bodies = new Rigidbody[bodyCount];
            renderers = new MeshRenderer[bodyCount];
            labels = new TextMesh[bodyCount];

            float range = 7f - (-7f);
            float scale = (range - spacing * (bodyCount - 1)) / bodyCount;

            scale = Mathf.Min(scale, 1.0f);

            /*
             14 
             bc: 15
             spacing: 0.125
             * 
             divide 14 by how 
             */

            Debug.LogFormat("Range {0} Scale {1}", range, scale);
            float x = -7f;
            for (int i = 0; i < bodyCount; i++)
            {
                GameObject obj = Instantiate(scalesNotePrefab, Vector3.zero, Quaternion.identity) as GameObject;

                //float xScale = obj.transform.localScale.x;
                //float halfBodyCount = bodyCount / 2f;
                //float x = -(halfBodyCount * scale) + i * (scale + spacing);
                Vector3 position = new Vector3(x, 0, 0);
                Vector3 localScale = Vector3.one * scale;

                x += scale + spacing;

                obj.transform.localPosition = position;
                obj.transform.localScale = localScale;
                obj.transform.SetParent(base.transform);
                obj.name = notes[i].ToString();

                bodies[i] = obj.GetComponent<Rigidbody>();
                renderers[i] = obj.GetComponent<MeshRenderer>();
                labels[i] = obj.GetComponentInChildren<TextMesh>();

                if (!showLabels)
                {
                    obj.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }

        private void RemoveScaleBodies()
        {
            if (bodies == null) { return; }
            for (int i = 0; i < bodies.Length; i++)
            {
                Destroy(bodies[i].gameObject);
            }
            bodies = null;
            renderers = null;
            labels = null;
        }

        private void ResetBody(int idx)
        {
            bodies[idx].useGravity = false;
            bodies[idx].velocity = Vector3.zero;
            bodies[idx].MovePosition(new Vector3(bodies[idx].position.x, 0));
        }

        private void ResetAllBlocks(int lastIdx)
        {
            for (int i = 0; i <= lastIdx; i++)
            {
                UnlightBlock(i);
                ResetBody(i);
            }
        }

        private void UnlightBlock(int idx)
        {
            renderers[idx].material.color = Color.white;
        }

        private void HighlightBlock(int idx)
        {
            renderers[idx].material.color = Color.yellow;
            bodies[idx].MovePosition(bodies[idx].position + Vector3.up * bodies[idx].GetComponent<Transform>().localScale.y);
        }

        private void UpdateLabels()
        {
            // for ascending notes

            for (int i = 0; i < labels.Length - 1; i++)
            {
                labels[i].text = scale.NoteSequence[i % scale.NoteSequence.Length].ToString();
            }
            labels[labels.Length - 1].text = scale.NoteSequence[0].ToString();

            // for descending notes
            /*
            List<Note> flippedSequence = new List<Note>(scale.NoteSequence);
            flippedSequence.Reverse();
            labels[0].text = flippedSequence[flippedSequence.Count - 1].ToString();
            for (int i = 1; i < labels.Length; i++)
            {
                int seqIdx = (i - 1) % flippedSequence.Count;
                labels[i].text = flippedSequence[seqIdx].ToString();
            }
            */
        }

        private void TriggerCorrectNote(int idx)
        {
            renderers[idx].material.color = Color.green;
            if (useSoundEffects)
            {
                mixer.SendPercussion(Percussion.Tambourine, 127);
            }
            correctNotes++;
        }

        private void TriggerIncorrectNote(int idx)
        {
            renderers[noteIdx].material.color = Color.red;
            if (useSoundEffects)
            {
                mixer.SendPercussion(Percussion.VibraSlap, 127);
            }
        }
        private void TriggerFinishScale()
        {

            if (useSoundEffects)
            {
                mixer.SendPercussion(Percussion.CrashCymbal1, 127);
            }
            
            infoText.text = "Finished!";

            if (!scaleDirector.IsPracticeMode)
            {
                // tell game director if they passed

                // see if there is a next scale or something to do

                if(GameDirector.Instance.CurrentScaleProgress == 0)
                {
                    //ScalesSection scalesData = GameDirector.Instance.ExamScalesSection;
                    
                    //int whichScale = 0;
                    //int currentScale = GameDirector.Instance.CurrentScaleIndex;

                    //do
                    //{
                    //    whichScale = UnityEngine.Random.Range(0, scalesData.Scales.Length);
                    //} while (whichScale == currentScale);
                                   
                    //GameDirector.Instance.PlayExamScales(scalesData, whichScale, false);

                    //played first scale
                    GameDirector.Instance.CurrentScaleProgress++;

                    GameDirector.Instance.MaxScaleScore = notes.Length * 2 - 1;

                    Debug.LogFormat("maxScaleScore: {0}", GameDirector.Instance.MaxScaleScore);

                    GameDirector.Instance.ScaleScore = 0;
                    GameDirector.Instance.ScaleScore += correctNotes;
                    scaleDirector.ShowNextButton();
                                        
                }
                else
                {
                    GameDirector.Instance.CurrentScaleProgress = 0;

                    GameDirector.Instance.MaxScaleScore += notes.Length * 2 - 1;

                    Debug.LogFormat("maxScaleScore: {0}", GameDirector.Instance.MaxScaleScore);

                    GameDirector.Instance.ScaleScore += correctNotes;
                    //return to menu, handle scores
                    scaleDirector.ShowGetResultsButton();


                }



            }
        }
        #endregion

        public void SetupScale(Scale _scale, int _octaveNum, int _startOctave)
        {
            this.scale = _scale;
            this.octaveNum = _octaveNum;
            this.startOctave = _startOctave;

            // reset state
            showLabels = scaleDirector.IsPracticeMode;
            correctNotes = 0;
            noteIdx = 0;
            finished = false;
            ascended = false;
            notes = new Pitch[(octaveNum * scale.NoteSequence.Length) + 1];
            

            Debug.Log("Scale Pattern");
            for (int z = 0; z < scale.Pattern.Ascent.Length; z++)
            {
                Debug.LogFormat("{0}", scale.Pattern.Ascent[z]);
            }
            Debug.Log("Note Sequence");
            for (int a = 0; a < scale.NoteSequence.Length - 1; a++)
            {
                Debug.LogFormat("{0}", scale.NoteSequence[a].SemitonesUpTo(scale.NoteSequence[a + 1]));
            }

            /*
             * How to build up a scale for a given scale, start octave and octave num?
             * 
             * Start from the tonic note of the scale, get that pitch in the start octave:
             * 
             * Now we run up the scale according to the semitones til the next note
             * 
            */

            // assign all the expected notes from the scale pattern and octave count
            Pitch startPitch = scale.NoteSequence[0].PitchInOctave(startOctave);
            Pitch currentPitch = startPitch;
            for (int n = 0; n < octaveNum; n++)
            {
                for (int i = 0; i < scale.NoteSequence.Length - 1; i++)
                {
                    int idx = n * scale.NoteSequence.Length + i;
                    notes[idx] = currentPitch;
                    currentPitch += scale.NoteSequence[i].SemitonesUpTo(scale.NoteSequence[i + 1]);
                }
                int octaveIdx = n * scale.NoteSequence.Length + (scale.NoteSequence.Length - 1);

                notes[octaveIdx] = currentPitch;
                currentPitch += scale.NoteSequence[scale.NoteSequence.Length - 1].SemitonesUpTo(scale.NoteSequence[0]);
            }
            notes[notes.Length - 1] = scale.NoteSequence[0].PitchInOctave(startOctave + octaveNum);

            for (int p = 0; p < notes.Length; p++)
            {
                Debug.LogFormat("Note {0} num {1} oct {2}", notes[p], (int)(notes[p]), (p + 1) % 8 == 0);
            }

            RemoveScaleBodies();
            SpawnScaleBodies(notes.Length);
            UpdateLabels();
            HighlightBlock(noteIdx);

            infoText.text = "Play the yellow notes!\n→";
            if (!scaleDirector.IsPracticeMode)
            {
                infoText.text = "Play the scale from " + startPitch + "\n→";
            }
        }

        public void Reset()
        {
            SetupScale(this.scale, this.octaveNum, this.startOctave);
        }

        // Use this for initialization
        void Start()
        {
            mixer = GetComponent<OutputMidiMixer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (notes == null)
            {
                return;
            }
            if (finished)
            {
                return;
            }

            Func<int, int, bool> continuePlaying = null;
            int increment = 0;
            if (!ascended)
            {
                //continuePlaying = noteIdx < notes.Length;
                continuePlaying = (idx, length) => { return idx >= 0 && idx < length; };
                increment = 1;
            }
            else
            {
                //continuePlaying = noteIdx >= 0;
                continuePlaying = (idx, length) => { return idx >= 0; };
                increment = -1;
            }

            if (continuePlaying(noteIdx, notes.Length))
            {
                // check on any key being pressed
                for (int i = 0; i < 128; i++)
                {
                    bool pressedPastDeadzone = MidiMaster.GetKey(i) >= keyPressDeadZone;
                    bool keyDown = MidiMaster.GetKeyDown(i) && pressedPastDeadzone;

                    if (keyDown && !finished)
                    {
                        // checking for correct note being played
                        if (i == (int)notes[noteIdx])
                        {
                            // trigger what should happen when the note is correct
                            TriggerCorrectNote(noteIdx);
                        }
                        else
                        {
                            // when the note is incorrect
                            TriggerIncorrectNote(noteIdx);
                        }

                        bodies[noteIdx].useGravity = true;

                        // go to the next expected note
                        noteIdx += increment;
                        if (continuePlaying(noteIdx, notes.Length))
                        {
                            HighlightBlock(noteIdx);
                        }
                        else
                        {
                            // what should happen when a motion has finished
                            if (!ascended)
                            {
                                ascended = true;
                                ResetAllBlocks(notes.Length - 2);
                                noteIdx = notes.Length - 2;
                                HighlightBlock(noteIdx);
                                infoText.text = "Play the yellow notes!\n←";
                                if (!scaleDirector.IsPracticeMode)
                                {
                                    infoText.text = "Play the scale\n←";
                                }
                            }
                            else
                            {
                                // trigger what should happen when the scale is finished
                                finished = true;
                                TriggerFinishScale();
                                Debug.Log("Finished!");
                            }
                        }
                    }
                }
            }
        }
    }    
}


