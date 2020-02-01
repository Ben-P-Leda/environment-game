using System;
using System.Collections;
using Common;
using GameManagement;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plants
{
    public class PlantPool : MonoBehaviour, ISuspendOnSmogLimitReached
    {
        [SerializeField] private GameObject _plantPrefab = null;
        [SerializeField] private int _poolSize = 5;
        [SerializeField] private int _startingPlants = 3;

        private ObjectPool _plantPool;

        public event Action AllPlantsHaveDied;

        private void Awake()
        {
            _plantPool = new ObjectPool(_plantPrefab, _poolSize, transform);

            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);
        }

        private void Start()
        {
            StartCoroutine(StartNextPlant());
        }

        private IEnumerator StartNextPlant()
        {
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));

            if ((enabled) && (_startingPlants > 0))
            {
                _startingPlants -= 1;
                _plantPool.GetFirstAvailable()?.SetActive(true);
                StartCoroutine(StartNextPlant());
            }
        }

        private void FixedUpdate()
        {
            if ((enabled) && (_startingPlants <= 0) && (_plantPool.AvailableObjectCount == _poolSize))
            {
                AllPlantsHaveDied?.Invoke();
            }
        }
    }
}