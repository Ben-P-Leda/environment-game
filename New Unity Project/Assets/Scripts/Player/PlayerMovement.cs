using PlayingField;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private int _startGridX = 10;
        [SerializeField] private int _startGridZ = 5;
        [SerializeField] private float _movementSpeed = 5.0f;

        private Transform _transform;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private PlayingFieldGrid _playingFieldGrid;
        private bool _isMoving;
        private bool _isFalling;
        private bool _isRepairing;

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _playingFieldGrid = FindObjectOfType<PlayingFieldGrid>();
        }

        private void Start()
        {
            Respawn();
        }

        private void Respawn()
        {
            _isMoving = false;
            _isFalling = false;
            _isRepairing = false;
            _transform.position = _playingFieldGrid.TileGrid[_startGridX][_startGridZ].Position;
        }

        private void FixedUpdate()
        {
            HandleFalling();
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

        private void HandleMovementInput()
        {
            if (_isFalling)
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
        }

        private const float MovementThreshold = 0.01f;
        private const float Fall_Threshold = -0.1f;
        private const float Respawn_Threshold = -15.0f;
    }
}