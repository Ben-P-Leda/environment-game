using System;
using System.Collections;
using Common;
using GameManagement;
using Interfaces;
using PlayingField;
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
        private PlayingFieldGrid _playingFieldGrid;
        private float _timeToNextPlant;

        public event Action AllPlantsHaveDied;

        private void Awake()
        {
            _plantPool = new ObjectPool(_plantPrefab, _poolSize, transform);
            _playingFieldGrid = FindObjectOfType<PlayingFieldGrid>();

            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);
        }

        private void Start()
        {
            _timeToNextPlant = 0.1f;

            StartCoroutine(StartNextPlant());
        }

        private IEnumerator StartNextPlant()
        {
            yield return new WaitForSeconds(_timeToNextPlant);

            if ((enabled) && (_plantPool.AvailableObjectCount > 0))
            {
                PlayingFieldTile newPlantLocation = _playingFieldGrid.GetRandomClearPatch();

                if (newPlantLocation != null)
                {
                    _startingPlants -= 1;

                    GameObject nextPlant = _plantPool.GetFirstAvailable();
                    if (nextPlant != null)
                    {
                        nextPlant.GetComponent<PlantLifecycle>().TileLocation = newPlantLocation;
                        nextPlant.SetActive(true);
                    }

                    _timeToNextPlant = _startingPlants > 0 ? Random.Range(0.2f, 0.5f) : 45.0f;
                }

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