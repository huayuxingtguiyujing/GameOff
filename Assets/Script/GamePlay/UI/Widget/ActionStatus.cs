using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameOff2023.GamePlay.UI {
    public class ActionStatus : MonoBehaviour {

        [SerializeField] Image leftPointImage;
        [SerializeField] Image lineImage;
        [SerializeField] Image rightPointImage;

        [Header("Í¼Æ¬×ÊÔ´")]
        [SerializeField] Sprite ableAction;
        [SerializeField] Sprite unableAction;
        [SerializeField] Sprite ableLine;
        [SerializeField] Sprite unableLine;

        public void SetActionStatus(int playerActionNum) {
            if(playerActionNum == 2) {
                leftPointImage.sprite = ableAction;
                lineImage.sprite = ableLine;
                rightPointImage.sprite = ableAction;
            } else if (playerActionNum == 1) {
                leftPointImage.sprite = ableAction;
                lineImage.sprite = ableLine;
                rightPointImage.sprite = unableAction;
            } else {
                leftPointImage.sprite = unableAction;
                lineImage.sprite = unableLine;
                rightPointImage.sprite = unableAction;
            }
        }

    }
}