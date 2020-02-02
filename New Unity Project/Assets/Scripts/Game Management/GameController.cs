using System.Collections.Generic;
using Interfaces;
using Plants;
using Smog;
using UI;
using UnityEngine;

namespace GameManagement
{
    public class GameController : MonoBehaviour
    {
        private SmogOverlay _smogOverlay;
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
            FindObjectOfType<SmogOverlay>().SmogCleared += HandleGameEnded;
            FindObjectOfType<PlantPool>().AllPlantsHaveDied += HandleGameEnded;
        }

        private void HandleGameEnded()
        {
            foreach (ISuspendOnSmogLimitReached scriptToSuspend in _scriptsToSuspendWhenGameEnds)
            {
                scriptToSuspend.enabled = false;
            }
        }
    }
}