using Enums;
using PlayingField;
using UnityEngine;

namespace Smog
{
    public class BugLifecycle : MonoBehaviour
    {
        private Transform _transform;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private PlayingFieldGrid _playingFieldGrid;
        private bool _entryComplete;

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _playingFieldGrid = FindObjectOfType<PlayingFieldGrid>();
        }

        private void OnEnable()
        {
            PlayingFieldTile targetTile = _playingFieldGrid.GetRandomTile(true);
            if (targetTile == null)
            {
                gameObject.SetActive(false);
                return;
            }

            targetTile.ObstructedBy = TileBlockers.Bug;
            _transform.position = targetTile.Position + (Vector3.up * 20.0f);
            _transform.localScale = Vector3.one * 0.5f;

            _rigidbody.velocity = new Vector3(0.0f, -5.0f, 0.0f);
            _entryComplete = false;
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