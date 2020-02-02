using System;
using System.Linq;
using Common;
using Enums;
using GameManagement;
using Interfaces;
using PlayingField;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour, ISuspendOnSmogLimitReached
    {
        [SerializeField] private float _movementSpeed = 5.0f;

        private Transform _transform;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private ParticleSystem _contactParticles;

        private Transform _hammer;
        private Transform _pickaxe;
        private Transform _wateringCan;

        private bool _isMoving;
        private bool _isFalling;
        private bool _actionInProgress;
        private bool _inProximityWithOtherPlayer;
        private bool _isAtCarousel;
        private float _actionCooldown;
        private float _waterInCan;

        private Vector3 _startPosition;
        private PlayerTools _activeTool;
        private ToolCarousel _carousel;

        private string _controllerPrefix;

        public event Action<PlayerTools, bool> SwapTools;

        public void InitializePlayer(string controllerPrefix, Vector3 startPosition, PlayerTools startingTool, ToolCarousel carousel)
        {
            _controllerPrefix = controllerPrefix;
            _startPosition = startPosition;
            _carousel = carousel;

            ActivateTool(startingTool, 0.0f);
            Respawn();
        }

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _contactParticles = GetComponentInChildren<ParticleSystem>();

            _hammer = GetComponentsInChildren<Transform>().First(x => x.name == "Hammer Container").transform;
            _pickaxe = GetComponentsInChildren<Transform>().First(x => x.name == "Pickaxe Container").transform;
            _wateringCan = GetComponentsInChildren<Transform>().First(x => x.name == "Can Container").transform;

            _animator.GetBehaviour<PlayerIdleEvent>().EnteredStateCallback += HandleEnterIdleAnimation;

            GetComponentInChildren<ProximityDetector>().EnteredProximity += HandleProximityStateChange;
            GetComponentInChildren<AnimationEventListener>().NotifyAnimationEvent += HandleAnimationEvent;

            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);
            FindObjectsOfType<PlayerMovement>().First(x => x != this).SwapTools += HandleToolSwap;
        }

        private void HandleEnterIdleAnimation()
        {
            _actionInProgress = false;
        }

        private void HandleProximityStateChange(bool enteredProximity)
        {
            _inProximityWithOtherPlayer = enteredProximity;

            if (enteredProximity)
            {
                _contactParticles.Play();
            }
            else
            {
                _contactParticles.Stop();
            }
        }

        private void HandleAnimationEvent(string message)
        {
            if (message == "Use Water")
            {
                _waterInCan = Mathf.Max(_waterInCan - 1.0f, 0.0f);
            }
        }

        private void HandleToolSwap(PlayerTools toActivate, bool originalSwapRequest)
        {
            if (originalSwapRequest)
            {
                SwapTools?.Invoke(_activeTool, false);
            }

            ActivateTool(toActivate, 0.5f);
        }

        private void ActivateTool(PlayerTools toActivate, float cooldownDuration)
        {
            _activeTool = toActivate;
            _pickaxe.localScale = _activeTool == PlayerTools.Pickaxe ? Vector3.one : Vector3.zero;
            _hammer.localScale = _activeTool == PlayerTools.Hammer ? Vector3.one : Vector3.zero;
            _wateringCan.localScale = _activeTool == PlayerTools.Can ? Vector3.one : Vector3.zero;

            if (toActivate == PlayerTools.Can)
            {
                _waterInCan = 5.0f;
            }

            _carousel.ActivateToolForPlayer(toActivate);
            _actionCooldown = cooldownDuration;
        }

        private void Respawn()
        {
            _transform.position = _startPosition;

            _isMoving = false;
            _isFalling = false;
            _actionInProgress = false;
            _inProximityWithOtherPlayer = false;
            _isAtCarousel = false;
            _actionCooldown = 0.0f;
            _waterInCan = 5.0f;
        }

        private void FixedUpdate()
        {
            HandleFalling();
            HandleActionInput();
            HandleMovementInput();
            UpdateAnimationSettings();

            _actionCooldown = Mathf.Max(_actionCooldown - Time.fixedDeltaTime, 0.0f);
        }

        private void HandleFalling()
        {
            if (_transform.position.y < Fall_Threshold)
            {
                _isFalling = true;
            }

            if (_transform.position.y < Respawn_Threshold)
            {
                Respawn();
            }
        }

        private void HandleActionInput()
        {
            if (!_isFalling)
            {
                if (_inProximityWithOtherPlayer)
                {
                    if ((_actionCooldown <= 0.0f) && (Input.GetButtonDown($"{_controllerPrefix}:Action")))
                    {
                        SwapTools?.Invoke(_activeTool, true);
                    }
                }
                else if ((_isAtCarousel) && (Input.GetButtonDown($"{_controllerPrefix}:Action")))
                {
                    if (_actionCooldown <= 0.0f)
                    {
                        PlayerTools nextTool = (PlayerTools) (((int) _activeTool + 1) % Enum.GetNames(typeof(PlayerTools)).Length);
                        ActivateTool(nextTool, 0.25f);
                    }
                }
                else if ((_activeTool == PlayerTools.Hammer) && (!_actionInProgress) && (Input.GetButtonDown($"{_controllerPrefix}:Action")))
                {
                    _actionInProgress = true;
                    _animator.SetTrigger("Attack");
                }
                else if ((_activeTool == PlayerTools.Pickaxe) || ((_activeTool == PlayerTools.Can) && (_actionCooldown <= 0.0f)))
                {
                    _actionInProgress = Input.GetButton($"{_controllerPrefix}:Action");
                }
            }
        }

        private void HandleMovementInput()
        {
            if ((_isFalling) || (_actionInProgress))
            {
                StopHorizontalMovement();
                return;
            }

            Vector3 direction = new Vector3(Input.GetAxis($"{_controllerPrefix}:Horizontal"), 0.0f, Input.GetAxis($"{_controllerPrefix}:Vertical")) * _movementSpeed;

            if (direction.magnitude > MovementThreshold)
            {
                _transform.LookAt(_transform.position + direction);
                _isMoving = true;
                _rigidbody.velocity = new Vector3(direction.x, _rigidbody.velocity.y, direction.z);
            }
            else
            {
                StopHorizontalMovement();
            }
        }

        private void StopHorizontalMovement()
        {
            _isMoving = false;
            _rigidbody.velocity = new Vector3(0.0f, _rigidbody.velocity.y, 0.0f);
        }

        private void UpdateAnimationSettings()
        {
            _animator.SetBool("Is Moving", _isMoving);
            _animator.SetBool("Is Falling", _isFalling);
            _animator.SetBool("Is Repairing", _actionInProgress && _activeTool == PlayerTools.Pickaxe);
            _animator.SetBool("Is Watering", _actionInProgress && _activeTool == PlayerTools.Can && _waterInCan > 0.0f);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.name == _carousel.name)
            {
                _isAtCarousel = true;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.name == _carousel.name)
            {
                _isAtCarousel = false;
            }
        }

        private const float MovementThreshold = 0.01f;
        private const float Fall_Threshold = -0.5f;
        private const float Respawn_Threshold = -15.0f;
    }
}