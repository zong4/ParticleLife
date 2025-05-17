using Authoring;
using Components;
using Unity.Entities;

namespace Bakers
{
    public class BoundaryConfigBaker : Baker<BoundaryConfigAuthoring>
    {
        public override void Bake(BoundaryConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new BoundaryConfigComponent
                {
                    BoundaryEnabled = authoring.boundaryEnabled,
                    MinPosition = authoring.minPosition,
                    MaxPosition = authoring.maxPosition,
                });

            DependsOn(authoring.transform);

            UnityEngine.Debug.Log("BoundaryConfigBaker");
        }
    }
}