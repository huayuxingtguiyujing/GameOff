using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameOff2023.Dialog.DialogControl
{
    public class DialogOptionButton : MonoBehaviour {
        
        public int NextId;
        public bool canChoose;

        #region UI组件
        RectTransform rectTransform;

        public Button OptionButton;
        public TMP_Text OptionText;
        #endregion

        public void SetOption(string ContentText, int Id, bool CanChoose, int OptionHeight, Action optionCallback){
            #region 绑定UI组件
            rectTransform = GetComponent<RectTransform>();
            OptionButton = GetComponentInChildren<Button>();
            OptionText = GetComponentInChildren<TMP_Text>();
            #endregion

            //配置选项
            OptionText.text = ContentText;
            NextId = Id;

            if (!CanChoose) {
                //选项不可选
                SetDisable();
            }

            //获取自适应大小
            float preferredHeight = OptionText.preferredHeight;
            float width = rectTransform.sizeDelta.x;
            rectTransform.sizeDelta = new Vector2(width, preferredHeight);

            OptionButton.onClick.AddListener(() => {
                optionCallback.Invoke();
            });
        }

        public void SetDisable() {
            OptionText.text = "<s>" + OptionText.text + "</s>";
            Debug.Log("set button disable");
            OptionText.richText = true;
            OptionButton.enabled = false;
            OptionText.color = Color.grey;
        }

    }
}