using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Midi;

namespace SoundOdyssey
{
    public struct SongLevelParams
    {
        public bool practiceMode;               // ignores score, doesn't save score and only progresses song when the player has played the current notes
        public bool pitchOnly;                  // ignores duration and velocity checks
        public bool ignoreVelocity;             // ignores velocity checks
        public bool isGalaxySong;               // is a song from the galaxy (not campaign)

        /*
        public bool measureScore;
        public bool saveScore;
                           
        Valid ignore modes:
        Type        |PVD
        ================
        Normal      |111
        Melody      |100
        Rhythm      |001
        Ignore Dyn  |101
        Melody w/Dyn|110
        Rhythm w/Dyn|011
        
        Invalid:
        ================
        Just Dyn.   |010
        
        public bool ignorePitch;
        public bool ignoreVelocity;             
        public bool ignoreDuration;
        public bool isGalaxySong;
        */

        public SongLevelParams(bool practice = false, bool onlyPitch = false, bool ignoreVelocity = false, bool isGalaxySong = false)
        {
            this.practiceMode = practice;
            this.pitchOnly = onlyPitch;
            this.ignoreVelocity = ignoreVelocity;
            this.isGalaxySong = isGalaxySong;
        }

        public override string ToString()
        {
            return string.Format("Params [PracMode:{0} PitchOnly:{1} IgnoreVelocity:{2} isGalaxySong: {3}]", practiceMode, pitchOnly, ignoreVelocity, isGalaxySong);
        }
    }
}
