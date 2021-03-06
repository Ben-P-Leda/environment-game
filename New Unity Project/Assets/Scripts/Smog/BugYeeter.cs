﻿using System.Collections;
using Common;
using Effects;
using GameManagement;
using Interfaces;
using UnityEngine;

namespace Smog
{
    public class BugYeeter : MonoBehaviour, ISuspendOnSmogLimitReached
    {
        [SerializeField] private GameObject _bugPrefab = null;
        [SerializeField] private int _poolSize = 5;
        [SerializeField] private float _minimumBugInterval = 7.5f;
        [SerializeField] private float _maximumBugInterval = 20.0f;

        private ObjectPool _bugPool;

        private void Awake()
        {
            _bugPool = new ObjectPool(_bugPrefab, _poolSize, transform, WireUpSplatParticles);
            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);
        }

        private void WireUpSplatParticles(GameObject bugObject)
        {
            bugObject.GetComponent<BugLifecycle>().SplatParticles = GetComponent<ParticlePool>();
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