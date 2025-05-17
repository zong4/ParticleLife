using Authoring;
using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bakers
{
    public class ParticleConfigBaker : Baker<ParticleSimulationConfigAuthoring>
    {
        public override void Bake(ParticleSimulationConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // ColorConfigComponent
            AddComponent(entity, new ColorConfigComponent { ColorCount = 3 });

            // ParticleSimulationConfigComponent
            AddComponent(entity, new ParticleSimulationConfigComponent()
            {
                SimulationEnabled = false,
                MaxAttractionDistance = authoring.scale * authoring.maxAttractionDistanceUnit,
                ForceStrength = authoring.forceStrength,
                FrictionHalfLife = authoring.frictionHalfLife,
                FrictionFactor = math.pow(0.5f, 0.3333333f / authoring.frictionHalfLife),
                // FrictionFactor = 0.00310039447f,
            });

            DependsOn(authoring.transform);

            UnityEngine.Debug.Log("ParticleConfigBaker");
        }
    }
}