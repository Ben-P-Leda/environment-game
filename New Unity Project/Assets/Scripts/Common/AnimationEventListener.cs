using System;
using UnityEngine;

namespace Common
{
    public class AnimationEventListener : MonoBehaviour
    {
        public event Action<string> NotifyAnimationEvent;

        private void DispatchAnimationEvent(string message)
        {
            NotifyAnimationEvent?.Invoke(message);
        }

        private void PlaySoundEffect(string effect)
        {
            AudioManager.PlaySound(effect);
        }

        private void PlayRandomSoundEffect(string effect)
        {
            AudioManager.PlayRandomSound(effect);
        }
    }
}