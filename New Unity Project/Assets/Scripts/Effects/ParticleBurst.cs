using UnityEngine;

namespace Effects
{
    public class ParticleBurst : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void FixedUpdate()
        {
            if (!_particleSystem.IsAlive())
            {
                gameObject.SetActive(false);
            }
        }
    }
}