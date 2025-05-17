using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

                    var width = max.x - min.x;
                    var height = max.y - min.y;

                    pos.x = min.x + math.fmod((pos.x - min.x + width), width);
                    pos.y = min.y + math.fmod((pos.y - min.y + height), height);
                }

                particle.Position = pos;
                localToWorld.Value = float4x4.Translate(new float3(pos, 0));
            }
        }
    }
}