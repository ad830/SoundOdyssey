using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class SpawnGalaxy : MonoBehaviour
    {

        public GameObject solarSystemPrefab;
        public GameObject planetPrefab;
        public GameObject songPrefab;
        public LevelInfoUI levelInfo;
        [SerializeField]
        WarpToSong songWarp;

        //public SolarSystemParameters[] systemData;

        public List<SolarSystemParameters> systemData;
        public int howManyRandom = 750;
        public float galaxyRadius = 750f;

        private List<GameObject> galaxySongs;

        #region 
        public GameObject[] GalaxySongs
        {
            get
            {
                return galaxySongs.ToArray();
            }
        }
        #endregion

        #region Public Data Structs

        [System.Serializable]
        public class PlanetLevelData
        {
            public string name;
            public string midiFileName;
            public float scale;
            public float orbitDistance;
            public float orbitSpeed;
            public Color colour;
        }

        [System.Serializable]
        public class SolarSystemParameters
        {
            public string name;
            public int id;
            public PlanetLevelData[] songLevels;
        }
        #endregion

        #region Spawning Methods
        //GameObject SpawnSong(SongLevelData data, string orbitEntityName)
        //{
        //    GameObject song = Instantiate(songPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        //    song.name = data.name;

        //    OrbitBehaviour behaviour = song.GetComponent<OrbitBehaviour>();
        //    behaviour.colour = Color.gray;
        //    behaviour.direction = OrbitBehaviour.OrbitDirection.Anticlockwise;
        //    behaviour.distance = 25f;
        //    behaviour.size = 0.25f;
        //    behaviour.speed = 0.25f;
        //    behaviour.orbitObjectName = orbitEntityName;

        //    TextMesh label = song.GetComponentInChildren<TextMesh>();
        //    label.text = data.name;

        //    return song;
        //}

        GameObject SpawnPlanet(PlanetLevelData data, string orbitEntityName)
        {
            GameObject planet = Instantiate(planetPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            planet.name = data.name;

            PlanetButton button = planet.AddComponent<PlanetButton>();
            button.levelPanel = levelInfo;
            button.callback = () =>
            {
                Debug.LogFormat("Set level panel for {0}", data.midiFileName);
                levelInfo.SetupGalaxySong(data.name, data.midiFileName);
            };

            OrbitBehaviour behaviour = planet.GetComponent<OrbitBehaviour>();
            behaviour.colour = data.colour;
            behaviour.direction = OrbitBehaviour.OrbitDirection.Clockwise;
            behaviour.distance = data.orbitDistance;
            behaviour.size = data.scale;
            behaviour.speed = data.orbitSpeed;
            behaviour.orbitObjectName = orbitEntityName;

            TextMesh label = planet.GetComponentInChildren<TextMesh>();
            label.text = data.name;
            //label.color = Color.clear;

            return planet;
        }

        GameObject SpawnSolarSystem(SolarSystemParameters systemParams, string galaxyCenterObjectName, float angleTier, float distanceTier, bool isGalaxySong = false)
        {
            if (distanceTier < 30f)
            {
                return null;
            }

            GameObject system = Instantiate(solarSystemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            system.name = systemParams.name;

            OrbitBehaviour behaviour = system.AddComponent<OrbitBehaviour>();
            behaviour.size = 5f;
            behaviour.colour = GetRandomColour();
            behaviour.direction = OrbitBehaviour.OrbitDirection.Clockwise;
            behaviour.speed = 0.005f;//0.03125f;
            behaviour.orbitObjectName = galaxyCenterObjectName;
            //behaviour.distance = Random.RandomRange(50f, 100f);
            behaviour.distance = distanceTier;
            behaviour.startAngle = angleTier;


            // spawn planets
            foreach (PlanetLevelData planetData in systemParams.songLevels)
            {
                GameObject planet = SpawnPlanet(planetData, system.name);

                planet.transform.SetParent(system.transform);

                if (isGalaxySong)
                {
                    galaxySongs.Add(planet);
                }
            }

            // spawn songs
            //foreach (SongLevelData songData in systemParams.songLevels)
            //{
            //    GameObject song = SpawnSong(songData, system.name);

            //    song.transform.SetParent(system.transform);
            //}

            return system;
        }
        #endregion

        static string GetRandomName()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length = Random.Range(1, 16);
            StringBuilder str = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                str.Append(chars[Random.Range(0, chars.Length - 1)]);
            }
            return str.ToString();
        }

        static Color GetRandomColour()
        {
            return new Color(Random.value, Random.value, Random.value);
        }

        // Use this for initialization
        void Start()
        {
            //systemData = new SolarSystemParameters[howManyRandom];
            //for (int i = 0; i < systemData.Length; i++)
            //{
            //    systemData[i] = new SolarSystemParameters();
            //    systemData[i].id = i;
            //    systemData[i].name = "Galaxy " + i;
            //    systemData[i].songLevels = new SongLevelData[Random.Range(0, 1)];
            //    for (int a = 0; a < systemData[i].songLevels.Length; a++)
            //    {
            //        systemData[i].songLevels[a] = new SongLevelData();
            //        systemData[i].songLevels[a].name = "Song " + a;
            //    }
            //    systemData[i].teachingLevels = new PlanetLevelData[Random.Range(1, 2)];
            //    for (int b = 0; b < systemData[i].teachingLevels.Length; b++)
            //    {
            //        systemData[i].teachingLevels[b] = new PlanetLevelData();
            //        systemData[i].teachingLevels[b].colour = GetRandomColour();
            //        //var name = GetRandomName();
            //        var name = "";
            //        systemData[i].teachingLevels[b].description = "You will learn about the concept of " + name;
            //        systemData[i].teachingLevels[b].name = name;
            //        systemData[i].teachingLevels[b].orbitDistance = Random.Range(5f, 10f);
            //        systemData[i].teachingLevels[b].orbitSpeed = Random.Range(0.25f, 0.6f);
            //        systemData[i].teachingLevels[b].scale = Random.Range(0.25f, 0.4f);
            //    }

            //}

            int midiFilesCount = MidiResourceHolder.Instance.MidiFileCount;

            systemData = new List<SolarSystemParameters>();

            
            //systemData[0] = new SolarSystemParameters();
            //systemData[0].id = 0;
            //systemData[0].name = "Galaxy " + 0;
            //systemData[0].songLevels = new PlanetLevelData[midiFilesCount];

            Debug.LogFormat("midiFilesCount: {0}", midiFilesCount);
            int actuallySpawnedMidi = 0;
            galaxySongs = new List<GameObject>(midiFilesCount);
            for (int b = 0; b < midiFilesCount; b++)
            {
                //int planetCount = Random.Range(1, Mathf.Min(4, midiFilesCount - b));
                int planetCount = 1;

                SolarSystemParameters tempParams = new SolarSystemParameters();
                tempParams.id = systemData.Count;
                tempParams.name = "Galaxy " + tempParams.id;
                tempParams.songLevels = new PlanetLevelData[planetCount];

                //for(int i = 0; i < planetCount && b < midiFilesCount; i++, b++)
                for (int i = 0; i < planetCount; i++)
                {
                    tempParams.songLevels[i] = new PlanetLevelData();
                    //Debug.LogFormat("i {0} b {1}", i, b);
                    var name = MidiResourceHolder.Instance.GetMidiAt(b).name;
                    tempParams.songLevels[i].midiFileName = name;
                    tempParams.songLevels[i].name = name.Substring(0, name.IndexOf("."));

                    tempParams.songLevels[i].colour = GetRandomColour();
                    tempParams.songLevels[i].orbitDistance = Random.Range(5f, 10f);
                    tempParams.songLevels[i].orbitSpeed = Random.Range(0.025f, 0.05f);//Random.Range(0.125f, 0.3f);
                    tempParams.songLevels[i].scale = Random.Range(0.25f, 0.4f);
                }

                systemData.Add(tempParams);
                actuallySpawnedMidi++;

                //systemData[0].songLevels[b] = new PlanetLevelData();
                //systemData[0].songLevels[b].colour = GetRandomColour();
                ////var name = GetRandomName();
                //var name = MidiResourceHolder.Instance.GetMidiAt(b).name;
                //systemData[0].songLevels[b].description = "You will play " + name;
                //systemData[0].songLevels[b].name = name;
                //systemData[0].songLevels[b].orbitDistance = Random.Range(5f, 10f);
                //systemData[0].songLevels[b].orbitSpeed = Random.Range(0.25f, 0.6f);
                //systemData[0].songLevels[b].scale = Random.Range(0.25f, 0.4f);
            }
            Debug.LogFormat("Matches {0} actual count {1}", midiFilesCount == actuallySpawnedMidi, actuallySpawnedMidi);

            //float angle = 0f;
            //float distance = 30f;
            /*
            float distanceOffset = 30f;
            float angleOffset = 7.5f;//7.5f;

            int currentSystem = 0;
            int systemsPerTier = 6;

            float angleSlice = 360f / systemsPerTier;

            int tierLevel = 0;
            List<float> angleList = new List<float>();
            */

            Spawn(systemData);

            Debug.Log("Spawned the galaxy?");
            songWarp.PopulatePlanetList(galaxySongs.ToArray());
        }

        private void Spawn(List<SolarSystemParameters> galaxyData)
        {
            const int numArms = 5;
            const float armSeparationDistance = 2 * Mathf.PI / numArms;
            const float armOffsetMax = 0.5f;
            const float rotationFactor = 5;
            //const float randomOffsetXY = 0.02f;
            //const float galaxyRadius = 750f;

            foreach (SolarSystemParameters data in galaxyData)
            {
                // Choose a distance from the center of the galaxy.
                float distance = Random.value;
                distance = Mathf.Pow(distance, 2);

                // Choose an angle between 0 and 2 * PI.
                float angle = Random.value * 2 * Mathf.PI;
                float armOffset = Random.value * armOffsetMax;
                armOffset = armOffset - armOffsetMax / 2;
                armOffset = armOffset * (1 / distance);

                float squaredArmOffset = Mathf.Pow(armOffset, 2);
                if (armOffset < 0)
                    squaredArmOffset = squaredArmOffset * -1;
                armOffset = squaredArmOffset;

                float rotation = distance * rotationFactor;

                angle = (int)(angle / armSeparationDistance) * armSeparationDistance + armOffset + rotation;

                // Convert polar coordinates to 2D cartesian coordinates.
                //float starX = Mathf.Cos(angle) * distance * galaxyRadius;
                //float starY = Mathf.Sin(angle) * distance * galaxyRadius;
                float distanceTier = distance * galaxyRadius;
                distanceTier = Mathf.Max(distanceTier, 30f);
                GameObject system = SpawnSolarSystem(data, gameObject.name, angle * Mathf.Rad2Deg, distanceTier, true);
                /*
                GameObject system = SpawnSolarSystem(data, gameObject.name, angle, distance);

                if (currentSystem < systemsPerTier - 1)
                {
                    // 0 1 2 3 4 5
                    angleList.Add(angle);
                    currentSystem++;
                }
                else
                {

                    string str = "";
                    foreach (var a in angleList)
                    {
                        str += a + ", ";
                    }
                    Debug.Log(str);
                    angleList.Clear();

                    tierLevel++;
                    currentSystem = 0;
                    distance += distanceOffset;
                }

                angle += angleSlice;
                if (angle >= 360f)
                {
                    angle -= 360f;
                    angle += angleOffset;
                }
                */
                if (system != null)
                {
                    system.transform.SetParent(this.transform);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

