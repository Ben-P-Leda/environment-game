using Common;
using Enums;
using Interfaces;
using PlayingField;
using Smog;
using UnityEngine;

namespace Plants
{
    public class PlantLifecycle : MonoBehaviour, ISmogDensityChangeModifier
    {
        private Transform _transform;
        private SmogOverlay _smogOverlay;
        private float _scale;
        private float _growthTimeRemaining;
        private float _timeToNextWaterAccept;

        public float ChangeRateModifier { get { return gameObject.activeInHierarchy ? -0.04f * _scale : 0.0f; } }
        public PlayingFieldTile TileLocation { set; private get; }
        public float HealthFraction { get { return Mathf.Clamp01(_scale); } }

        private void Awake()
        {
            _transform = transform;
            _smogOverlay = FindObjectOfType<SmogOverlay>();

            _smogOverlay.RegisterDensityChangeModifier(this);
        }

        private void OnEnable()
        {
            _scale = 0.0f;
            _growthTimeRemaining = Random.Range(0.5f, 0.55f);
            _timeToNextWaterAccept = 0.0f;

            _transform.position = TileLocation.Position;
            _transform.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            _transform.localScale = Vector3.zero;

            TileLocation.ObstructedBy = TileBlockers.Plant;
        }

        private void FixedUpdate()
        {
            if (_growthTimeRemaining > 0)
            {
                _growthTimeRemaining -= Time.fixedDeltaTime;
                _scale = Mathf.Min(1.0f, _scale + Time.fixedDeltaTime);
            }

            _scale -= 0.1f * Time.fixedDeltaTime * _smogOverlay.SmogDensity;
            _transform.localScale = Vector3.one * Mathf.Clamp01(_scale);

            if ((_scale <= 0.0f) && (_growthTimeRemaining <= 0.0f))
            {
                TileLocation.ObstructedBy = TileBlockers.None;
                AudioManager.PlaySound("plant-death");
                gameObject.SetActive(false);
            }

            _timeToNextWaterAccept = Mathf.Max(_timeToNextWaterAccept - Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if ((collider.tag == "Water Plant") && (_timeToNextWaterAccept <= 0.0f))
            {
                _timeToNextWaterAccept = 1.0f;
                _growthTimeRemaining = 0.4f;

                AudioManager.PlaySound("plant-grow");
            }
        }
    }
}