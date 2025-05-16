using UnityEngine;
using Core;

namespace Authoring
{
    public class ParticleSimulationConfigAuthoring : MonoBehaviour
    {
        public bool simulationEnabled;
        [ReadOnly] public float scale = 0.1f;
        public float maxAttractionDistanceUnit;
        public float forceStrength;
        public float frictionHalfLife;
    }
}