using Smog;
using UnityEngine;

namespace PlayingField
{
    public class WinBackground : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private bool _gameHasBeenWon;
        private float _tintModifier;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _gameHasBeenWon = false;
            _tintModifier = 0.0f;

            FindObjectOfType<SmogOverlay>().SmogCleared += HandleGameWon;
        }

        private void HandleGameWon()
        {
            _gameHasBeenWon = true;
        }

        private void FixedUpdate()
        {
            if (_gameHasBeenWon)
            {
                _tintModifier = Mathf.Clamp01(_tintModifier + Time.fixedDeltaTime);
                _spriteRenderer.color = Color.Lerp(Color.clear, Color.white, _tintModifier);
            }
        }
    }
}