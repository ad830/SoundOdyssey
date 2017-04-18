using UnityEngine;
using UnityEngine.UI;
using SoundOdyssey;


namespace SoundOdyssey
{

    public class FinishedLevelManager : MonoBehaviour
    {

        Animator anim;                          // Reference to the animator component.

        Text pitchScoreText;
        Text durationAccuracyText;
        Text velocityAccuracyText;
        Text pitchAccuracyText;

        void HandleEndScreen(ScoreManager scoreMgr, int noteCount, bool isPracticeMode)
        {
            Debug.LogFormat("DurationScore: {0}", scoreMgr.DurationScore);
            float pitchAccuracy = (float) scoreMgr.PitchScore / (float) noteCount * 100;
            float durationAccuracy = (float) scoreMgr.DurationScore / (float) noteCount * 100;
            float expressionAccuracy = (float) scoreMgr.ExpressionScore / (float) noteCount * 100;
            string scoreComment = (isPracticeMode ? "\n[Practice Mode]" : "");
            
            pitchScoreText.text = string.Format("Score: {0} {1}", scoreMgr.PitchScore, scoreComment);
            pitchAccuracyText.text = string.Format("Accuracy: {0:0.##}%\nFluency: {1:0.##}%", pitchAccuracy, scoreMgr.Fluency);

            durationAccuracyText.text = string.Format("Duration: {0:0.##}%", durationAccuracy);
            velocityAccuracyText.text = string.Format("Expression: {0:0.##}%", expressionAccuracy);

            // ... tell the animator the game is over.
            anim.ResetTrigger("RestartLevelTrigger");
            anim.SetTrigger("FinishedLevelTrigger");
        }

        void HandleRestartLevel()
        {
            anim.ResetTrigger("FinishedLevelTrigger");
            anim.SetTrigger("RestartLevelTrigger");
        }

        void OnEnable()
        {
            SongLevelDirector.OnShowEndScreen += HandleEndScreen;
            SongLevelDirector.OnLevelRestart += HandleRestartLevel;
        }
        void OnDisable()
        {
            SongLevelDirector.OnShowEndScreen -= HandleEndScreen;
            SongLevelDirector.OnLevelRestart -= HandleRestartLevel;
        }

        void Awake()
        {
            // Set up the reference.
            anim = GetComponent<Animator>();
            Debug.Log("Awake in finishedlevelManager");

            pitchScoreText = GameObject.Find("Score").GetComponent<Text>();
            durationAccuracyText = GameObject.Find("Duration Accuracy").GetComponent<Text>();
            velocityAccuracyText = GameObject.Find("Velocity Accuracy").GetComponent<Text>();
            pitchAccuracyText = GameObject.Find("Pitch Accuracy").GetComponent<Text>();
        }


    }
}
