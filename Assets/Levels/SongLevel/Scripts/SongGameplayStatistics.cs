using UnityEngine;
using System.Collections;
using SoundOdyssey;
using Midi;

namespace SoundOdyssey
{
    public class SongGameplayStatistics : MonoBehaviour
    {
        GameDirector gameDirector;
        MenuSession session;

        void HandleCaughtNote(Pitch pitch, int vel)
        {
            if (gameDirector.CurrentProfile != null)
            {
                gameDirector.CurrentProfile.Statistics.notesPlayed++;
                UpdateCurrentPlayMissRatio();
            }
        }
        void HandleMissedNote(Pitch pitch, int vel)
        {
            if (gameDirector.CurrentProfile != null)
            {
                gameDirector.CurrentProfile.Statistics.notesMissed++;
                UpdateCurrentPlayMissRatio();
            }
        }

        float CalculatePlayMissRatio()
        {
            const float defaultRatio = 1.0f;
            bool noNotesMissed = gameDirector.CurrentProfile.Statistics.notesMissed == 0;
            bool noNotesPlayed = gameDirector.CurrentProfile.Statistics.notesPlayed == 0;

            if (noNotesMissed)
            {
                if (noNotesPlayed)
                {
                    return defaultRatio;
                }
                else
                {
                    return gameDirector.CurrentProfile.Statistics.notesPlayed;
                }
            }
            return (float)gameDirector.CurrentProfile.Statistics.notesPlayed / (float)gameDirector.CurrentProfile.Statistics.notesMissed;
        }

        void UpdateCurrentPlayMissRatio()
        {
            gameDirector.CurrentProfile.Statistics.currentPlayMissRatio = CalculatePlayMissRatio();
        }

        void OnEnable()
        {
            MusicMatterManager.OnCaughtNote += HandleCaughtNote;
            MusicMatterManager.OnMissedNote += HandleMissedNote;
        }
        void OnDisable()
        {
            MusicMatterManager.OnCaughtNote -= HandleCaughtNote;
            MusicMatterManager.OnMissedNote -= HandleMissedNote;
        }

        // Use this for initialization
        void Start()
        {
            gameDirector = GameDirector.Instance;
            session = MenuSession.Instance;
        }
    }
    
}