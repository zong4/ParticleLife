using Unity.Entities;

namespace Components
{
    public struct AttractionMatrixBlob
    {
        public BlobArray<float> Matrix;
    }

    public struct ColorConfigComponent : IComponentData
    {
        public int ColorCount;
        public BlobAssetReference<AttractionMatrixBlob> AttractionMatrix;
    }
}