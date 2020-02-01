using UnityEngine;

namespace Smog
{
    public class SmogOverlay : MonoBehaviour
    {
        private ParticleSystem _particles;

        private float _particleDensity;

        private void Awake()
        {
            _particles = GetComponent<ParticleSystem>();
            _particleDensity = 0.0f;

            SetSmogDensity();
        }

        private void SetSmogDensity()
        {
            ParticleSystem.MainModule particleMainSettings = _particles.main;
            particleMainSettings.startColor = Color.Lerp(Color.clear, Color.gray, _particleDensity);

            ParticleSystem.EmissionModule particleEmissionSettings = _particles.emission;
            particleEmissionSettings.rateOverTime = new ParticleSystem.MinMaxCurve(Mathf.Lerp(0.0f, 100.0f, _particleDensity));
        }

        private void FixedUpdate()
        {
            _particleDensity = Mathf.Clamp01(_particleDensity + (Time.fixedDeltaTime * 0.1f));

            SetSmogDensity();
        }
    }
}