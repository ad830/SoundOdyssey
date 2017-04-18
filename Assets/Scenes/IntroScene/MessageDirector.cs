using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class MessageDirector : MonoBehaviour
    {
        #region Public member variables
        public string[] messages;
        public float messageDuration = 3f;
        public float initialDelay = 2f;
        public Text textObject;
        #endregion

        #region Private member variables
        private int messageIndex;
        private float timer;
        private float duration;
        private float fadeDuration;
        private float visibleDuration;
        private int stage;
        private const int stageMax = 3;
        private bool done;

        #endregion

        #region Inspector variables
        public UnityEvent doneCallback;
        [SerializeField]
        private bool allowSkip = true;
        #endregion

        void OnDone()
        {
            doneCallback.Invoke();
            Application.LoadLevelAsync("SplashScreen");
        }

        #region MonoBehaviour functions

        void OnEnable()
        {
            messageIndex = 0;
            timer = 0;

            fadeDuration = messageDuration / 4;
            visibleDuration = messageDuration / 2;

            if (initialDelay <= 0)
            {
                duration = fadeDuration;
                stage = 0;
            }
            else
            {
                stage = -1;
                duration = initialDelay;
            }


            done = false;

            textObject.color = Color.clear;
            textObject.text = messages[messageIndex++];
        }

        void OnDisable()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!done)
            {
                if (allowSkip && Input.anyKeyDown)
                {
                    stage = stageMax - 1;
                }

                timer += Time.deltaTime;
                if (timer > duration)
                {
                    timer = 0;

                    if (stage < stageMax - 1)
                    {
                        stage++;
                    }
                    else
                    {
                        stage = 0;
                        // finished showing one message, continue sequence
                        if (messageIndex < messages.Length)
                        {
                            textObject.text = messages[messageIndex++];
                        }
                        else
                        {
                            done = true;
                            OnDone();
                        }
                    }
                    if (stage == 1)
                    {
                        duration = visibleDuration;
                    }
                    else
                    {
                        duration = fadeDuration;
                    }
                }

                if (stage >= 0 && stage != 1)
                {
                    float alpha = timer / duration;
                    if (stage == 2)
                    {
                        alpha = 1 - alpha;
                    }
                    textObject.color = new Color(1, 1, 1, alpha);
                }
            }
        }
        #endregion
    }
    
}

