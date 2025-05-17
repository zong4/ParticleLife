using Components;
using Unity.Entities;
using UnityEngine;

namespace UI
{
    public class ParticleSpawnerUI : MonoBehaviour
    {
        private EntityManager _entityManager;

        private int _count = 1000;

        private bool _isSelectingStart;
        private bool _isSelectingEnd;
        private bool _shouldStartSelectNextFrame;
        private bool _isDrawing;

        private Camera _camera;
        private Vector2 _startWorldPos;
        private Vector2 _endWorldPos;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _camera = Camera.main;
        }

        private void Update()
        {
            if (_shouldStartSelectNextFrame)
            {
                _shouldStartSelectNextFrame = false;
                _isSelectingStart = true;

                _isDrawing = false;
                return;
            }

            if (_isSelectingStart)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    _startWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                    _endWorldPos = _startWorldPos;

                    _isSelectingStart = false;
                    _isSelectingEnd = true;

                    _isDrawing = true;
                    return;
                }
            }

            if (_isSelectingEnd)
            {
                _endWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);

                if (Input.GetMouseButtonUp(0))
                {
                    _isSelectingEnd = false;
                }
            }
        }

        private void OnGUI()
        {
            if (!_isDrawing) return;

            DrawRect.DrawScreenRectBorder(
                DrawRect.GetScreenRect(_camera.WorldToScreenPoint(_startWorldPos),
                    _camera.WorldToScreenPoint(_endWorldPos)), 2, Color.yellow);
        }

        public void SetCount(string str)
        {
            if (!int.TryParse(str, out var result)) return;

            _count = result;
        }

        public void SelectArea()
        {
            _shouldStartSelectNextFrame = true;
        }

        public void Spawn()
        {
            if (!_isDrawing || _isSelectingEnd) return;

            if (_count <= 0)
            {
                Debug.LogError("Count must be greater than 0");
                return;
            }

            var minPosition = new Vector2(Mathf.Min(_startWorldPos.x, _endWorldPos.x),
                Mathf.Min(_startWorldPos.y, _endWorldPos.y));
            var maxPosition = new Vector2(Mathf.Max(_startWorldPos.x, _endWorldPos.x),
                Mathf.Max(_startWorldPos.y, _endWorldPos.y));

            if (minPosition == maxPosition)
            {
                Debug.LogError("Invalid area selected");
                return;
            }

            var request = new ParticleCreateRequestComponent()
            {
                Count = _count, MinPosition = minPosition, MaxPosition = maxPosition
            };
            var e = _entityManager.CreateEntity();
            _entityManager.AddComponentData(e, request);

            _isDrawing = false;
        }
    }
}