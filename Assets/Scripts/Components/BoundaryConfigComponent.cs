using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct BoundaryConfigComponent : IComponentData
    {
        public bool IsBoundaryEnabled;
        public float2 MinPosition;
        public float2 MaxPosition;
    }
}