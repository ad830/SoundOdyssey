using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class TempoScaleLabel : MonoBehaviour
    {

        [SerializeField]
        Slider slider;
        [SerializeField]
        float multiple = 5;
        [SerializeField]
        Text label;

        // Use this for initialization
        void Start()
        {
            label = base.GetComponent<Text>();
        }

        public void UpdateLabel(float value)
        {
            float actualValue = value * multiple;
            label.text = string.Format("{0}%", actualValue);
        }
    }
    
}

