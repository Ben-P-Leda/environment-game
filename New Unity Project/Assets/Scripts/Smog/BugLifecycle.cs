﻿using Common;
using Effects;
using Enums;
using GameManagement;
using Interfaces;
using PlayingField;
using UnityEngine;

namespace Smog
{
    public class BugLifecycle : MonoBehaviour, ISmogDensityChangeModifier, ISuspendOnSmogLimitReached
    {
        private Transform _transform;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private PlayingFieldGrid _playingFieldGrid;
        private SmogOverlay _smogOverlay;
        private PlayingFieldTile _destinationTile;
        private ParticleSystem _exhaledParticles;
        private AudioSource _soundEffect;

        private RigidbodyConstraints _startingConstraints;

        private bool _entryComplete;
        private float _scale;

        public float ChangeRateModifier { get { return gameObject.activeInHierarchy && _entryComplete ? 0.3f * _scale : 0.0f; } }
        public ParticlePool SplatParticles { private get; set; }

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _exhaledParticles = GetComponentInChildren<ParticleSystem>();
            _soundEffect = GetComponent<AudioSource>();
            _playingFieldGrid = FindObjectOfType<PlayingFieldGrid>();

            _startingConstraints = _rigidbody.constraints;

            FindObjectOfType<SmogOverlay>().RegisterDensityChangeModifier(this);
            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);

            GetComponentInChildren<AnimationEventListener>().NotifyAnimationEvent += HandleAnimationEvent;
        }

        private void HandleAnimationEvent(string message)
        {
            if (message == "Start Smog")
            {
                _exhaledParticles.Play();
            }
            else if (message == "Stop Smog")
            {
                _exhaledParticles.Stop();
            }
        }

        private void OnEnable()
        {
            _destinationTile = _playingFieldGrid.GetRandomTile(true);
            if (_destinationTile == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _scale = 0.5f;
            _entryComplete = false;

            _destinationTile.ObstructedBy = TileBlockers.Bug;
            _transform.position = _destinationTile.Position + (Vector3.up * 12.0f);
            _transform.localScale = Vector3.one * _scale;

            _animator.enabled = true;
            _rigidbody.constraints = _startingConstraints;
            _rigidbody.velocity = new Vector3(0.0f, -1.0f, 0.0f);

            _soundEffect.volume = 0.0f;
        }

        private void OnDisable()
        {
            _soundEffect.volume = 0.0f;
        }

        private void FixedUpdate()
        {
            if (_transform.position.y <= 0.01f)
            {
                _entryComplete = true;
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                _animator.SetTrigger("Has Landed");
                _soundEffect.volume = 0.5f;
            }
            else
            {
                _soundEffect.volume = Mathf.Clamp01(_soundEffect.volume + Time.fixedDeltaTime);
            }

            if (_entryComplete)
            {
                _scale = Mathf.Clamp01(_scale + Time.fixedDeltaTime * 0.03f);
                _transform.localScale = Vector3.one * _scale;

                if (!_destinationTile.gameObject.activeInHierarchy)
                {
                    ChangeDestinationTile();
                }
            }
        }

        private void ChangeDestinationTile()
        {
            // TODO: Sort this logic!
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Damage Collider")
            {
                AudioManager.PlayRandomSound("bug-splat");
                SplatParticles.LaunchParticleEffect(_transform.position, _transform.localScale);
                _destinationTile.ObstructedBy = TileBlockers.None;
                gameObject.SetActive(false);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((collision.transform.name == "Player Character Container") && (!_entryComplete) && (_transform.position.y > 1.6))
            {
                ChangeDestinationTile();
            }
        }
    }
}