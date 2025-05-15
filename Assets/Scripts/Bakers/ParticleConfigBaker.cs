using Authoring;
using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bakers
{
    public class ParticleConfigBaker : Baker<ParticleConfigAuthoring>
    {
        public override void Bake(ParticleConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity,
                new ParticleInitConfigComponent()
                {
                    ParticleCount = authoring.particleCount,
                    MinPosition = authoring.minPosition,
                    MaxPosition = authoring.maxPosition,
                });
            AddComponent(entity, new ParticleSimulationConfigComponent()
            {
                MaxAttractionDistance = authoring.scale * authoring.attractionDistanceUnit,
                ForceStrength = authoring.forceStrength,
                FrictionHalfLife = authoring.frictionHalfLife,
                FrictionFactor = math.pow(0.5f, 0.3333333f / authoring.frictionHalfLife),
                // FrictionFactor = 0.00310039447f,
            });

            UnityEngine.Debug.Log("ParticleConfigBaker");
        }
    }
}