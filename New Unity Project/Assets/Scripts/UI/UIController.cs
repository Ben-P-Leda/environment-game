using Smog;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            FindObjectOfType<SmogOverlay>().SmogLimitReached += HandleSmogLimitReached;
        }

        private void HandleSmogLimitReached(bool smogWasCleared)
        {
            if (!smogWasCleared)
            {
                _animator.SetTrigger("Lose Game");
            }
        }
    }
}