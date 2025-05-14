using UnityEngine;

namespace Authoring
{
    public class AttractionMatrixAuthoring : MonoBehaviour
    {
        public Color[] colors;
        public readonly float[,] Matrix = { { 1.0f, 0.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 0.0f, 1.0f } };
    }
}