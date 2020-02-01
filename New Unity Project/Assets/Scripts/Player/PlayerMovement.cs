using System;
using System.Linq;
using Enums;
using PlayingField;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 5.0f;

        private Transform _transform;
        private Rigidbody _rigidbody;
        private Animator _animator;

        private Transform _hammer;
        private Transform _pickaxe;
        private Transform _wateringCan;

        private bool _isMoving;
        private bool _isFalling;
        private bool _actionInProgress;

        private Vector3 _startPosition;
        private Tool _activeTool;
        private ToolCarousel _carousel;

        public void InitializePlayer(Vector3 startPosition, Tool startingTool, ToolCarousel carousel)
        {
            _startPosition = startPosition;
            _carousel = carousel;

            ActivateTool(startingTool);
            Respawn();
        }

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();

            _hammer = GetComponentsInChildren<Transform>().First(x => x.name == "Hammer Container").transform;
            _pickaxe = GetComponentsInChildren<Transform>().First(x => x.name == "Pickaxe Container").transform;
            _wateringCan = GetComponentsInChildren<Transform>().First(x => x.name == "Can Container").transform;

            _animator.GetBehaviour<PlayerIdleEvent>().EnteredStateCallback += HandleEnterIdleAnimation;
        }

        private void HandleEnterIdleAnimation()
        {
            _actionInProgress = false;
        }

        private void ActivateTool(Tool toActivate)
        {
            _activeTool = toActivate;
            _pickaxe.localScale = _activeTool == Tool.Pickaxe ? Vector3.one : Vector3.zero;
            _hammer.localScale = _activeTool == Tool.Hammer ? Vector3.one : Vector3.zero;
            _wateringCan.localScale = _activeTool == Tool.Can ? Vector3.one : Vector3.zero;
        }

        private void Respawn()
        {
            _transform.position = _startPosition;

            _isMoving = false;
            _isFalling = false;
        }

        private void FixedUpdate()
        {
            HandleFalling();
            HandleActionInput();
            HandleMovementInput();
            UpdateAnimationSettings();
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
                switch (_activeTool)
                {
                    case Tool.Hammer:
                        if ((!_actionInProgress) && (Input.GetButtonDown("Jump")))
                        {
                            _actionInProgress = true;
                            _animator.SetTrigger("Attack");
                        }
                        break;
                    case Tool.Pickaxe:
                        _actionInProgress = Input.GetButton("Jump");
                        break;
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

            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")) * _movementSpeed;

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
            _animator.SetBool("Is Repairing", _actionInProgress && _activeTool == Tool.Pickaxe);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.name == _carousel.name)
            {
                Tool nextTool = (Tool)(((int)_activeTool + 1) % Enum.GetNames(typeof(Tool)).Length);
                ActivateTool(nextTool);
                _carousel.ActivateToolForPlayer(nextTool);
            }
        }

        private const float MovementThreshold = 0.01f;
        private const float Fall_Threshold = -0.1f;
        private const float Respawn_Threshold = -15.0f;
    }
}