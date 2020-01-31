using UnityEngine;

namespace Common
{
    public class ObjectPool
    {
        private GameObject[] _objectPool;

        public delegate void CreationCallback(GameObject newObject);

        public ObjectPool(GameObject prefab, int poolSize, Transform parentTransform)
            : this(prefab, poolSize, parentTransform, null) { }

        public ObjectPool(GameObject prefab, int poolSize, Transform parentTransform, CreationCallback creationCallback)
        {
            _objectPool = new GameObject[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = GameObject.Instantiate(prefab);
                newObject.transform.parent = parentTransform;
                newObject.SetActive(false);
                newObject.name = newObject.name + " [" + i + "]";

                creationCallback?.Invoke(newObject);

                _objectPool[i] = newObject;
            }
        }

        public GameObject GetFirstAvailable()
        {
            GameObject firstAvailable = null;

            for (int i = 0; ((i < _objectPool.Length) && (firstAvailable == null)); i++)
            {
                if (!_objectPool[i].activeInHierarchy)
                {
                    firstAvailable = _objectPool[i];
                }
            }

            return firstAvailable;
        }

        public int AvailableObjectCount
        {
            get
            {
                int available = 0;
                for (int i = 0; i < _objectPool.Length; i++)
                {
                    available += (_objectPool[i].activeInHierarchy ? 0 : 1);
                }
                return available;
            }
        }
    }
}
