using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class ScoreKeeper : MonoBehaviour
    {
        Text text;

        void HandleScore(int value)
        {
            text.text = string.Format("Score: {0}", value);
        }

        void OnEnable()
        {
            SongLevelDirector.OnScoreChange += HandleScore;
        }
        void OnDisable()
        {
            SongLevelDirector.OnScoreChange -= HandleScore;
        }

        // Use this for initialization
        void Start()
        {
            text = GetComponent<Text>();
        }
    }    
}
