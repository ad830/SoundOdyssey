using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class WarpToSong : MonoBehaviour
    {
        // References
        [SerializeField]
        SolarMenuCamera galaxyCam;
        [SerializeField]
        private Animator anim;
        [SerializeField]
        private GameObject topper;
        [SerializeField]
        private GameObject planetList;
        [SerializeField]
        private GameObject galaxySongButtonPrefab;

        private GameObject heading;
        private GameObject search;
        private InputField searchField;
        private GameObject clearButtonObj;
        private List<GameObject> planetResults;

        private void ResetAllChildren(Transform trans)
        {
            int childCount = trans.childCount;
            for (int i = 0; i < childCount; i++)
			{
			    Destroy(trans.GetChild(i).gameObject);
			}
        }

        private void SetupTopper()
        {
            Button action = topper.GetComponent<Button>();
            action.onClick.RemoveAllListeners();
            action.onClick.AddListener(() =>
            {
                if (planetList.activeInHierarchy)
                {
                    HidePlanetList();
                }
                else
                {
                    ShowPlanetList();
                }
            });
            Transform inner = topper.transform.GetChild(0);
            heading = inner.Find("Heading").gameObject;
            search = inner.Find("SearchField").gameObject;
            searchField = search.GetComponent<InputField>();
            searchField.onValueChange.AddListener(FilterResults);
            clearButtonObj = search.transform.Find("ClearButton").gameObject;
            Button clearButton = clearButtonObj.GetComponent<Button>();
            clearButton.onClick.AddListener(ClearSearchFilter);
            clearButtonObj.SetActive(false);
        }

        private void FilterResults(string searchInput)
        {
            bool emptyInput = searchInput == "";
            clearButtonObj.SetActive(!emptyInput);
            foreach (GameObject result in planetResults)
            {
                bool showResult = false;
                if (emptyInput)
                {
                    showResult = true;
                }
                else
                {
                    //showResult = result.name.Contains(searchInput);
                    showResult = result.name.ToUpperInvariant().Contains(searchInput.ToUpperInvariant());
                }
                result.SetActive(showResult);
            }
        }
        
        public void ShowPlanetList()
        {
            //planetList.SetActive(true);
            heading.SetActive(false);
            search.SetActive(true);
            anim.SetBool("isOpen", true);

            // focus on the search field
            searchField.Select();
        }

        public void HidePlanetList()
        {
            //planetList.SetActive(false);
            heading.SetActive(true);
            search.SetActive(false);
            anim.SetBool("isOpen", false);
        }

        public void ClearSearchFilter()
        {
            searchField.text = "";
            clearButtonObj.SetActive(false);

            // focus on the search field
            searchField.Select();
        }

        public void PopulatePlanetList(GameObject[] planets)
        {
            Debug.LogFormat("populating planet list - size {0}", planets.Length);
            Transform listNode = planetList.transform.Find("Inner");
            ResetAllChildren(listNode);
            planetResults = new List<GameObject>(planets.Length);
            foreach (GameObject planetObj in planets)
            {
                // create prefabbed button
                GameObject button = Instantiate(galaxySongButtonPrefab) as GameObject;
                button.transform.SetParent(listNode);
                button.name = string.Format("Warp -> {0}", planetObj.name);
                
                // set button to go to planet
                PlanetButton planetButton = planetObj.GetComponent<PlanetButton>();
                Button buttonAction = button.GetComponent<Button>();
                buttonAction.onClick.AddListener(() => 
                {
                    if (!planetButton.Selected)
                    {
                        planetButton.Select();
                    }
                    HidePlanetList();
                });

                // set label
                Text label = button.transform.GetChild(0).GetComponent<Text>();
                label.text = planetObj.name;

                // save reference to button
                planetResults.Add(button);
            }
        }

        // Use this for initialization
        void Start()
        {
            SetupTopper();
            HidePlanetList();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    
}
