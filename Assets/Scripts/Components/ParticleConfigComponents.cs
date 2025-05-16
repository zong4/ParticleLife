using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct ParticleCreateRequestComponent : IComponentData
    {
        public int Count;
        public float2 MinPosition;
        public float2 MaxPosition;
    }

    public struct ParticleSimulationConfigComponent : IComponentData
    {
        public bool SimulationEnabled;
        public float MaxAttractionDistance;
        public float ForceStrength;
        public float FrictionHalfLife;
        public float FrictionFactor;
    }
}