using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Particle : IComponentData
    {
        public int ColorIndex;
        public float2 Position;
        public float2 Velocity;
    }
}