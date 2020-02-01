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

            //FindObjectOfType<SmogOverlay>().SmogLimitReached += HandleSmogLimitReached;
            FindObjectOfType<PlantPool>().AllPlantsHaveDied += DisplayGameOver;
        }

        private void DisplayGameOver()
        {
            _animator.SetTrigger("Lose Game");
        }
    }
}