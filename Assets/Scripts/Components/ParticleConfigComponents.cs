using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct ParticleInitConfigComponent : IComponentData
    {
        public int ParticleCount;
        public float2 MinPosition;
        public float2 MaxPosition;
    }

    public struct ParticleSimulationConfigComponent : IComponentData
    {
        public float MaxAttractionDistance;
        public float ForceStrength;
        public float FrictionHalfLife;
        public float FrictionFactor;
    }
}