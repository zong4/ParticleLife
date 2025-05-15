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
            state.RequireForUpdate<BoundaryConfigComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var configComponent = SystemAPI.GetSingleton<BoundaryConfigComponent>();

            var job = new MovementJob { DeltaTime = SystemAPI.Time.DeltaTime, ConfigComponent = configComponent };
            job.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MovementJob : IJobEntity
        {
            public float DeltaTime;
            public BoundaryConfigComponent ConfigComponent;

            private void Execute(ref Particle particle, ref LocalToWorld localToWorld)
            {
                var pos = particle.Position;
                pos += particle.Velocity * DeltaTime;

                if (ConfigComponent.IsBoundaryEnabled)
                {
                    var min = ConfigComponent.MinPosition;
                    var max = ConfigComponent.MaxPosition;
                    if (pos.x < min.x || pos.x > max.x)
                    {
                        particle.Velocity.x = 0;
                        pos.x = math.clamp(pos.x, min.x, max.x);
                    }

                    if (pos.y < min.y || pos.y > max.y)
                    {
                        particle.Velocity.y = 0;
                        pos.y = math.clamp(pos.y, min.y, max.y);
                    }
                }

                particle.Position = pos;
                localToWorld.Value = float4x4.Translate(new float3(pos, 0));
            }
        }
    }
}