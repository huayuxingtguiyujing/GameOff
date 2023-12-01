using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameOff2023.GamePlay.UI {
    public class MoveValueBar : MonoBehaviour {

        [SerializeField] Transform holderLine;
        [SerializeField] List<Image> moveValueLine;
        [Header("ͼƬ��Դ")]
        [SerializeField] Sprite greenActiveSprite;
        [SerializeField] Sprite redActiveSprite;
        [SerializeField] Sprite unactiveSprite;

        public void SetMoveValueBar(int moveValue) {
            if(moveValueLine == null) {
                moveValueLine = new List<Image>();
                moveValueLine = holderLine.GetComponentsInChildren<Image>().ToList();
            }

            moveValue += 5;
            Debug.Log("the total value is : " + moveValueLine.Count + ", the value : " + moveValue);

            // moveValue ��-6��6֮�䣬������11��������movevalue��5
            for(int i = 0; i < moveValueLine.Count; i++) {
                if(i <= moveValue) {
                    moveValueLine[i].sprite = greenActiveSprite;
                } else {
                    moveValueLine[i].sprite = unactiveSprite;
                }
            }

        }

    }
}