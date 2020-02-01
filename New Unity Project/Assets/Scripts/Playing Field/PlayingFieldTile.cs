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

        public Vector3 Position { get { return _transform.position; } }

        public void SetHealthyColour(Color healthyColor)
        {
            _healthyColor = healthyColor;

            _material = GetComponentInChildren<MeshRenderer>().material;

            ColourizeForDamage();
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
        }

        private void OnTriggerEnter(Collider collider)
        {
            switch (collider.tag)
            {
                case "Rain": _activeDamageSources += 1; break;
                case "Repair Ground": _repairsInProgress = true; break;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            switch (collider.tag)
            {
                case "Rain": _activeDamageSources -= 1; break;
                case "Repair Ground": _repairsInProgress = false; break;
            }
        }

        private void FixedUpdate()
        {
            bool damageValueChanged = false;

            if (_activeDamageSources > 0)
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

        private const float Seconds_Of_Damage_To_Destroy = 1.0f;
        private const float Seconds_To_Repair = 2.0f;
    }
}