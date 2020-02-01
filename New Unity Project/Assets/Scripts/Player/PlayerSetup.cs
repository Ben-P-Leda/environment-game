using Enums;
using PlayingField;
using UnityEngine;

namespace Player
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private bool _playsFromLeft = true;
        [SerializeField] private PlayerTools _startingTool = PlayerTools.Pickaxe;

        private void Start()
        {
            PlayingFieldGrid grid = FindObjectOfType<PlayingFieldGrid>();

            ToolCarousel carouselScript = transform.Find("Tool Carousel").GetComponent<ToolCarousel>();
            carouselScript.Initialize(grid.GetCarouselPosition(_playsFromLeft), _startingTool);

            PlayerMovement playerMovementScript = transform.Find("Player Character Container").GetComponent<PlayerMovement>();
            playerMovementScript.InitializePlayer(grid.GetPlayerStartPosition(_playsFromLeft), _startingTool, carouselScript);
        }
    }
}