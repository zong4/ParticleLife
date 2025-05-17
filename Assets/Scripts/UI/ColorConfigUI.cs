using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Components;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ColorConfigUI : MonoBehaviour
    {
        public List<Color> colors;
        private int _virtualColorCount;

        public GameObject colorPicker;
        public int pickerIndex = -1;

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

        public float[,] AttractionMatrix;

        private static EntityManager _entityManager;
        private static Entity _colorConfig;

        private void Awake()
        {
            colorPicker.SetActive(false);
        }

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _colorConfig = _entityManager.CreateEntityQuery(typeof(ColorConfigComponent)).GetSingletonEntity();

            _virtualColorCount = colors.Count + 2;
            InitGameObjects();
            UpdateTransforms();
            SetAttractionMatrix();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                pickerIndex = -1;
                colorPicker.SetActive(false);
            }
        }

        private void InitGameObjects()
        {
            var oldMatrix = AttractionMatrix;
            AttractionMatrix = new float[colors.Count, colors.Count];

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
                            btn.GetComponent<Image>().color = colors[r + c - 1];
                            btn.GetComponent<ColorButton>().colorIndex = r + c - 1;
                            btn.GetComponent<ColorButton>().colorConfigUI = this;
                            _colorButtons.Add(btn);
                        }

                        continue;
                    }

                    if (r < _virtualColorCount - 1 && c < _virtualColorCount - 1)
                    {
                        var i = r - 1;
                        var j = c - 1;

                        var go = Instantiate(inputField, transform);
                        var input = go.GetComponent<MatrixInputField>();
                        input.row = i;
                        input.column = j;
                        input.colorConfigUI = this;
                        _inputFields.Add(go);

                        if (oldMatrix != null && oldMatrix.GetLength(0) > i && oldMatrix.GetLength(0) > j)
                        {
                            AttractionMatrix[i, j] = oldMatrix[i, j];
                        }
                        else
                        {
                            AttractionMatrix[i, j] = i == j ? 1.0f : 0.0f;
                        }
                    }
                    else
                    {
                        _emptyRectTransforms.Add(Instantiate(emptyRectTransform, transform));
                    }
                }
            }

            UpdateInputFields();
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

        public void PickColor(Color color)
        {
            if (pickerIndex == -1) return;

            colors[pickerIndex] = color;
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
            SetColorCount();
            SetAttractionMatrix();
            StartCoroutine(DelayedLayoutUpdate());
        }

        private IEnumerator DelayedLayoutUpdate()
        {
            yield return null;
            UpdateTransforms();
        }

        // ReSharper disable once UnusedMember.Local
        private void ExpandIdentityMatrix(float[,] oldMatrix = null)
        {
            var oldColorCount = 0;
            if (oldMatrix != null)
                oldColorCount = oldMatrix.GetLength(0);

            for (var i = 0; i < colors.Count; i++)
            {
                for (var j = 0; j < colors.Count; j++)
                {
                    if (oldMatrix != null && oldColorCount > i && oldColorCount > j)
                    {
                        AttractionMatrix[i, j] = oldMatrix[i, j];
                    }
                    else
                    {
                        AttractionMatrix[i, j] = i == j ? 1.0f : 0.0f;
                    }
                }
            }
        }

        public void GenerateIdentityMatrix()
        {
            AttractionMatrix = new float[colors.Count, colors.Count];
            for (var i = 0; i < colors.Count; i++)
            {
                for (var j = 0; j < colors.Count; j++)
                {
                    AttractionMatrix[i, j] = i == j ? 1.0f : 0.0f;
                }
            }

            UpdateInputFields();
        }

        public void GenerateRandomMatrix()
        {
            AttractionMatrix = new float[colors.Count, colors.Count];
            for (var i = 0; i < colors.Count; i++)
            {
                for (var j = 0; j < colors.Count; j++)
                {
                    AttractionMatrix[i, j] = Random.Range(-1f, 1f);
                }
            }

            UpdateInputFields();
        }

        private void UpdateInputFields()
        {
            foreach (var input in _inputFields)
            {
                var matrixIndex = input.GetComponent<MatrixInputField>();
                input.GetComponent<TMP_InputField>().text = AttractionMatrix[matrixIndex.row, matrixIndex.column]
                    .ToString(CultureInfo.InvariantCulture);
            }
        }

        private void SetColorCount()
        {
            var data = _entityManager.GetComponentData<ColorConfigComponent>(_colorConfig);
            data.ColorCount = colors.Count;
            _entityManager.SetComponentData(_colorConfig, data);
        }

        public void SetAttractionMatrix()
        {
            var newBlob = CreateAttractionMatrixBlob();

            var data = _entityManager.GetComponentData<ColorConfigComponent>(_colorConfig);

            // if (data.AttractionMatrix.IsCreated)
            //     data.AttractionMatrix.Dispose();

            data.AttractionMatrix = newBlob;
            data.ColorCount = colors.Count;
            _entityManager.SetComponentData(_colorConfig, data);

            // newBlob.Dispose();
        }

        private BlobAssetReference<AttractionMatrixBlob> CreateAttractionMatrixBlob()
        {
            var size = AttractionMatrix.GetLength(0);
            var total = size * size;

            var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<AttractionMatrixBlob>();

            var array = builder.Allocate(ref root.Matrix, total);
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    array[i * size + j] = AttractionMatrix[i, j];
                }
            }

            var blobAsset = builder.CreateBlobAssetReference<AttractionMatrixBlob>(Allocator.Temp);
            builder.Dispose();

            return blobAsset;
        }

        public void SetDelButtonInteractable(bool interactable)
        {
            _deleteButton.GetComponent<Button>().interactable = interactable;
        }
    }
}