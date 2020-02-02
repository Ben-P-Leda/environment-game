using System.Collections.Generic;
using Interfaces;
using Plants;
using Smog;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    public class GameController : MonoBehaviour
    {
        private SmogOverlay _smogOverlay;
        private List<ISuspendOnSmogLimitReached> _scriptsToSuspendWhenGameEnds = new List<ISuspendOnSmogLimitReached>();
        private bool _gameOver;
        private float _exitBlockTimer;

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

            _exitBlockTimer = 2.0f;
            _gameOver = true;
        }

        private void OnEnable()
        {
            _gameOver = false;
        }

        private void FixedUpdate()
        {
            if ((_gameOver) && (_exitBlockTimer <= 0.0f) && ((Input.GetButtonDown("P1:Action")) || (Input.GetButtonDown("P2:Action"))))
            {
                SceneManager.LoadScene("Start Scene");
            }

            _exitBlockTimer = Mathf.Max(0.0f, _exitBlockTimer - Time.fixedDeltaTime);
        }
    }
}