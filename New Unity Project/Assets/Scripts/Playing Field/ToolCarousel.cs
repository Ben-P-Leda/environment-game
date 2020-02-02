using System.Linq;
using Enums;
using UnityEngine;

namespace PlayingField
{
    public class ToolCarousel : MonoBehaviour
    {
        private Transform _transform;

        private Transform _hammer;
        private Transform _pickaxe;
        private Transform _wateringCan;
        private ParticleSystem _contactParticles;

        private bool _isMoving;
        private bool _isFalling;
        private bool _actionInProgress;
        private string _owningPlayer;

        private PlayerTools _activeTool;

        public void Initialize(Vector3 position, PlayerTools startingSelectedTool, string owningPlayer)
        {
            _transform.position = position;
            _owningPlayer = owningPlayer;

            ActivateToolForPlayer(startingSelectedTool);
        }

        public void ActivateToolForPlayer(PlayerTools toActivate)
        {
            _activeTool = toActivate;
            _pickaxe.localScale = _activeTool != PlayerTools.Pickaxe ? Vector3.one : Vector3.zero;
            _hammer.localScale = _activeTool != PlayerTools.Hammer ? Vector3.one : Vector3.zero;
            _wateringCan.localScale = _activeTool != PlayerTools.Can ? Vector3.one : Vector3.zero;
        }

        private void Awake()
        {
            _transform = transform;

            _hammer = GetComponentsInChildren<Transform>().First(x => x.name == "Hammer Axle").transform;
            _pickaxe = GetComponentsInChildren<Transform>().First(x => x.name == "Pick Axle").transform;
            _wateringCan = GetComponentsInChildren<Transform>().First(x => x.name == "Can Axle").transform;

            _contactParticles = GetComponentInChildren<ParticleSystem>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.transform.parent.name == _owningPlayer)
            {
                _contactParticles.Play();
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.transform.parent.name == _owningPlayer)
            {
                _contactParticles.Stop();
            }
        }
    }
}