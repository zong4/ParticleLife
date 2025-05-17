using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ParticleMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ParticleSimulationConfigComponent>();
            state.RequireForUpdate<BoundaryConfigComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var simulationComponent = SystemAPI.GetSingleton<ParticleSimulationConfigComponent>();
            if (!simulationComponent.SimulationEnabled) return;

            var boundaryComponent = SystemAPI.GetSingleton<BoundaryConfigComponent>();
            var job = new MovementJob { DeltaTime = SystemAPI.Time.DeltaTime, BoundaryComponent = boundaryComponent };
            job.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MovementJob : IJobEntity
        {
            public float DeltaTime;
            public BoundaryConfigComponent BoundaryComponent;

            private void Execute(ref Particle particle, ref LocalToWorld localToWorld)
            {
                var pos = particle.Position;
                pos += particle.Velocity * DeltaTime;

                if (BoundaryComponent.BoundaryEnabled)
                {
                    var min = BoundaryComponent.MinPosition;
                    var max = BoundaryComponent.MaxPosition;

                    // Wrap on X axis
                    if (pos.x < min.x) pos.x = max.x;
                    else if (pos.x > max.x) pos.x = min.x;

                    // Wrap on Y axis
                    if (pos.y < min.y) pos.y = max.y;
                    else if (pos.y > max.y) pos.y = min.y;
                }

                particle.Position = pos;
                localToWorld.Value = float4x4.Translate(new float3(pos, 0));
            }
        }
    }
}