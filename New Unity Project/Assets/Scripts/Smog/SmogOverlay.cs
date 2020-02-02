using System;
using System.Collections.Generic;
using System.Linq;
using GameManagement;
using Interfaces;
using UI;
using UnityEngine;

namespace Smog
{
    public class SmogOverlay : MonoBehaviour, ISuspendOnSmogLimitReached
    {
        [SerializeField] private GameObject _smogDensityMeterGameObject = null;
        [SerializeField] private float _particleDensity = 0.5f;

        private ParticleSystem _particles;
        private MeterDisplay _smogHealthMeter;

        public event Action SmogCleared;

        public float SmogDensity { get { return _particleDensity; } }

        private List<ISmogDensityChangeModifier> changeRateModifiers = new List<ISmogDensityChangeModifier>();

        public void RegisterDensityChangeModifier(ISmogDensityChangeModifier modifierProvider)
        {
            if (!changeRateModifiers.Contains(modifierProvider))
            {
                changeRateModifiers.Add(modifierProvider);
            }
        }

        private void Awake()
        {
            _particles = GetComponent<ParticleSystem>();
            _smogHealthMeter = _smogDensityMeterGameObject.GetComponent<MeterDisplay>();

            changeRateModifiers = new List<ISmogDensityChangeModifier>();

            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);
        }

        private void OnEnable()
        {
            _smogHealthMeter.StartValue = _particleDensity;
        }

        private void SetSmogDensity()
        {
            ParticleSystem.MainModule particleMainSettings = _particles.main;
            particleMainSettings.startColor = Color.Lerp(Color.clear, Color.gray, (_particleDensity * 0.75f) + 0.25f);

            ParticleSystem.EmissionModule particleEmissionSettings = _particles.emission;
            particleEmissionSettings.rateOverTime = new ParticleSystem.MinMaxCurve(Mathf.Lerp(0.0f, 100.0f, _particleDensity));
        }

        private void FixedUpdate()
        {
            float rateOfChange = changeRateModifiers.Sum(x => x.ChangeRateModifier);

            _particleDensity = Mathf.Clamp01(_particleDensity + (Time.fixedDeltaTime * 0.1f * rateOfChange));

            SetSmogDensity();

            if (_particleDensity <= 0.0f)
            {
                SmogCleared?.Invoke();
            }

            _smogHealthMeter.DisplayValue = _particleDensity;
        }
    }
}