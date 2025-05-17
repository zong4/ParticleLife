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
            state.RequireForUpdate<ColorConfigComponent>();
            state.RequireForUpdate<ParticleSimulationConfigComponent>();
            state.RequireForUpdate<BoundaryConfigComponent>();

            _particleQuery = SystemAPI.QueryBuilder().WithAll<Particle>().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var simulationComponent = SystemAPI.GetSingleton<ParticleSimulationConfigComponent>();
            if (!simulationComponent.SimulationEnabled) return;

            var colorComponent = SystemAPI.GetSingleton<ColorConfigComponent>();
            var boundaryComponent = SystemAPI.GetSingleton<BoundaryConfigComponent>();

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

            var deltaTime = SystemAPI.Time.DeltaTime;
            var frictionFactor = CalFrictionFactor(deltaTime, simulationComponent.FrictionHalfLife,
                simulationComponent.FrictionFactor);
            var job = new ForceJob
            {
                // Single
                ColorIndices = colorIndices,
                Positions = positions,
                Velocities = velocities,

                // Global
                ColorComponent = colorComponent,
                DeltaTime = deltaTime,
                FrictionFactor = frictionFactor,
                SimulationComponent = simulationComponent,
                BoundaryComponent = boundaryComponent
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
            // Single
            [ReadOnly] public NativeArray<int> ColorIndices;
            [ReadOnly] public NativeArray<float2> Positions;
            public NativeArray<float2> Velocities;

            // Global
            [ReadOnly] public ColorConfigComponent ColorComponent;
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public float FrictionFactor;
            [ReadOnly] public ParticleSimulationConfigComponent SimulationComponent;
            [ReadOnly] public BoundaryConfigComponent BoundaryComponent;

            public void Execute(int i)
            {
                var colorIndexA = ColorIndices[i];
                var posA = Positions[i];

                var totalForceDir = float2.zero;
                for (var j = 0; j < Positions.Length; j++)
                {
                    if (i == j) continue;

                    float2 dir;
                    if (BoundaryComponent.BoundaryEnabled)
                    {
                        GetWrappedDelta(posA, Positions[j], BoundaryComponent.MinPosition,
                            BoundaryComponent.MaxPosition, out dir);
                    }
                    else
                    {
                        dir = Positions[j] - posA;
                    }

                    var dist = math.length(dir);
                    if (!(dist < SimulationComponent.MaxAttractionDistance)) continue;

                    var f = CalForce(dist / SimulationComponent.MaxAttractionDistance,
                        ColorComponent.AttractionMatrix.Value.Matrix[
                            colorIndexA * ColorComponent.ColorCount + ColorIndices[j]], 0.3f);
                    totalForceDir += math.normalizesafe(dir) * f;

                    // {
                    //     var attrFactor = attraction * math.pow(1 - dist / maxDist, 1);
                    //     var repulsionFactor = -math.pow((maxDist - dist) / (maxDist - radius), 1.0f); // 0.9 and 1.0
                    //
                    //     totalForceDir += math.normalizesafe(dir) * (attrFactor + repulsionFactor);
                    // }

                    // {
                    //     var beta = ConfigComponent.Scale * ConfigComponent.AttractionMiddleUnit;
                    //     
                    //     float factor;
                    //     if (dist <= radius)
                    //     {
                    //         factor = dist / radius - 1;
                    //     }
                    //     else if(dist <= beta)
                    //     {
                    //         factor = attraction * (dist - radius) / (beta - radius);
                    //     }
                    //     else
                    //     {
                    //         factor = attraction * (maxDist - dist) / (maxDist - beta);
                    //     }
                    //     totalForceDir += math.normalizesafe(dir) * factor;
                    // }
                }

                Velocities[i] *= FrictionFactor;
                Velocities[i] += totalForceDir * (SimulationComponent.ForceStrength * DeltaTime);
            }
        }

        [BurstCompile]
        private static float CalFrictionFactor(float deltaTime, float frictionHalfLife, float taylor1)
        {
            // var frictionFactor = math.pow(0.5f, deltaTime / configComponent.FrictionHalfLife);
            var taylor2 = -math.LN2 / frictionHalfLife * taylor1 * (deltaTime - 0.3333333f);
            var taylor3 = -math.LN2 / frictionHalfLife * taylor2 * (deltaTime - 0.3333333f) / 2;
            return taylor1 + taylor2 + taylor3;
        }

        [BurstCompile]
        private static float CalForce(float r, float a, float beta)
        {
            if (r < beta)
            {
                return r / beta - 1;
            }

            if (r > beta && r < 1)
            {
                return a * (1 - math.abs(2 * r - 1 - beta) / (1 - beta));
            }

            return 0.0f;
        }

        [BurstCompile]
        private static void GetWrappedDelta(in float2 a, in float2 b, in float2 min, in float2 max, out float2 result)
        {
            var size = max - min;
            var delta = b - a;

            if (delta.x > size.x / 2f) delta.x -= size.x;
            else if (delta.x < -size.x / 2f) delta.x += size.x;

            if (delta.y > size.y / 2f) delta.y -= size.y;
            else if (delta.y < -size.y / 2f) delta.y += size.y;

            result = delta;
        }
    }
}