using System.Collections;
using Common;
using GameManagement;
using Interfaces;
using UnityEngine;

namespace Smog
{
    public class BugYeeter : MonoBehaviour, ISuspendOnSmogLimitReached
    {
        [SerializeField] private GameObject _bugPrefab = null;
        [SerializeField] private int _poolSize = 5;
        [SerializeField] private float _minimumBugInterval = 5.0f;
        [SerializeField] private float _maximumBugInterval = 15.0f;

        private ObjectPool _bugPool;

        private void Awake()
        {
            _bugPool = new ObjectPool(_bugPrefab, _poolSize, transform);
            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);
        }

        private void Start()
        {
            StartCoroutine(StartNextBug());
        }

        private IEnumerator StartNextBug()
        {
            yield return new WaitForSeconds(Random.Range(_minimumBugInterval, _maximumBugInterval));

            if (enabled)
            {
                _bugPool.GetFirstAvailable()?.SetActive(true);

                StartCoroutine(StartNextBug());
            }
        }
    }
}