using UnityEngine;
using Core;

namespace Authoring
{
    public class ParticleConfigAuthoring : MonoBehaviour
    {
        // Init
        public int particleCount;
        public Vector2 minPosition;
        public Vector2 maxPosition;

        // Simulation
        [ReadOnly] public float scale = 0.1f;
        public float attractionDistanceUnit;
        public float forceStrength;
        public float frictionHalfLife;
    }
}