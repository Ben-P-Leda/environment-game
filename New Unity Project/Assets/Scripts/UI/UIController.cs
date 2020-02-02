using Common;
using Plants;
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

            GetComponent<AnimationEventListener>().NotifyAnimationEvent += HandleAnimationEvent;

            FindObjectOfType<SmogOverlay>().SmogCleared += DisplayGameWon;
            FindObjectOfType<PlantPool>().AllPlantsHaveDied += DisplayGameOver;

            Time.timeScale = 0.0f;
        }

        private void HandleAnimationEvent(string message)
        {
            Time.timeScale = 1.0f;
        }

        private void DisplayGameWon()
        {
            _animator.SetTrigger("Win Game");
        }

        private void DisplayGameOver()
        {
            _animator.SetTrigger("Lose Game");
        }
    }
}