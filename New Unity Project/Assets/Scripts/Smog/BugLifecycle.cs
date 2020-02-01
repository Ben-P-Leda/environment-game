using Common;
using Enums;
using PlayingField;
using UnityEngine;

namespace Smog
{
    public class BugLifecycle : MonoBehaviour, ISmogDensityChangeModifier
    {
        private Transform _transform;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private PlayingFieldGrid _playingFieldGrid;
        private SmogOverlay _smogOverlay;
        private bool _entryComplete;
        private float _scale;

        public float ChangeRateModifier { get { return gameObject.activeInHierarchy && _entryComplete ? 0.4f * _scale : 0.0f; } }

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _playingFieldGrid = FindObjectOfType<PlayingFieldGrid>();

            FindObjectOfType<SmogOverlay>().RegisterDensityChangeModifier(this);
        }

        private void OnEnable()
        {
            PlayingFieldTile targetTile = _playingFieldGrid.GetRandomTile(true);
            if (targetTile == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _scale = 0.5f;

            targetTile.ObstructedBy = TileBlockers.Bug;
            _transform.position = targetTile.Position + (Vector3.up * 20.0f);
            _transform.localScale = Vector3.one * _scale;

            _rigidbody.velocity = new Vector3(0.0f, -5.0f, 0.0f);
        }

        private void FixedUpdate()
        {
            if (_transform.position.y <= 0.0f)
            {
                _entryComplete = true;
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                _animator.SetTrigger("Has Landed");
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Damage Collider")
            {
                gameObject.SetActive(false);
            }
        }
    }
}