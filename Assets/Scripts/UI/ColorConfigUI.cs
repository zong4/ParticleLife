using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ColorConfigUI : MonoBehaviour
    {
        public List<Color> colors;
        private int _virtualColorCount;

        public GameObject colorPicker;
        private int _pickerIndex = -1;

        public GameObject title;
        public GameObject addButton;
        public GameObject deleteButton;
        public GameObject colorButton;
        public GameObject inputField;
        public GameObject emptyRectTransform;

        private GameObject _title;
        private GameObject _addButton;
        private GameObject _deleteButton;
        private List<GameObject> _colorButtons;
        private List<GameObject> _inputFields;
        private List<GameObject> _emptyRectTransforms;

        private float[,] _attractionMatrix;

        private void Awake()
        {
            colorPicker.SetActive(false);

            _virtualColorCount = colors.Count + 2;
            InitGameObjects();
            UpdateTransforms();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _pickerIndex = -1;
                colorPicker.SetActive(false);
            }
        }

        private void InitGameObjects()
        {
            _colorButtons = new List<GameObject>();
            _inputFields = new List<GameObject>();
            _emptyRectTransforms = new List<GameObject>();
            for (var r = 0; r < _virtualColorCount; r++)
            {
                for (var c = 0; c < _virtualColorCount; c++)
                {
                    if (r == 0 && c == 0)
                    {
                        _title = Instantiate(title, transform);
                        continue;
                    }

                    if (r == 0 || c == 0)
                    {
                        if (r == _virtualColorCount - 1)
                        {
                            _deleteButton = Instantiate(deleteButton, transform);
                            _deleteButton.GetComponent<Button>().onClick.AddListener(DelColor);
                        }
                        else if (c == _virtualColorCount - 1)
                        {
                            _addButton = Instantiate(addButton, transform);
                            _addButton.GetComponent<Button>().onClick.AddListener(AddColor);
                        }
                        else
                        {
                            var btn = Instantiate(colorButton, transform);
                            btn.GetComponent<ColorButton>().colorIndex = r + c - 1;
                            btn.GetComponent<Image>().color = colors[r + c - 1];
                            btn.GetComponent<Button>().onClick.AddListener(() => EnterPickColor(btn));
                            _colorButtons.Add(btn);
                        }

                        continue;
                    }

                    if (r < _virtualColorCount - 1 && c < _virtualColorCount - 1)
                    {
                        _inputFields.Add(Instantiate(inputField, transform));
                    }
                    else
                    {
                        _emptyRectTransforms.Add(Instantiate(emptyRectTransform, transform));
                    }
                }
            }
        }

        [ContextMenu("Update Transforms")]
        public void UpdateTransforms()
        {
            for (var i = 0; i < _virtualColorCount; i++)
            {
                for (var j = 0; j < _virtualColorCount; j++)
                {
                    var rectTransform = transform.GetChild(i * _virtualColorCount + j).GetComponent<RectTransform>();
                    rectTransform.anchorMin = new Vector2((float)j / _virtualColorCount,
                        1 - (float)(i + 1) / _virtualColorCount);
                    rectTransform.anchorMax = new Vector2((float)(j + 1) / _virtualColorCount,
                        1 - (float)i / _virtualColorCount);
                }
            }
        }

        private void UpdateColor()
        {
            if (_colorButtons == null) return;
            foreach (var button in _colorButtons)
            {
                button.GetComponent<Image>().color = colors[button.GetComponent<ColorButton>().colorIndex];
            }
        }

        private void Clear()
        {
            if (_title != null) Destroy(_title);
            if (_addButton != null) Destroy(_addButton);
            if (_deleteButton != null) Destroy(_deleteButton);

            foreach (var button in _colorButtons)
            {
                Destroy(button);
            }

            foreach (var input in _inputFields)
            {
                Destroy(input);
            }

            foreach (var empty in _emptyRectTransforms)
            {
                Destroy(empty);
            }

            _colorButtons.Clear();
            _inputFields.Clear();
            _emptyRectTransforms.Clear();
        }

        private void EnterPickColor(GameObject button)
        {
            _pickerIndex = button.GetComponent<ColorButton>().colorIndex;

            colorPicker.SetActive(true);
        }

        public void PickColor(Color color)
        {
            if (_pickerIndex == -1) return;

            colors[_pickerIndex] = color;
            UpdateColor();
        }

        private void AddColor()
        {
            colors.Add(colors[^1]);
            Rebuild();
        }

        private void DelColor()
        {
            if (colors.Count <= 1) return;
            colors.RemoveAt(colors.Count - 1);
            Rebuild();
        }

        private void Rebuild()
        {
            Clear();
            _virtualColorCount = colors.Count + 2;
            InitGameObjects();
            StartCoroutine(DelayedLayoutUpdate());
        }

        private IEnumerator DelayedLayoutUpdate()
        {
            yield return null;
            UpdateTransforms();
        }
    }
}