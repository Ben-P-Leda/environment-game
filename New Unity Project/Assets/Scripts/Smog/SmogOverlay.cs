using System;
using System.Collections.Generic;
using System.Linq;
using GameManagement;
using Interfaces;
using UnityEngine;

namespace Smog
{
    public class SmogOverlay : MonoBehaviour, ISuspendOnSmogLimitReached
    {
        [SerializeField] private float _particleDensity = 0.5f;
        private ParticleSystem _particles;

        public event Action<bool> SmogLimitReached;

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
            changeRateModifiers = new List<ISmogDensityChangeModifier>();

            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);
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

            if ((_particleDensity <= 0.0f) || (_particleDensity >= 1.0f))
            {
                SmogLimitReached?.Invoke(_particleDensity <= 0.0f);
            }
        }
    }
}