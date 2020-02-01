using System.Collections.Generic;
using Interfaces;
using Smog;
using UnityEngine;

namespace GameManagement
{
    public class GameController : MonoBehaviour
    {
        private List<ISuspendOnSmogLimitReached> _scriptsToSuspendWhenGameEnds = new List<ISuspendOnSmogLimitReached>();

        public void RegisterScriptToSuspendWhenGameEnds(ISuspendOnSmogLimitReached toRegister)
        {
            if (!_scriptsToSuspendWhenGameEnds.Contains(toRegister))
            {
                _scriptsToSuspendWhenGameEnds.Add(toRegister);
            }
        }

        private void Awake()
        {
            FindObjectOfType<SmogOverlay>().SmogLimitReached += HandleSmogLimitReached;
        }

        private void HandleSmogLimitReached(bool smogWasCleared)
        {
            foreach (ISuspendOnSmogLimitReached scriptToSuspend in _scriptsToSuspendWhenGameEnds)
            {
                scriptToSuspend.enabled = false;
            }
        }
    }
}