using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SoundOdyssey;


namespace SoundOdyssey
{
    public class SolarMenuCamera : MonoBehaviour
    {

        Vector3 startPosition;
        Vector3 galaxyPosition = Vector3.zero;
        GameObject target;

        float startOrthoSize;
        float lastScrollDelta = 0f;
        Vector3 transitionSpeed;

        public float targetLerpSpeed = 2f;
        public float zoomedOrthographicSize = 5;

        float galaxyZoom = 500f;
        bool useGalaxyZoom = false;

        Vector3 mouseVel;
        bool mouseControlsCamera;

        Camera cameraRef;
        Transform baseCamTransform;
        float galaxyRadius;
        Vector3 galaxyOrigin;

        [SerializeField]
        GameObject galaxy;
        List<TextMesh> planetLabels;
        float fontSizeTimer = 0;
        [SerializeField]
        int minFontSize = 20;
        [SerializeField]
        int maxFontSize = 128;

        public bool HasTarget
        {
            get 
            {
                return target != null;
            }
        }

        // Use this for initialization
        void Start()
        {
            cameraRef = GetComponent<Camera>();
            startOrthoSize = cameraRef.orthographicSize;
            startPosition = transform.position;
            galaxyPosition = new Vector3(startPosition.x, startPosition.y, -cameraRef.farClipPlane);
            target = null;

            baseCamTransform = GameObject.Find("BaseCam").transform;

            GameObject galaxy = GameObject.Find("Galaxy");
            galaxyRadius = galaxy.GetComponent<SpawnGalaxy>().galaxyRadius;
            galaxyOrigin = galaxy.GetComponent<Transform>().position;

            mouseVel = Vector3.zero;
            mouseControlsCamera = false;

            planetLabels = new List<TextMesh>();
            int systemCount = galaxy.transform.childCount - 1;
            for (int i = 0; i < systemCount; i++)
            {
                Transform systemTrans = galaxy.transform.GetChild(i);
                int planetCount = systemTrans.childCount;
                for (int a = 2; a < planetCount; a++)
                {
                    Transform planetTrans = systemTrans.GetChild(a);
                    planetLabels.Add(planetTrans.GetComponentInChildren<TextMesh>());
                }
            }
        }

        void Update()
        {
            if (target != null) { return; }

            Vector3 deltaMouse = Vector3.zero;
            if (mouseControlsCamera)
            {
                if (Input.GetMouseButton(0))
                {
                    deltaMouse = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
                    deltaMouse *= -base.transform.position.z;       // further zoomed out you are, the more it moves
                }
            }


            if (deltaMouse.magnitude != 0)
            {
                mouseVel += deltaMouse * Time.deltaTime * 10f;
            }
            else
            {
                mouseVel -= mouseVel * Time.deltaTime;
            }

            float mouseVelMagnitude = mouseVel.magnitude;
            if (mouseVelMagnitude > 10f)
            {
                mouseVel.Normalize();
                mouseVel *= 10f;
            }
            else if (mouseVelMagnitude < 0.01f)
            {
                mouseVel = Vector3.zero;
            }

            base.transform.position += mouseVel * Time.deltaTime * 10;
            /*
            if (Input.GetMouseButton(0))
            {
                base.transform.position += deltaMouse * Time.deltaTime;
            }
            */
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (target != null)
            {
                Vector3 distance = target.transform.position - base.transform.position;
                //Debug.LogFormat("SQRMag {0}", distance.sqrMagnitude);
                //Debug.LogFormat("mag {0}", distance.magnitude);
                //Debug.LogFormat("base {0} target {1}", base.transform.position, target.transform.position);
                if (distance.sqrMagnitude > 26f * 26f)
                {
                    base.transform.position = Vector3.Lerp(base.transform.position, target.transform.position, targetLerpSpeed * Time.deltaTime);
                }
                else if (!base.transform.parent != target.transform)
                {
                    base.transform.SetParent(target.transform, true);
                }
            }

            float zpos = base.transform.position.z;
            //float scrollOrth = cameraRef.orthographicSize;
            float scrollDelta = 0;
            if (mouseControlsCamera)
            {
                scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            }
            /*
            lastScrollDelta += scrollDelta;
            scrollOrth += lastScrollDelta;
            lastScrollDelta *= 0.99f;
            if (scrollOrth > galaxyZoom)
            {
                scrollOrth = galaxyZoom;
                lastScrollDelta = 0;
            }
            if (scrollOrth < startOrthoSize)
            {
                scrollOrth = startOrthoSize;
                lastScrollDelta = 0;
            }
            GetComponent<Camera>().orthographicSize = scrollOrth;
            */
            if (target == null)
            {
                lastScrollDelta += scrollDelta;
                zpos += lastScrollDelta;
                lastScrollDelta *= 0.99f;
            }
            if (zpos < galaxyPosition.z)
            {
                zpos = galaxyPosition.z;
                lastScrollDelta = 0;
            }
            if (zpos > startPosition.z)
            {
                zpos = startPosition.z;
                lastScrollDelta = 0;
            }
            base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, zpos);

            // constrain camera position to the bounds of the galaxy circle
            Vector3 distanceFromOrigin = base.transform.position - galaxyOrigin;
            base.transform.position = galaxyOrigin + Vector3.ClampMagnitude(distanceFromOrigin, galaxyRadius);

            fontSizeTimer += Time.deltaTime;
            if (fontSizeTimer > 0.5f)
            {
                fontSizeTimer = 0;

                float absZ = Mathf.Abs(zpos);
                float absZoomMin = Mathf.Abs(startPosition.z);
                float absZoomMax = Mathf.Abs(galaxyPosition.z);
                float absZoomRatio = absZ / (absZoomMax - absZoomMin);
                int fontSize = (int)((absZoomRatio * (maxFontSize - minFontSize)) + minFontSize);

                foreach (var label in planetLabels)
                {
                    label.fontSize = fontSize;
                }
            }
        }

        public void DisableMouseControl()
        {
            mouseControlsCamera = false;
        }
        public void EnableMouseControl()
        {
            mouseControlsCamera = true;
        }

        public void LerpToTarget(GameObject _target)
        {
            if (target != null)
            {
                // un select previous target
                target.GetComponent<PlanetButton>().Deselect();
            }
            target = _target;
            mouseVel = Vector3.zero;
            //base.transform.SetParent(target.transform, true);
        }

        public void ResetPosition()
        {
            target = null;

            base.transform.SetParent(baseCamTransform, true);
            base.transform.localScale = baseCamTransform.localScale;
        }

        public void ToggleLerp(GameObject _target)
        {
            if (target == null || target != _target)
            {
                LerpToTarget(_target);
            }
            else
            {
                ResetPosition();
            }
        }
    }
}

