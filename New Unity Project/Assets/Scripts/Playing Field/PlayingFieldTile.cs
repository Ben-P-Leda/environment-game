using UnityEngine;

namespace PlayingField
{
    public class PlayingFieldTile : MonoBehaviour
    {
        private Transform _transform;
        private Color _healthyColor;
        private Material _material;
        private float _damageFraction;

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
        }

        private void OnTriggerEnter(Collider collider)
        {
            Debug.Log($"{_transform.name} entered collision with {collider.name}");
        }

        private void OnTriggerExit(Collider collider)
        {
            Debug.Log($"{_transform.name} left collision with {collider.name}");
        }
    }
}