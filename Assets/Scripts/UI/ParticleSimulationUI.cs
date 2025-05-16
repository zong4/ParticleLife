using Authoring;
using Components;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace UI
{
    public class ParticleSimulationUI : MonoBehaviour
    {
        private static EntityManager _entityManager;

        private static Entity _particleSimulationConfig;
        private static ParticleSimulationConfigAuthoring _particleSimulationAuthoring;

        public TextMeshProUGUI simulationEnabledText;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _particleSimulationConfig = _entityManager.CreateEntityQuery(typeof(ParticleSimulationConfigComponent))
                .GetSingletonEntity();
            _particleSimulationAuthoring = FindObjectOfType<ParticleSimulationConfigAuthoring>();
        }

        public void FlipSimulationState()
        {
            var data = _entityManager.GetComponentData<ParticleSimulationConfigComponent>(_particleSimulationConfig);
            data.SimulationEnabled = !data.SimulationEnabled;
            _entityManager.SetComponentData(_particleSimulationConfig, data);

            _particleSimulationAuthoring.simulationEnabled = data.SimulationEnabled;

            simulationEnabledText.text = data.SimulationEnabled ? "Simulate" : "Pause";
        }

        public void SetMaxAttractionDistanceUnit(string str)
        {
            if (!float.TryParse(str, out var result)) return;

            var data = _entityManager.GetComponentData<ParticleSimulationConfigComponent>(_particleSimulationConfig);
            data.MaxAttractionDistance = result;
            _entityManager.SetComponentData(_particleSimulationConfig, data);

            _particleSimulationAuthoring.maxAttractionDistanceUnit = data.MaxAttractionDistance;
        }

        public void SetForceStrength(string str)
        {
            if (!float.TryParse(str, out var result)) return;

            var data = _entityManager.GetComponentData<ParticleSimulationConfigComponent>(_particleSimulationConfig);
            data.ForceStrength = result;
            _entityManager.SetComponentData(_particleSimulationConfig, data);

            _particleSimulationAuthoring.forceStrength = data.ForceStrength;
        }

        public void SetFrictionHalfLife(string str)
        {
            if (!float.TryParse(str, out var result)) return;

            var data = _entityManager.GetComponentData<ParticleSimulationConfigComponent>(_particleSimulationConfig);
            data.FrictionHalfLife = result;
            _entityManager.SetComponentData(_particleSimulationConfig, data);

            _particleSimulationAuthoring.frictionHalfLife = data.FrictionHalfLife;
        }
    }
}