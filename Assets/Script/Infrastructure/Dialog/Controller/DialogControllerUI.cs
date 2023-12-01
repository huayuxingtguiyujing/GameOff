using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameOff2023.Dialog.DialogControl
{
    public class DialogControllerUI : MonoBehaviour {
        #region UI组件
        [Header("对话框UI组件")]
        public Image CharaterProfileImage;
        public TMP_Text CharacterNameText;
        public Image NPCProfileImage;
        public TMP_Text NPCNameText;

        public Button ContinueButton;

        public GameObject ContentPart;
        #endregion

        #region Prefab
        [Header("预制件")]
        public GameObject DialogText;
        public GameObject DialogOption;
        #endregion

        #region 控制所需变量
        [Header("动画效果")]
        protected int CurrentTalkLength;
        protected int AddLength = 140;

        protected Tweener TextEmergeTwn;          //文字浮现动画控件
        #endregion

        protected void UpdateCharacterProfile(string characterTag, string nPCTag) {
            //CharaterProfileImage.gameObject.SetActive(true);
            //NPCProfileImage.gameObject.SetActive(true);

            //CharaterProfileImage.sprite = ResourceData.LoadCharacterImage(characterTag, CharacterImageType.Small);
            //CharacterNameText.text = ResourceData.LoadCharacterName(characterTag);
            //if (CharaterProfileImage.sprite == null) {
            //    CharaterProfileImage.gameObject.SetActive(false);
            //}

            //NPCProfileImage.sprite = ResourceData.LoadCharacterImage(nPCTag, CharacterImageType.Small);
            //NPCNameText.text = ResourceData.LoadNPCName(nPCTag);
            //if (NPCProfileImage.sprite == null) {
            //    NPCProfileImage.gameObject.SetActive(false);
            //}
        }

        #region UI控制
        protected void MoveContentUpWard() {
            
            float currentHeight = 10;
            RectTransform[] rectTransforms = ContentPart.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform rect in rectTransforms)
            {
                if(rect.name == ContentPart.name) continue;
                currentHeight += rect.sizeDelta.y;
            }

            //文本堆积过高时，将ContentPartUI上移
            if (currentHeight > ContentPart.GetComponent<RectTransform>().sizeDelta.y) {
                ContentPart.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    0, currentHeight
                );
                ContentPart.transform.localPosition = new Vector3(
                    ContentPart.transform.localPosition.x, 
                    currentHeight, 
                    ContentPart.transform.localPosition.z
                );
            }

        }

        protected void ResetContentHeight() {

            //重设ContentPartUI高度
            float originalY = 125;
            ContentPart.transform.position = new Vector3(
                ContentPart.transform.position.x, 
                originalY, 
                ContentPart.transform.position.z
            );

            RectTransform contentRectTransform = ContentPart.GetComponent<RectTransform>();
            float originSizeX = contentRectTransform.sizeDelta.x;
            float originSizeY = 200;
            contentRectTransform.sizeDelta = new Vector2(originSizeX, originSizeY);
        }
        #endregion

        #region 生成对话内容
        protected void CreateTextObj(DialogRowData RowData, GameObject DialogTextPrefab, Transform ParentTrans) {
            CreateTextObj(RowData.CharacterTag, RowData.Content, DialogTextPrefab, ParentTrans);
        }

        protected void CreateTextObj(string characterTag, string TextContent, GameObject DialogTextPrefab, Transform ParentTrans) {
            GameObject DialogTextObj = Instantiate(DialogTextPrefab, ParentTrans);
            //配置文本行信息
            int ContentHeight = (TextContent.Length / 32 + 1) * 25;
            DialogTextObj.GetComponent<DialogText>().SetText(characterTag, TextContent, ContentHeight);
            TextEmergeTwn = DialogTextObj.GetComponent<TMP_Text>().DOText(TextContent, TextContent.Length / 10);
        }

        protected void CreateOptionObj(DialogRowData RowData, GameObject DialogOptionPrefab, Transform ParentTrans, Action<string, int>  OptionButtonEvent,ref List<DialogOptionButton> CurrentOptions) {
            
            Dictionary<string, KeyValuePair<int, bool>> optionIdTrigger = DialogTextExplainer.GetInstance().ReadOptionText(
                RowData.Content, RowData.Condition
            );

            foreach (string OptionKey in optionIdTrigger.Keys)
            {
                //配置选项信息
                int OptionHeight = (OptionKey.Length / 50 + 1) * 25;
                DialogOptionButton dialogOption = Instantiate(DialogOptionPrefab, ParentTrans).GetComponent<DialogOptionButton>();
                dialogOption.SetOption(OptionKey, optionIdTrigger[OptionKey].Key, optionIdTrigger[OptionKey].Value , OptionHeight, () => { 
                    OptionButtonEvent(OptionKey, optionIdTrigger[OptionKey].Key);
                });
                CurrentOptions.Add(dialogOption);
            }
        }

        #endregion
    }
}