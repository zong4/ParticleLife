using Authoring;
using Components;
using Unity.Collections;
using Unity.Entities;

namespace Bakers
{
    public class ColorConfigBaker : Baker<ColorConfigAuthoring>
    {
        public override void Bake(ColorConfigAuthoring authoring)
        {
            var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<AttractionMatrixBlob>();

            var flatMatrix = builder.Allocate(ref root.Matrix, authoring.colors.Length * authoring.colors.Length);
            for (var i = 0; i < authoring.colors.Length; i++)
            {
                for (var j = 0; j < authoring.colors.Length; j++)
                {
                    flatMatrix[i * authoring.colors.Length + j] = authoring.matrix[i * authoring.colors.Length + j];
                }
            }

            var blobRef = builder.CreateBlobAssetReference<AttractionMatrixBlob>(Allocator.Temp);
            builder.Dispose();

            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity,
                new ColorConfigComponent() { ColorCount = authoring.colors.Length, AttractionMatrix = blobRef });

            UnityEngine.Debug.Log("AttractionMatrixBaker");
        }
    }
}