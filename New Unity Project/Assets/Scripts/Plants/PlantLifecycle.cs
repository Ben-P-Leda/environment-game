﻿using PlayingField;
using Smog;
using UnityEngine;

namespace Plants
{
    public class PlantLifecycle : MonoBehaviour
    {
        private Transform _transform;
        private PlayingFieldGrid _playingFieldGrid;
        private PlayingFieldTile _tileOccupied;
        private SmogOverlay _smogOverlay;
        private float _scale;
        private float _growthTimeRemaining;
        private float _timeToNextWaterAccept;

        private void Awake()
        {
            _transform = transform;
            _playingFieldGrid = FindObjectOfType<PlayingFieldGrid>();
            _smogOverlay = FindObjectOfType<SmogOverlay>();
        }

        private void OnEnable()
        {
            _scale = 0.0f;
            _growthTimeRemaining = Random.Range(0.5f, 0.55f);
            _timeToNextWaterAccept = 0.0f;
            _tileOccupied = _playingFieldGrid.GetRandomTile(true);

            if (_tileOccupied == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _transform.position = _tileOccupied.Position;
            _transform.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            _transform.localScale = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (_growthTimeRemaining > 0)
            {
                _growthTimeRemaining -= Time.fixedDeltaTime;
                _scale += Time.fixedDeltaTime;
            }

            _scale -= 0.1f * Time.fixedDeltaTime * _smogOverlay.SmogDensity;
            _transform.localScale = Vector3.one * _scale;

            if ((_scale <= 0.0f) && (_growthTimeRemaining <= 0.0f))
            {
                gameObject.SetActive(false);
            }

            _timeToNextWaterAccept = Mathf.Max(_timeToNextWaterAccept - Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider collider)
        {
            Debug.Log($"{collider.tag} - {_transform.name} - {_timeToNextWaterAccept}");

            if ((collider.tag == "Water Plant") && (_timeToNextWaterAccept <= 0.0f))
            {
                Debug.Log("Topped up!");
                _timeToNextWaterAccept = 1.0f;
                _growthTimeRemaining = 0.4f;
            }
        }
    }
}