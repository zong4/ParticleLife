using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
using Authoring;
using Components;
using UI;

public class ParticleViewManager : MonoBehaviour
{
    private EntityManager _entityManager;
    public GameObject particlePrefab;
    private readonly Dictionary<Entity, GameObject> _views = new();

    // Properties
    private float _scale;
    private List<Color> _colors;

    // Extend functionality
    public Canvas canvas;
    public ColorConfigUI colorConfig;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Properties
        _scale = FindObjectOfType<ParticleSimulationConfigAuthoring>().scale;
        _colors = FindObjectOfType<ColorConfigUI>().colors;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            canvas.enabled = !canvas.enabled;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearEntities();
            ClearViews();

            // DelButton
            colorConfig.SetDelButtonInteractable(true);
        }

        var query = _entityManager.CreateEntityQuery(typeof(Particle));
        var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
        foreach (var entity in entities)
        {
            var data = _entityManager.GetComponentData<Particle>(entity);
            if (!_views.ContainsKey(entity))
            {
                var go = Instantiate(particlePrefab, transform);
                _views[entity] = go;

                go.transform.localScale = new Vector3(_scale, _scale, _scale);

                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                go.GetComponent<SpriteRenderer>().color = _colors[data.ColorIndex];
            }

            _views[entity].transform.position = new Vector3(data.Position.x, data.Position.y, 0);
        }
    }

    private void ClearEntities()
    {
        var query = _entityManager.CreateEntityQuery(typeof(Particle));
        _entityManager.DestroyEntity(query);
    }

    private void ClearViews()
    {
        foreach (var go in _views.Values)
        {
            Destroy(go);
        }

        _views.Clear();
    }
}