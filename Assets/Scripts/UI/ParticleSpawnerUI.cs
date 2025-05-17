using Components;
using Unity.Entities;
using UnityEngine;

namespace UI
{
    public class ParticleSpawnerUI : MonoBehaviour
    {
        private EntityManager _entityManager;

        private int _count = 1000;
        private Vector2 _minPosition;
        private Vector2 _maxPosition;

        private bool _isSelectingStart;
        private bool _isSelectingEnd;
        private bool _shouldStartSelectNextFrame;

        private Camera _camera;
        private Vector2 _startPos;
        private Vector2 _endPos;
        private Rect _rect;

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
                return;
            }

            if (_isSelectingStart)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    _startPos = Input.mousePosition;
                    _isSelectingStart = false;
                    _isSelectingEnd = true;
                    return;
                }
            }

            if (_isSelectingEnd)
            {
                _endPos = Input.mousePosition;
                _rect = DrawRect.GetScreenRect(_startPos, _endPos);

                if (Input.GetMouseButtonUp(0))
                {
                    var worldStart = _camera.ScreenToWorldPoint(new Vector3(_startPos.x, _startPos.y, 0));
                    var worldEnd = _camera.ScreenToWorldPoint(new Vector3(_endPos.x, _endPos.y, 0));
                    _minPosition = new Vector2(Mathf.Min(worldStart.x, worldEnd.x),
                        Mathf.Min(worldStart.y, worldEnd.y));
                    _maxPosition = new Vector2(Mathf.Max(worldStart.x, worldEnd.x),
                        Mathf.Max(worldStart.y, worldEnd.y));

                    _isSelectingEnd = false;
                }
            }
        }

        private void OnGUI()
        {
            if (_rect == default) return;
            DrawRect.DrawScreenRectBorder(_rect, 2, Color.yellow);
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
            if (_count <= 0)
            {
                Debug.LogError("Count must be greater than 0");
                return;
            }

            if (_minPosition == _maxPosition)
            {
                Debug.LogError("Invalid area selected");
                return;
            }

            var request = new ParticleCreateRequestComponent()
            {
                Count = _count, MinPosition = _minPosition, MaxPosition = _maxPosition
            };
            var e = _entityManager.CreateEntity();
            _entityManager.AddComponentData(e, request);

            _rect = default;
        }
    }
}