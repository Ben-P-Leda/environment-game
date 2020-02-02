using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using GameManagement;
using Interfaces;
using PlayingField;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plants
{
    public class PlantPool : MonoBehaviour, ISuspendOnSmogLimitReached
    {
        [SerializeField] private GameObject _plantHealthMeterGameObject = null;
        [SerializeField] private GameObject _plantPrefab = null;
        [SerializeField] private int _poolSize = 5;
        [SerializeField] private int _startingPlants = 3;

        private ObjectPool _plantPool;
        private PlayingFieldGrid _playingFieldGrid;
        private MeterDisplay _plantHealthMeter;
        private float _timeToNextPlant;

        private List<PlantLifecycle> _plants;

        public event Action AllPlantsHaveDied;

        private void Awake()
        {
            _plants = new List<PlantLifecycle>();
            _plantPool = new ObjectPool(_plantPrefab, _poolSize, transform, GetLifecycleHandle);
            _playingFieldGrid = FindObjectOfType<PlayingFieldGrid>();
            _plantHealthMeter = _plantHealthMeterGameObject.GetComponent<MeterDisplay>();

            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);
        }

        private void GetLifecycleHandle(GameObject plantGameObject)
        {
            _plants.Add(plantGameObject.GetComponent<PlantLifecycle>());
        }

        private void Start()
        {
            _timeToNextPlant = 0.1f;
            _plantHealthMeter.StartValue = 0.0f;

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

            float currentPlantHealth = _plants.Where(x => x.gameObject.activeInHierarchy).Sum(x => x.HealthFraction) / _poolSize;
            _plantHealthMeter.DisplayValue = currentPlantHealth;
        }
    }
}