using Components;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace UI
{
    public class ParticleSimulationUI : MonoBehaviour
    {
        public ColorConfigUI colorConfig;

        private static EntityManager _entityManager;
        private static Entity _particleSimulationConfig;
        // private static ParticleSimulationConfigAuthoring _particleSimulationAuthoring;

        public TextMeshProUGUI simulationEnabledText;
        public TMP_InputField maxAttractionDistanceText;
        public TMP_InputField forceStrengthText;
        public TMP_InputField frictionHalfLifeText;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _particleSimulationConfig = _entityManager.CreateEntityQuery(typeof(ParticleSimulationConfigComponent))
                .GetSingletonEntity();
            // _particleSimulationAuthoring = FindObjectOfType<ParticleSimulationConfigAuthoring>();

            SetMaxAttractionDistanceUnit(maxAttractionDistanceText.text);
            SetForceStrength(forceStrengthText.text);
            SetFrictionHalfLife(frictionHalfLifeText.text);
        }

        public void FlipSimulationState()
        {
            colorConfig.SetDelButtonInteractable(false);

            var data = _entityManager.GetComponentData<ParticleSimulationConfigComponent>(_particleSimulationConfig);
            data.SimulationEnabled = !data.SimulationEnabled;
            _entityManager.SetComponentData(_particleSimulationConfig, data);

            simulationEnabledText.text = data.SimulationEnabled ? "Simulate" : "Pause";
        }

        public void SetMaxAttractionDistanceUnit(string str)
        {
            if (!float.TryParse(str, out var result)) return;

            var data = _entityManager.GetComponentData<ParticleSimulationConfigComponent>(_particleSimulationConfig);
            data.MaxAttractionDistance = result;
            _entityManager.SetComponentData(_particleSimulationConfig, data);
        }

        public void SetForceStrength(string str)
        {
            if (!float.TryParse(str, out var result)) return;

            var data = _entityManager.GetComponentData<ParticleSimulationConfigComponent>(_particleSimulationConfig);
            data.ForceStrength = result;
            _entityManager.SetComponentData(_particleSimulationConfig, data);
        }

        public void SetFrictionHalfLife(string str)
        {
            if (!float.TryParse(str, out var result)) return;

            var data = _entityManager.GetComponentData<ParticleSimulationConfigComponent>(_particleSimulationConfig);
            data.FrictionHalfLife = result;
            _entityManager.SetComponentData(_particleSimulationConfig, data);
        }
    }
}