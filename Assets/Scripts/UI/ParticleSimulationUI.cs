using System.Globalization;
using Components;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
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

        private float _forceStrength;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Update()
        {
            if (_particleSimulationConfig == Entity.Null)
            {
                var query = _entityManager.CreateEntityQuery(typeof(ParticleSimulationConfigComponent));
                if (query.IsEmpty)
                    return;

                _particleSimulationConfig = query.GetSingletonEntity();
                // _particleSimulationAuthoring = FindObjectOfType<ParticleSimulationConfigAuthoring>();

                SetMaxAttractionDistanceUnit(maxAttractionDistanceText.text);
                SetForceStrength(forceStrengthText.text);
                SetFrictionHalfLife(frictionHalfLifeText.text);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                FlipSimulationState();
            }

            if (Input.GetKeyUp(KeyCode.Comma))
            {
                _forceStrength *= 0.5f;
                UpdateForceText();
            }

            if (Input.GetKeyUp(KeyCode.Period))
            {
                _forceStrength *= 2f;
                UpdateForceText();
            }
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

            _forceStrength = result;
        }

        public void SetFrictionHalfLife(string str)
        {
            if (!float.TryParse(str, out var result)) return;

            var data = _entityManager.GetComponentData<ParticleSimulationConfigComponent>(_particleSimulationConfig);
            data.FrictionHalfLife = result;
            data.FrictionFactor = math.pow(0.5f, 0.3333333f / 0.04f);
            _entityManager.SetComponentData(_particleSimulationConfig, data);
        }

        private void UpdateForceText()
        {
            forceStrengthText.text = _forceStrength.ToString(CultureInfo.InvariantCulture);
            SetForceStrength(forceStrengthText.text);
        }
    }
}