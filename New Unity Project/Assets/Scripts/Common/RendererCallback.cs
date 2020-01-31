using System;
using UnityEngine;

namespace Common
{
    public class RendererCallback : MonoBehaviour
    {
        public event Action<bool> VisibleStateChanged;

        private void OnBecameVisible()
        {
            VisibleStateChanged?.Invoke(true);
        }

        private void OnBecameInvisible()
        {
            VisibleStateChanged?.Invoke(false);
        }
    }
}