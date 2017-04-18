using UnityEngine;
using System.Collections;
using SoundOdyssey;


namespace SoundOdyssey
{
    public class NextLevelFromDuration : MonoBehaviour
    {
        public void LoadSongFromDuration()
        {
            // player has now 'played' this teaching level
            string levelName = GameDirector.Instance.TutorialLevel.common.title;
            MenuSession.Instance.SetPlayedLevel(levelName);

            SongLevelParams songParams = new SongLevelParams(true, false, false, true);
            GameDirector.Instance.PlayGalaxySongLevel("Hot Cross Buns.mid", songParams);
        }


    }

}
