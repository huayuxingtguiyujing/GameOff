using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameOff2023.Dialog.DialogControl
{
    public class DialogControllerUI : MonoBehaviour {
        #region UI���
        [Header("�Ի���UI���")]
        public Image CharaterProfileImage;
        public TMP_Text CharacterNameText;
        public Image NPCProfileImage;
        public TMP_Text NPCNameText;

        public Button ContinueButton;

        public GameObject ContentPart;
        #endregion

        #region Prefab
        [Header("Ԥ�Ƽ�")]
        public GameObject DialogText;
        public GameObject DialogOption;
        #endregion

        #region �����������
        [Header("����Ч��")]
        protected int CurrentTalkLength;
        protected int AddLength = 140;

        protected Tweener TextEmergeTwn;          //���ָ��ֶ����ؼ�
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

        #region UI����
        protected void MoveContentUpWard() {
            
            float currentHeight = 10;
            RectTransform[] rectTransforms = ContentPart.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform rect in rectTransforms)
            {
                if(rect.name == ContentPart.name) continue;
                currentHeight += rect.sizeDelta.y;
            }

            //�ı��ѻ�����ʱ����ContentPartUI����
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

            //����ContentPartUI�߶�
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

        #region ���ɶԻ�����
        protected void CreateTextObj(DialogRowData RowData, GameObject DialogTextPrefab, Transform ParentTrans) {
            CreateTextObj(RowData.CharacterTag, RowData.Content, DialogTextPrefab, ParentTrans);
        }

        protected void CreateTextObj(string characterTag, string TextContent, GameObject DialogTextPrefab, Transform ParentTrans) {
            GameObject DialogTextObj = Instantiate(DialogTextPrefab, ParentTrans);
            //�����ı�����Ϣ
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
                //����ѡ����Ϣ
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