using Common;
using UnityEngine;

namespace Rain
{
    public class CloudPool : MonoBehaviour
    {
        [SerializeField] private GameObject _cloudPrefab = null;
        [SerializeField] private int _poolSize = 5;

        private ObjectPool _cloudPool;

        private void Awake()
        {
            _cloudPool = new ObjectPool(_cloudPrefab, _poolSize, transform);
        }

        private void FixedUpdate()
        {
            //if (Input.anyKeyDown)
            //{
            //    _cloudPool.GetFirstAvailable().SetActive(true);
            //}
        }
    }
}