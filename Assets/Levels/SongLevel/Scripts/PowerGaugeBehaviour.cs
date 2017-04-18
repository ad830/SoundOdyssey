using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SoundOdyssey;

namespace SoundOdyssey
{
    // Represents Fluency
    public class PowerGaugeBehaviour : MonoBehaviour
    {
        private float value;        // percentage
        private float min;
        private float max;
        private float range;
        private int streak;
        private float movingValue;
        private Slider slider;

        public int maxGauge = 5;

        void HandleNoteMiss(Midi.Pitch pitch, int velocity)
        {
            UpdateStreak(-1);
            UpdateBarState();
        }

        void HandleNoteCatch(Midi.Pitch pitch, int velocity)
        {
            UpdateStreak(1);
            UpdateBarState();
        }

        void UpdateBarState()
        {
            value = (float)(streak) / maxGauge;
            value = Mathf.Min(value, 1f);
            // Left=position.x Right=sizeDelta.x PosY=position.y PosZ=position.z Height=sizeDelta.y
        }

        void UpdateStreak(int amount)
        {
            streak += amount;
            streak = Mathf.Clamp(streak, 0, maxGauge);
        }

        void HandleLevelRestart()
        {
            ResetDisplay();
        }

        void OnEnable()
        {
            MusicMatterManager.OnCaughtNote += HandleNoteCatch;
            MusicMatterManager.OnMissedNote += HandleNoteMiss;
            SongLevelDirector.OnLevelRestart += HandleLevelRestart;
        }
        void OnDisable()
        {
            MusicMatterManager.OnCaughtNote -= HandleNoteCatch;
            MusicMatterManager.OnMissedNote -= HandleNoteMiss;
            SongLevelDirector.OnLevelRestart -= HandleLevelRestart;
        }

        // Use this for initialization
        void Start()
        {
            ResetDisplay();
            slider = base.GetComponent<Slider>();
        }

        // Update is called once per frame
        void Update()
        {
            movingValue = Mathf.Lerp(movingValue, value, Time.deltaTime * 6f);
            slider.value = movingValue;
        }

        public void ResetDisplay()
        {
            value = 0;
            movingValue = 0;
            streak = 0;
        }
    }
}
