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
        public float Scale;
        public float AttractionMiddleUnit;
        public float AttractionDistanceUnit;
        public float ForceStrength;
        public float DampingFactor;
    }
}