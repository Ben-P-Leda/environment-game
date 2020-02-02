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

            FindObjectOfType<SmogOverlay>().SmogCleared += DisplayGameWon;
            FindObjectOfType<PlantPool>().AllPlantsHaveDied += DisplayGameOver;
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