using System.Collections;
using Common;
using GameManagement;
using Interfaces;
using UnityEngine;

namespace Rain
{
    public class CloudPool : MonoBehaviour, ISuspendOnSmogLimitReached
    {
        [SerializeField] private GameObject _cloudPrefab = null;
        [SerializeField] private int _poolSize = 5;
        [SerializeField] private float _minimumCloudInterval = 15.0f;
        [SerializeField] private float _maximumCloudInterval = 30.0f;

        private ObjectPool _cloudPool;

        private void Awake()
        {
            _cloudPool = new ObjectPool(_cloudPrefab, _poolSize, transform);

            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);
        }

        private void Start()
        {
            StartCoroutine(StartNextRainCloud());
        }

        private IEnumerator StartNextRainCloud()
        {
            yield return new WaitForSeconds(Random.Range(_minimumCloudInterval, _maximumCloudInterval));

            if (enabled)
            {
                _cloudPool.GetFirstAvailable()?.SetActive(true);

                StartCoroutine(StartNextRainCloud());
            }
        }
    }
}