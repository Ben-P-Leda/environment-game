using System.Collections;
using Common;
using UnityEngine;

namespace Rain
{
    public class CloudPool : MonoBehaviour
    {
        [SerializeField] private GameObject _cloudPrefab = null;
        [SerializeField] private int _poolSize = 5;
        [SerializeField] private float _minimumCloudInterval = 5.0f;
        [SerializeField] private float _maximumCloudInterval = 15.0f;

        private ObjectPool _cloudPool;

        private void Awake()
        {
            _cloudPool = new ObjectPool(_cloudPrefab, _poolSize, transform);
        }

        private void Start()
        {
            StartCoroutine(StartNextRainCloud());
        }

        private IEnumerator StartNextRainCloud()
        {
            yield return new WaitForSeconds(Random.Range(_maximumCloudInterval, _maximumCloudInterval));
            _cloudPool.GetFirstAvailable()?.SetActive(true);

            StartCoroutine(StartNextRainCloud());
        }
    }
}