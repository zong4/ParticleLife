using Components;
using Unity.Entities;
using UnityEngine;

namespace UI
{
    public class BoundaryConfigUI : MonoBehaviour
    {
        private EntityManager _entityManager;
        private Entity _boundaryConfig;

        private bool _isSelectingStart;
        private bool _isSelectingEnd;
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
            if (_boundaryConfig == Entity.Null)
            {
                var query = _entityManager.CreateEntityQuery(typeof(BoundaryConfigComponent));
                if (query.IsEmpty)
                    return;

                _boundaryConfig = query.GetSingletonEntity();
                // _particleSimulationAuthoring = FindObjectOfType<ParticleSimulationConfigAuthoring>();
            }

            if (Input.GetKeyUp(KeyCode.B))
            {
                if (_isDrawing)
                {
                    _isDrawing = false;
                    SetBoundaryEnabled(false);
                }
                else
                {
                    _isSelectingStart = true;
                }
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

                    SetPositions();
                    SetBoundaryEnabled(true);
                }
            }
        }

        private void OnGUI()
        {
            if (!_isDrawing) return;

            DrawRect.DrawScreenRectBorder(
                DrawRect.GetScreenRect(_camera.WorldToScreenPoint(_startWorldPos),
                    _camera.WorldToScreenPoint(_endWorldPos)), 2, Color.green);
        }

        private void SetBoundaryEnabled(bool boundaryEnabled)
        {
            var data = _entityManager.GetComponentData<BoundaryConfigComponent>(_boundaryConfig);
            data.BoundaryEnabled = boundaryEnabled;
            _entityManager.SetComponentData(_boundaryConfig, data);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SetPositions()
        {
            var minPosition = new Vector2(Mathf.Min(_startWorldPos.x, _endWorldPos.x),
                Mathf.Min(_startWorldPos.y, _endWorldPos.y));
            var maxPosition = new Vector2(Mathf.Max(_startWorldPos.x, _endWorldPos.x),
                Mathf.Max(_startWorldPos.y, _endWorldPos.y));

            if (minPosition == maxPosition)
            {
                Debug.LogError("Invalid area selected");
                return;
            }

            var data = _entityManager.GetComponentData<BoundaryConfigComponent>(_boundaryConfig);
            data.MinPosition = minPosition;
            data.MaxPosition = maxPosition;
            _entityManager.SetComponentData(_boundaryConfig, data);
        }
    }
}