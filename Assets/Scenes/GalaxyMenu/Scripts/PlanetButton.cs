using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using SoundOdyssey;


namespace SoundOdyssey
{

    public class PlanetButton : MonoBehaviour
    {


        public UnityAction callback;
        public LevelInfoUI levelPanel;

        float scale = 1f;
        float originalScale = 1f;
        bool clicked;
        Color originalColour;
        Color highlightColour;
        Color pressedColour;
        TextMesh label;

        SolarMenuCamera cam;
        MeshRenderer meshRenderer;
        GameObject selectedSprite;

        public bool Selected
        {
            get { return clicked; }
        }

        public void ToggleSelect()
        {
            Debug.Log(gameObject.name);
            scale *= 2f;

            // zoom the camera to this object
            cam.ToggleLerp(gameObject);
            // toggle clicked state
            clicked = !clicked;
            SetSelect(clicked);

            if (clicked)
            {
                callback();
            }
        }

        public void Select()
        {
            SetSelect(true);
            cam.LerpToTarget(gameObject);
            callback();
        }
        public void Deselect()
        {
            SetSelect(false);
        }

        // Use this for initialization
        void Start()
        {
            label = GetComponentInChildren<TextMesh>();
            originalScale = transform.localScale.x;
            scale = originalScale;
            clicked = false;

            originalColour = GetComponent<OrbitBehaviour>().colour;
            Color colour = new Color(originalColour.r, originalColour.g, originalColour.b);
            colour.r *= 1.5f;
            colour.g *= 1.5f;
            colour.b *= 1.5f;
            highlightColour = colour;
            colour.r *= 0.8f;
            colour.g *= 0.8f;
            colour.b *= 0.8f;
            pressedColour = colour;

            cam = GameObject.Find("Main Camera").GetComponent<SolarMenuCamera>();
            meshRenderer = GetComponent<MeshRenderer>();
            selectedSprite = base.transform.Find("Selected").gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            scale = originalScale + (scale - originalScale) * Mathf.Exp(-20.0f * Time.deltaTime);
            transform.localScale = Vector3.one * scale;
        }

        void OnMouseDown()
        {
            ToggleSelect();
        }
        void OnMouseEnter()
        {
            meshRenderer.material.color = highlightColour;
            //label.color = Color.white;

        }
        void OnMouseExit()
        {
            meshRenderer.material.color = originalColour;
            //label.color = Color.clear;

        }

        public void SetSelect(bool status)
        {
            clicked = status;
            selectedSprite.SetActive(status);
        }
    }
}
