using System;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationEvent : MonoBehaviour
    {
        public event Action<string> NotifyAnimationEvent;

        private void DispatchAnimationEvent(string message)
        {
            NotifyAnimationEvent?.Invoke(message);
        }
    }
}