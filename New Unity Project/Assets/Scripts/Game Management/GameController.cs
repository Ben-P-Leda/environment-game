using System.Collections;
using System.Collections.Generic;
using Common;
using Interfaces;
using Plants;
using Smog;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    public class GameController : MonoBehaviour
    {
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

            AudioManager.MusicPlaying = false;

            _exitBlockTimer = 2.0f;
            _gameOver = true;
        }

        private void OnEnable()
        {
            _gameOver = false;
        }

        private void Start()
        {
            AudioManager.MusicPlaying = true;
        }

        private void FixedUpdate()
        {
            if (_gameOver)
            {
                if ((_exitBlockTimer <= 0.0f) && ((Input.GetButtonDown("P1:Action")) || (Input.GetButtonDown("P2:Action"))))
                {
                    StartCoroutine(ExitToTitleScene());
                }

                _exitBlockTimer = Mathf.Max(0.0f, _exitBlockTimer - Time.fixedDeltaTime);
            }
        }

        private IEnumerator ExitToTitleScene()
        {
            _gameOver = false;

            AudioManager.PlaySound("UI Selection");

            yield return new WaitForSeconds(1.0f);

            SceneManager.LoadScene("Start Scene");
        }
    }
}