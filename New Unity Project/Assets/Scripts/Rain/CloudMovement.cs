using Common;
using GameManagement;
using Interfaces;
using PlayingField;
using UnityEngine;

namespace Rain
{
    public class CloudMovement : MonoBehaviour, ISuspendOnSmogLimitReached
    {
        private Transform _transform;
        private Rigidbody _rigidbody;
        private PlayingFieldGrid _grid;

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _grid = FindObjectOfType<PlayingFieldGrid>();

            GetComponentInChildren<RendererCallback>().VisibleStateChanged += HandleVisibilityStateChange;
            FindObjectOfType<GameController>().RegisterScriptToSuspendWhenGameEnds(this);
        }

        private void HandleVisibilityStateChange(bool becameVisible)
        {
            if (!becameVisible)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            Vector3 targetLocation = _grid.GetRandomTile(false).Position;
            Vector3 direction = Quaternion.Euler(0.0f, Random.Range(20.0f, 160.0f), 0.0f) * Vector3.right;

            _transform.position = new Vector3(targetLocation.x, Random.Range(5.0f, 7.0f), targetLocation.z) - (direction * 20.0f);
            _rigidbody.velocity = direction * Random.Range(1.0f, 5.0f);
        }
    }
}