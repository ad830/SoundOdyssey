using UnityEngine;
using System.Collections;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class SpawnStaves : MonoBehaviour
    {
        public GameObject stavePrefab;
        public float radius = 14f;

        private GameObject[] staves;

        // Use this for initialization
        void Start()
        {
            staves = new GameObject[10];
            int staveIdx = 0;

            Vector3 baseTrebleStave = base.transform.position + 1f * Vector3.down;
            //Vector3 baseBassStave = base.transform.position + 1f * Vector3.down;
            Vector3 position = Vector3.zero;

            // spawn treble staves
            for (int i = 0; i < 5; i++)
            {
                position = baseTrebleStave + (i * 1f * Vector3.up);
                position.y += 1f;
                GameObject obj = Instantiate(stavePrefab, position, Quaternion.identity) as GameObject;
                obj.transform.SetParent(base.transform);
                LineRenderer line = obj.GetComponent<LineRenderer>();
                line.SetVertexCount(2);
                line.SetPosition(0, obj.transform.position + (radius * Vector3.left));
                line.SetPosition(1, obj.transform.position + (radius * Vector3.right));
                staves[staveIdx++] = obj;
            }

            // spawn bass staves
            for (int i = 0; i < 5; i++)
            {
                position = baseTrebleStave + (i * 1f * Vector3.down);
                position.y -= 1f;
                GameObject obj = Instantiate(stavePrefab, position, Quaternion.identity) as GameObject;
                obj.transform.SetParent(base.transform);
                LineRenderer line = obj.GetComponent<LineRenderer>();
                line.SetVertexCount(2);
                line.SetPosition(0, obj.transform.position + (radius * Vector3.left));
                line.SetPosition(1, obj.transform.position + (radius * Vector3.right));
                staves[staveIdx++] = obj;
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void HideStaves()
        {
            for (int i = 0; i < staves.Length; i++)
            {
                staves[i].SetActive(false);
            }
        }
    }    
}
