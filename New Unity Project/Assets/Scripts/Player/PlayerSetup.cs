using Enums;
using PlayingField;
using UnityEngine;

namespace Player
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private int _playerIndex = 1;
        [SerializeField] private bool _playsFromLeft = true;
        [SerializeField] private PlayerTools _startingTool = PlayerTools.Pickaxe;

        private void Start()
        {
            PlayingFieldGrid grid = FindObjectOfType<PlayingFieldGrid>();

            PlayerMovement playerMovementScript = transform.Find("Player Character Container").GetComponent<PlayerMovement>();
            ToolCarousel carouselScript = transform.Find("Tool Carousel").GetComponent<ToolCarousel>();

            carouselScript.Initialize(grid.GetCarouselPosition(_playsFromLeft), _startingTool, playerMovementScript.transform.parent.name);
            playerMovementScript.InitializePlayer($"P{_playerIndex}", grid.GetPlayerStartPosition(_playsFromLeft), _startingTool, carouselScript);
        }
    }
}