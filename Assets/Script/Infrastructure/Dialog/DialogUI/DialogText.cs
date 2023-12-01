using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameOff2023.Dialog.DialogControl
{
    public class DialogText : MonoBehaviour {

        TMP_Text DialogUIText;
        RectTransform rectTransform;

        //[Header("不同的字体")]
        //public TMP_FontAsset CommonTextAsset;
        //public TMP_FontAsset CommonTextAsset2;
        //public TMP_FontAsset CommonTextAsset3;
        //public TMP_FontAsset CommonTextAsset4;
        //public TMP_FontAsset CommonTextAsset5;

        public void SetText(string characterTag, string ContentText, int Height) {
            ChangeTextAssetByTag(characterTag);

            DialogUIText = GetComponent<TMP_Text>();
            rectTransform = GetComponent<RectTransform>();

            DialogUIText.text = ContentText;
            //获取自适应大小
            float preferredHeight = DialogUIText.preferredHeight;

            //设置UI显示
            float width = rectTransform.sizeDelta.x;
            DialogUIText.text = "";
            rectTransform.sizeDelta = new Vector2(width, preferredHeight);
        }

        private void ChangeTextAssetByTag(string characterTag) {
            switch (characterTag) {
                case "CH2":
                    //DialogUIText.font = CommonTextAsset2;
                    break;
                case "PS":
                    //DialogUIText.font = CommonTextAsset3;
                    break;
                case "ZLF":
                    //DialogUIText.font = CommonTextAsset4;
                    break;
                case "WZR":
                    //DialogUIText.font = CommonTextAsset5;
                    break;
                default:
                    //DialogUIText.font = CommonTextAsset;
                    break;
            }
        }
    }
}