using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InitParticlesSystem : SystemBase
    {
        private bool _initialized;

        protected override void OnCreate()
        {
            RequireForUpdate<ParticleInitConfigComponent>();
            RequireForUpdate<AttractionMatrixComponent>();
        }

        protected override void OnUpdate()
        {
            if (_initialized) return;

            // DOTS
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var archetype = entityManager.CreateArchetype(typeof(Particle), typeof(LocalToWorld));

            var rand = new Random((uint)UnityEngine.Random.Range(1, 100000));
            var configComponent = SystemAPI.GetSingleton<ParticleInitConfigComponent>();
            var colorCount = SystemAPI.GetSingleton<AttractionMatrixComponent>().ColorCount;
            for (var i = 0; i < configComponent.ParticleCount; i++)
            {
                var e = entityManager.CreateEntity(archetype);

                var pos = rand.NextFloat2(new float2(configComponent.MinPosition.x, configComponent.MinPosition.y),
                    new float2(configComponent.MaxPosition.x, configComponent.MaxPosition.y));

                entityManager.SetComponentData(e,
                    new Particle { ColorIndex = rand.NextInt(0, colorCount), Position = pos, Velocity = float2.zero });
                entityManager.SetComponentData(e, new LocalToWorld { Value = float4x4.Translate(new float3(pos, 0)) });
            }

            _initialized = true;
        }
    }
}