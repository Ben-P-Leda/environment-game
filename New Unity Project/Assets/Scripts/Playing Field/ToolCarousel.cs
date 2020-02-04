using System.Linq;
using Enums;
using UnityEngine;

namespace PlayingField
{
    public class ToolCarousel : MonoBehaviour
    {
        [SerializeField] private Color _inactiveColor = Color.grey;
        [SerializeField] private Color _activeColor = Color.white;

        private Transform _transform;

        private Transform _hammer;
        private Transform _pickaxe;
        private Transform _wateringCan;
        private Transform _base;
        private Material _baseMaterial;
        private ParticleSystem _contactParticles;

        private string _owningPlayer;
        private float _baseDisplayModifier;
        private bool _ownerIsInContact;

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
            _base = GetComponentsInChildren<Transform>().First(x => x.name == "Base").transform;
            _baseMaterial = _base.GetComponent<MeshRenderer>().material;
            _contactParticles = GetComponentInChildren<ParticleSystem>();

            _baseDisplayModifier = 0.0f;
            _ownerIsInContact = false;
            SetBaseDisplay(_baseDisplayModifier);
        }

        private void SetBaseDisplay(float delta)
        {
            _baseDisplayModifier = Mathf.Clamp01(_baseDisplayModifier + delta);
            _baseMaterial.color = Color.Lerp(_inactiveColor, _activeColor, _baseDisplayModifier);

            float scale = Mathf.Lerp(0.0f, 1.5f, _baseDisplayModifier);
            _base.localScale = new Vector3(scale, _base.localScale.y, scale);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.transform.parent.name == _owningPlayer)
            {
                _contactParticles.Play();
                _ownerIsInContact = true;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.transform.parent.name == _owningPlayer)
            {
                _contactParticles.Stop();
                _ownerIsInContact = false;
            }
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime * (_ownerIsInContact ? 1.0f : -1.0f) * 2.0f;
            SetBaseDisplay(delta);
        }
    }
}