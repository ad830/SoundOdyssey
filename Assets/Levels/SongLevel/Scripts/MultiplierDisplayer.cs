using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class MultiplierDisplayer : MonoBehaviour
    {
        Text text;

        void HandleMultiplier(int value)
        {
            text.text = string.Format("Multiplier: {0}", value);
        }

        void OnEnable()
        {
            SongLevelDirector.OnMultiplierChange += HandleMultiplier;
        }
        void OnDisable()
        {
            SongLevelDirector.OnMultiplierChange -= HandleMultiplier;
        }

        // Use this for initialization
        void Start()
        {
            text = GetComponent<Text>();
        }
    }   
}

