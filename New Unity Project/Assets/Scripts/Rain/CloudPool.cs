using System.Collections;
using Common;
using UnityEngine;

namespace Rain
{
    public class CloudPool : MonoBehaviour
    {
        [SerializeField] private GameObject _cloudPrefab = null;
        [SerializeField] private int _poolSize = 5;

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
            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
            _cloudPool.GetFirstAvailable()?.SetActive(true);

            StartCoroutine(StartNextRainCloud());
        }
    }
}