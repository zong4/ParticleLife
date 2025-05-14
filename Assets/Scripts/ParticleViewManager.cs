using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
using Authoring;
using Components;

public class ParticleViewManager : MonoBehaviour
{
    private EntityManager _entityManager;

    public GameObject particlePrefab;
    private readonly Dictionary<Entity, GameObject> _views = new();

    private float _scale;
    private Color[] _colors;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _scale = FindObjectOfType<ParticleConfigAuthoring>().scale;
        _colors = FindObjectOfType<AttractionMatrixAuthoring>().colors;
    }

    private void Update()
    {
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
}