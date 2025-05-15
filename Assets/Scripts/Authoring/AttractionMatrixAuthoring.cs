using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Authoring
{
    public class AttractionMatrixAuthoring : MonoBehaviour
    {
        public Color[] colors;
        public float[] matrix;

        public void OnValidate()
        {
            if (matrix != null && matrix.Length == colors.Length * colors.Length) return;

            var oldMatrix = matrix;
            matrix = new float[colors.Length * colors.Length];
            GenerateIdentityMatrix(oldMatrix);
        }

        private void GenerateIdentityMatrix(float[] oldMatrix = null)
        {
            var oldColorCount = 0;
            if (oldMatrix != null)
                oldColorCount = (int)math.sqrt(oldMatrix.Length);

            for (var i = 0; i < colors.Length; i++)
            {
                for (var j = 0; j < colors.Length; j++)
                {
                    if (oldMatrix != null && oldColorCount > i && oldColorCount > j)
                    {
                        matrix[colors.Length * i + j] = oldMatrix[oldColorCount * i + j];
                    }
                    else
                    {
                        matrix[colors.Length * i + j] = i == j ? 1.0f : 0.0f;
                    }
                }
            }
        }

        [ContextMenu("Generate Random Matrix")]
        public void GenerateRandomMatrix()
        {
            for (var i = 0; i < colors.Length; i++)
            {
                for (var j = 0; j < colors.Length; j++)
                {
                    matrix[colors.Length * i + j] = Random.Range(-1f, 1f);
                }
            }
        }
    }
}