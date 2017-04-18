using UnityEngine;
using System.Collections;
using SoundOdyssey;


namespace SoundOdyssey
{
    public class SpawnPlanets : MonoBehaviour
    {
        string[] planetNames = {
                                "Rhythm",
                                "Pitch",
                                "Duration",
                                "Notation"
                           };
        #region Public properties
        public int numberOfPlanets = 4;
        public float minDistance = 3;
        public float maxDistance = 6;
        public float minSpeed = 0.5f;
        public float maxSpeed = 2.5f;
        public float minSize = 1f;
        public float maxSize = 3f;
        public int minNumMoons = 0;
        public int maxNumMoons = 3;
        public Color spawnColour = Color.white;
        public GameObject planetPrefab;

        public string[] PlanetNames
        {
            get
            {
                return planetNames;
            }
        }
        #endregion

        private GameObject SpawnPlanet(string orbitObjectName, string name, bool isButton = true)
        {
            var planet = Instantiate(planetPrefab, Vector2.zero, Quaternion.identity) as GameObject;

            if (isButton)
            {
                planet.AddComponent<PlanetButton>();
            }

            var planetBehaviour = planet.GetComponent<OrbitBehaviour>();
            planetBehaviour.distance = Random.value * (maxDistance - minDistance) + minDistance;
            planetBehaviour.speed = Random.value * (maxSpeed - minSpeed) + minSpeed;
            planetBehaviour.size = Random.value * (maxSize - minSize) + minSize;
            planetBehaviour.orbitObjectName = orbitObjectName;
            planetBehaviour.direction = (Random.Range(0, 2) == 0 ? OrbitBehaviour.OrbitDirection.Clockwise : OrbitBehaviour.OrbitDirection.Anticlockwise);
            //planetBehaviour.colour = spawnColour;
            planetBehaviour.colour = new Color(Random.value, Random.value, Random.value);

            var label = planet.GetComponentInChildren<TextMesh>();
            label.text = name;
            planet.transform.SetParent(transform);
            planet.name = name;
            return planet;
        }

        // Use this for initialization
        void Start()
        {
            //GameObject sun = GameObject.Find("Sun");

            for (int i = 0; i < numberOfPlanets; i++)
            {
                string name = "Default - " + i.ToString();
                if (i < planetNames.Length)
                {
                    name = planetNames[i];
                }
                SpawnPlanet("Sun", name);

                /*
                int moonCount = Random.Range(minNumMoons, maxNumMoons);
                for (int a = 0; a < moonCount; a++)
                {
                    var moon = SpawnPlanet(name, "MOON", false);
                    moon.GetComponent<OrbitBehaviour>().distance *= 0.25f;
                    moon.GetComponent<OrbitBehaviour>().size *= 0.5f;
                }
                */
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
