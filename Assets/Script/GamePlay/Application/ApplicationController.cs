using GameOff2023.Infrastructure.Audio;
using GameOff2023.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.GamePlay.Application {
    public class ApplicationController : MonoBehaviour{

        public static ApplicationController Instance;

        [SerializeField] FailPanel failPanel;

        [SerializeField] AudioManager audioManager;

        private void Awake() {
            Instance = this;

            audioManager.InitAudioManager();
        }

        public void TriggerDialog() {

        }

        public void PlayerFail(string levelName) {
            failPanel.gameObject.SetActive(true);
            failPanel.InvokeFailPanel(levelName);
        }

        public void GameOver() {
            Debug.Log("”Œœ∑Ω· ¯£°");
        }

        public void QuitGame() {

        }

    }
}