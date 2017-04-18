using Midi;
using SoundOdyssey;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SoundOdyssey
{
    public class ExamResultDisplay : MonoBehaviour {

        [SerializeField]
        Text resultText;

        public void ExitLevel()
        {
            GameDirector.Instance.LoadLevel("GalaxyMenu");
        }

	    // Use this for initialization
	    void Start () {

            float score = GameDirector.Instance.ScaleScore / (float) GameDirector.Instance.MaxScaleScore;

            int readableScore =(int) (score * 100);

            if (score > 0.85f)
            {
                resultText.text = "You achieved " + readableScore + "% and passed!";
            }
            else
            {
                resultText.text = "Your score of " + readableScore + "% is not enough to pass.";
            }
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }
    }
   
}