using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class GalaxyUI : MonoBehaviour
    {

        [SerializeField]
        private GameObject profilePanel;

        // Use this for initialization
        void Start() 
        {
            Text profileText = profilePanel.transform.GetChild(0).GetChild(0).GetComponent<Text>();
            string profileFormat = "Welcome: {0}";
            profileText.text = string.Format(profileFormat, GameDirector.Instance.CurrentProfile.Name);
	    }

        // Update is called once per frame
        void Update()
        {

        }
    }
}