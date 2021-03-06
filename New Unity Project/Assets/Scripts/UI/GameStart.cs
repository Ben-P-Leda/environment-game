﻿using System.Linq;
using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameStart : MonoBehaviour
    {
        [SerializeField] private Sprite _awaitingPlayerImage = null;

        private bool _playerOneReady;
        private bool _playerTwoReady;

        private SpriteRenderer _promptRenderer;

        private void Awake()
        {
            _promptRenderer = FindObjectsOfType<SpriteRenderer>().First(x => x.name == "Prompt");
        }

        private void OnEnable()
        {
            _playerOneReady = false;
            _playerTwoReady = false;
        }

        private void Start()
        {
            AudioManager.MusicPlaying = true;
        }

        private void FixedUpdate()
        {
            bool playerOnePress = Input.GetButtonDown("P1:Action");
            bool playerTwoPress = Input.GetButtonDown("P2:Action");

            if ((playerOnePress) || (playerTwoPress))
            {
                AudioManager.PlaySound("UI Selection");

                if ((BothPlayersReady(playerOnePress, _playerTwoReady)) || (BothPlayersReady(playerTwoPress, _playerOneReady)))
                {
                    AudioManager.MusicPlaying = false;
                    SceneManager.LoadScene("Gameplay Scene");
                }
                else
                {
                    _promptRenderer.sprite = _awaitingPlayerImage;
                    _playerOneReady |= playerOnePress;
                    _playerTwoReady |= playerTwoPress;
                }
            }
        }

        private bool BothPlayersReady(bool buttonPresser, bool alreadyReady)
        {
            return buttonPresser && alreadyReady;
        }
    }
}