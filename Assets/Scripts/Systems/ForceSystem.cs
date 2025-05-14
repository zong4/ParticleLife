using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ForceSystem : ISystem
    {
        private EntityQuery _particleQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ParticleSimulationConfigComponent>();
            state.RequireForUpdate<AttractionMatrixComponent>();

            _particleQuery = SystemAPI.QueryBuilder().WithAll<Particle>().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var particles = _particleQuery.ToComponentDataArray<Particle>(Allocator.TempJob);
            var colorIndices = new NativeArray<int>(particles.Length, Allocator.TempJob);
            var positions = new NativeArray<float2>(particles.Length, Allocator.TempJob);
            var velocities = new NativeArray<float2>(particles.Length, Allocator.TempJob);
            for (var i = 0; i < particles.Length; i++)
            {
                colorIndices[i] = particles[i].ColorIndex;
                positions[i] = particles[i].Position;
                velocities[i] = particles[i].Velocity;
            }

            var job = new ForceJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                ColorIndices = colorIndices,
                Positions = positions,
                Velocities = velocities,
                ConfigComponent = SystemAPI.GetSingleton<ParticleSimulationConfigComponent>(),
                MatrixComponent = SystemAPI.GetSingleton<AttractionMatrixComponent>(),
            };
            job.ScheduleParallel(particles.Length, 64, state.Dependency).Complete();

            var index = 0;
            foreach (var particle in SystemAPI.Query<RefRW<Particle>>())
            {
                var p = particle.ValueRO;
                p.Velocity = velocities[index];
                particle.ValueRW = p;
                index++;
            }

            particles.Dispose();
            colorIndices.Dispose();
            positions.Dispose();
            velocities.Dispose();
        }

        [BurstCompile]
        private struct ForceJob : IJobFor
        {
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public NativeArray<int> ColorIndices;
            [ReadOnly] public NativeArray<float2> Positions;
            public NativeArray<float2> Velocities;
            [ReadOnly] public ParticleSimulationConfigComponent ConfigComponent;
            [ReadOnly] public AttractionMatrixComponent MatrixComponent;

            public void Execute(int i)
            {
                var colorIndexA = ColorIndices[i];
                var posA = Positions[i];

                var totalForce = float2.zero;
                for (var j = 0; j < Positions.Length; j++)
                {
                    if (i == j) continue;

                    var dir = Positions[j] - posA;
                    var dist = math.length(dir);
                    if (!(dist < ConfigComponent.Scale * ConfigComponent.AttractionDistanceUnit)) continue;

                    var unitDir = math.normalizesafe(dir);
                    var attraction =
                        MatrixComponent.AttractionMatrix.Value.Matrix[
                            colorIndexA * MatrixComponent.ColorCount + ColorIndices[j]];
                    var distanceEffect =
                        math.pow(1 - dist / (ConfigComponent.Scale * ConfigComponent.AttractionDistanceUnit), 1);

                    totalForce += unitDir * (attraction * distanceEffect);
                }

                Velocities[i] += totalForce * (ConfigComponent.ForceStrength * DeltaTime);
                Velocities[i] *= ConfigComponent.DampingFactor;
            }
        }
    }
}