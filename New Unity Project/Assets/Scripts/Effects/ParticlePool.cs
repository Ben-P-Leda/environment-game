using Common;
using UnityEngine;

namespace Effects
{
    public class ParticlePool : MonoBehaviour
    {
        [SerializeField] private GameObject _particlePrefab = null;
        [SerializeField] private int _poolSize = 5;

        private ObjectPool _particlePool;

        public void LaunchParticleEffect(Vector3 position, Vector3 scale)
        {
            GameObject particles = _particlePool.GetFirstAvailable();

            if (particles != null)
            {
                particles.transform.position = position;
                particles.transform.localScale = scale;
                particles.SetActive(true);
            }
        }

        private void Awake()
        {
            _particlePool = new ObjectPool(_particlePrefab, _poolSize, transform);
        }
    }
}