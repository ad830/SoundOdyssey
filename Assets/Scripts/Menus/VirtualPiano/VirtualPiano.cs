using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SoundOdyssey;
using MidiJack;
using Midi;

namespace SoundOdyssey
{
    public class VirtualPiano : MonoBehaviour
    {
        public enum KeyRange {Range_61, Range_76, Range_88};

        [SerializeField]
        private KeyRange keyRange = KeyRange.Range_61;
        [SerializeField]
        private GameObject whiteKeyPrefab;
        [SerializeField]
        private GameObject blackKeyPrefab;

        private bool isOn = true;
        private int minKey;
        private int maxKey;

        public void AlterKeyRange(KeyRange nextRange)
        {
            keyRange = nextRange;
            SetMinMaxKeys(keyRange);
            ResetKeys(keyRange);
        }

        private void SetMinMaxKeys(KeyRange range)
        {
            switch(range)
            {
                case KeyRange.Range_61:
                    minKey = 36;
                    maxKey = 96;
                    break;
                case KeyRange.Range_76:
                    minKey = 21;
                    maxKey = 108;
                    break;
                case KeyRange.Range_88:
                    minKey = 21;
                    maxKey = 108;
                    break;
            }
        }
        
        private void RemoveAllChildNodes(Transform node)
        {
            for (int i = 0; i < node.childCount; i++)
            {
                Destroy(node.GetChild(i).gameObject);
            }
        }

        private void ResetKeys(KeyRange range)
        {
            RemoveAllChildNodes(base.transform);

        }
        
        private void PressKey(int i)
        {

        }
        private void LiftKey(int i)
        {

        }

        // Use this for initialization
        void Start()
        {
            AlterKeyRange(keyRange);
        }

        // Update is called once per frame
        void Update()
        {
            if (!isOn) { return; }
            for (int i = 0; i < 128; i++)
            {
                if (i >= minKey && i <= maxKey)
                {
                    float value = MidiMaster.GetKey(i);
                    if (value > 0f)
                    {
                        // key is being pressed
                        PressKey(i);
                    }
                    else
                    {
                        // key is lifted
                        LiftKey(i);
                    }
                }
            }
        }
    }    
}
