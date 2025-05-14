using Authoring;
using Components;
using Unity.Entities;

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
            AddComponent(entity,
                new ParticleSimulationConfigComponent()
                {
                    Scale = authoring.scale,
                    AttractionDistanceUnit = authoring.attractionDistanceUnit,
                    ForceStrength = authoring.forceStrength,
                    DampingFactor = authoring.dampingFactor,
                });

            UnityEngine.Debug.Log("ParticleConfigBaker");
        }
    }
}