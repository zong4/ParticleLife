using Unity.Entities;

namespace Components
{
    public struct AttractionMatrixBlob
    {
        public BlobArray<float> Matrix;
    }

    public struct AttractionMatrixComponent : IComponentData
    {
        public int ColorCount;
        public BlobAssetReference<AttractionMatrixBlob> AttractionMatrix;
    }
}