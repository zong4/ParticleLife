using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InitParticlesSystem : SystemBase
    {
        private Unity.Mathematics.Random _rand;

        protected override void OnCreate()
        {
            var seed = (uint)UnityEngine.Random.Range(1, 100000);
            Debug.Log("Random seed: " + seed);
            _rand = new Unity.Mathematics.Random(seed);

            RequireForUpdate<ParticleCreateRequestComponent>();
            RequireForUpdate<AttractionMatrixComponent>();
        }

        protected override void OnUpdate()
        {
            var query = World.EntityManager.CreateEntityQuery(typeof(ParticleCreateRequestComponent));
            if (query.CalculateEntityCount() == 0)
                return;

            var requestEntity = query.GetSingletonEntity();
            var request = SystemAPI.GetComponent<ParticleCreateRequestComponent>(requestEntity);

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var archetype = entityManager.CreateArchetype(typeof(Particle), typeof(LocalToWorld));

            var colorCount = SystemAPI.GetSingleton<AttractionMatrixComponent>().ColorCount;
            for (var i = 0; i < request.Count; i++)
            {
                var e = entityManager.CreateEntity(archetype);

                var pos = _rand.NextFloat2(request.MinPosition, request.MaxPosition);
                entityManager.SetComponentData(e,
                    new Particle { ColorIndex = _rand.NextInt(0, colorCount), Position = pos, Velocity = float2.zero });
                entityManager.SetComponentData(e, new LocalToWorld { Value = float4x4.Translate(new float3(pos, 0)) });
            }

            entityManager.DestroyEntity(requestEntity);
        }
    }
}