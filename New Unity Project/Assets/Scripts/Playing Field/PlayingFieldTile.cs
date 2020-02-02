using Enums;
using UnityEngine;

namespace PlayingField
{
    public class PlayingFieldTile : MonoBehaviour
    {
        private Transform _transform;
        private Color _healthyColor;
        private Material _material;
        private float _damageFraction;
        private int _activeDamageSources;
        private bool _repairsInProgress;
        private bool _immuneToDamage;

        public int GridX { get; set; }
        public int GridZ { get; set; }

        public Vector3 Position { get { return _transform.position; } }
        public TileBlockers ObstructedBy { get; set; }
        public bool AvailableForObjectPlacement { get {  return gameObject.activeInHierarchy && ObstructedBy == TileBlockers.None;} }

        public void SetHealthyColour(Color healthyColor)
        {
            _healthyColor = healthyColor;

            _material = GetComponentInChildren<MeshRenderer>().material;

            ColourizeForDamage();
        }

        public void SetAsPathTile(Color pathColor)
        {
            _immuneToDamage = true;
            _material = GetComponentInChildren<MeshRenderer>().material;
            _material.color = pathColor;

            ObstructedBy = TileBlockers.Path;
        }

        private void ColourizeForDamage()
        {
            float colorScaler = _damageFraction * 2.0f;
            _material.color = colorScaler <= 1.0f
                ? Color.Lerp(_healthyColor, Color.black, colorScaler)
                : Color.Lerp(Color.black, Color.clear, colorScaler - 1.0f);
        }

        private void Awake()
        {
            _transform = transform;
            _activeDamageSources = 0;
            _repairsInProgress = false;

            ObstructedBy = TileBlockers.None;
        }

        private void OnTriggerEnter(Collider collider)
        {
            HandleTriggerEvent(collider.tag, true);
        }

        private void OnTriggerExit(Collider collider)
        {
            HandleTriggerEvent(collider.tag, false);
        }

        private void HandleTriggerEvent(string tag, bool enteredCollision)
        {
            if (!_immuneToDamage)
            {
                switch (tag)
                {
                    case "Rain": _activeDamageSources += enteredCollision ? 1 : -1; break;
                    case "Repair Ground": _repairsInProgress = enteredCollision; break;
                }
            }
        }

        private void FixedUpdate()
        {
            bool damageValueChanged = false;

            if ((_activeDamageSources > 0) && (ObstructedBy != TileBlockers.Plant) && (ObstructedBy != TileBlockers.PlayerStartPoint))
            {
                _damageFraction = Mathf.Clamp01(_damageFraction + (Seconds_Of_Damage_To_Destroy * Time.fixedDeltaTime * _activeDamageSources));
                damageValueChanged = true;
            }

            if (_repairsInProgress)
            {
                _damageFraction = Mathf.Clamp01(_damageFraction - (Seconds_To_Repair * Time.fixedDeltaTime));
                damageValueChanged = true;
            }

            if (damageValueChanged)
            {
                ColourizeForDamage();
            }

            if (_damageFraction >= 1.0f)
            {
                gameObject.SetActive(false);
            }
        }

        private const float Seconds_Of_Damage_To_Destroy = 5.0f;
        private const float Seconds_To_Repair = 3.0f;
    }
}