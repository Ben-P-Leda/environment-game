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

        private bool _isMoving;
        private bool _isFalling;
        private bool _actionInProgress;

        private Tool _activeTool;

        public void Initialize(Vector3 position, Tool startingSelectedTool)
        {
            _transform.position = position;

            ActivateToolForPlayer(startingSelectedTool);
        }

        public void ActivateToolForPlayer(Tool toActivate)
        {
            _activeTool = toActivate;
            _pickaxe.localScale = _activeTool != Tool.Pickaxe ? Vector3.one : Vector3.zero;
            _hammer.localScale = _activeTool != Tool.Hammer ? Vector3.one : Vector3.zero;
            _wateringCan.localScale = _activeTool != Tool.Can ? Vector3.one : Vector3.zero;
        }

        private void Awake()
        {
            _transform = transform;

            _hammer = GetComponentsInChildren<Transform>().First(x => x.name == "Hammer Axle").transform;
            _pickaxe = GetComponentsInChildren<Transform>().First(x => x.name == "Pick Axle").transform;
            _wateringCan = GetComponentsInChildren<Transform>().First(x => x.name == "Can Axle").transform;
        }
    }
}