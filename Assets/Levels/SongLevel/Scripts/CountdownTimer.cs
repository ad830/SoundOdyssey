using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class CountdownTimer : MonoBehaviour
    {
        #region Public variables
        public float timerDuration = 3f;
        public bool shouldDisplay = true;

        #endregion

        #region Public events and delegates
        // Behaviour that should happen on tick
        public delegate void TickHandler();
        public static event TickHandler OnTick;

        // Behaviour that should happen on the last tick
        public delegate void LastTickHandler();
        public static event LastTickHandler OnLastTick;
        #endregion

        #region Private variables
        float timer = 0f;
        float perSecTimer = 0f;
        bool isEnabled = false;
        Text text;
        #endregion

        #region Private methods
        void StartCountDown()
        {
            Reset();
            isEnabled = true;
            text.enabled = true;
        }

        void Reset()
        {
            isEnabled = false;
            timer = timerDuration;
        }
        #endregion

        #region Monobehaviour methods
        void OnEnable()
        {
            Reset();
            // subscribe to the OnLevelStart event from SongLevelDirector
            SongLevelDirector.OnLevelStart += StartCountDown;
        }
        void OnDisable()
        {
            // unsubscribe to the OnLevelStart event from SongLevelDirector
            SongLevelDirector.OnLevelStart -= StartCountDown;
        }

        // Use this for initialization
        void Start()
        {
            if (shouldDisplay)
            {
                text = GetComponent<Text>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isEnabled)
            {
                if (shouldDisplay)
                {
                    int seconds = (int)timer + 1;
                    if (text != null)
                    {
                        if (seconds != 0)
                        {
                            text.text = seconds.ToString();
                        }
                        else
                        {
                            text.text = "";
                        }

                    }
                }

                perSecTimer -= Time.deltaTime;
                if (perSecTimer <= 0 && OnTick != null)
                {
                    perSecTimer = 1f;
                    OnTick();
                }

                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    if (shouldDisplay)
                    {
                        text.text = "";
                    }

                    isEnabled = false;
                    // the timer is finished
                    SongLevelDirector director = GameObject.Find("Director").GetComponent<SongLevelDirector>();
                    director.StartLevel();

                    text.enabled = false;

                    if (OnLastTick != null)
                    {
                        OnLastTick();
                    }
                }
            }
        }
        #endregion
    }

}

