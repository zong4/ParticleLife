using UnityEngine;

namespace Authoring
{
    public class ParticleConfigAuthoring : MonoBehaviour
    {
        // Init
        public int particleCount;
        public Vector2 minPosition;
        public Vector2 maxPosition;

        // Single particle properties
        public float scale;
        public float attractionDistanceUnit;
        public float forceStrength;
        public float dampingFactor;
    }
}