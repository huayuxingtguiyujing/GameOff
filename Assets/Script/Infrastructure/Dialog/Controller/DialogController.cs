//using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using TextAsset = UnityEngine.TextAsset;
using System;
using GameOff2023.Utils;
using DG.Tweening;
using GameOff2023.Utils;

namespace GameOff2023.Dialog.DialogControl {
    using DialogRowDataList = List<DialogRowData>;
    public class DialogController : DialogControllerUI {
        #region �Ի����� ��Ҫ�ı���
        DialogRowDataList dialogRowDatas;     //�ı���Ϣ

        [Header("�Ի�����")]
        public int CurrentId;
        public int NextId;
        public bool IsChoosingFlag = false;      //�Ƿ�����ѡ��ѡ��

        List<DialogOptionButton> CurrentOptions = new List<DialogOptionButton>();       //��ǰѡ��

        #endregion

        #region ���ƶԻ� ����
        ///<summary>
        /// ��ʼ�Ի�
        ///</summary>
        public void StartDialog(EventPlaceDialogData eventData) {
            //����Ի�����
            dialogRowDatas = eventData.GetTriggerEvent();

            if(dialogRowDatas == null) {
                return;
            }
            else {
                // ���ڶԻ���
                //Global.IsTalking = true;
            }

            gameObject.SetActive(true);

            //���öԻ���λ
            ResetContentHeight();

            //��յ�ǰ�Ի����ݣ�����һ�ζԻ����ݣ�
            ContentPart.gameObject.ClearObjChildren();

            //�����һ�ζԻ�
            UpdateDialogData(dialogRowDatas[0]);
        }

        ///<summary>
        /// ������һ�ζԻ�
        ///</summary>
        public void UpdateNextDialog() {
            if (NextId < 0 || NextId > dialogRowDatas.Count) {
                //Խ��������������˳��Ի�
                CancelDialog();
                return;
            }

            if (IsChoosingFlag) {
                //������ѡ��ѡ��״̬
                return;
            }
            //��ת�ı� nextId��Ҫ������һ ��Ϊ����0Ϊ�¼�������Ϣ
            UpdateDialogData(dialogRowDatas[NextId]);

            //�����ı���
            MoveContentUpWard();
        }

        ///<summary>
        /// �����Ի� --�ṩ��PlayerController
        ///</summary>
        public void CancelDialog() {
            //Global.IsTalking = false;

            //��յ�ǰ�Ի����ݣ�ContentPart�µģ�
            ContentPart.ClearObjChildren();
            //Transform[] ContentDialogs = ContentPart.transform.GetComponentsInChildren<Transform>(true);
            //foreach (Transform dialog in ContentDialogs)
            //{
            //    if(dialog.name.Equals(ContentPart.name)) {
            //        continue;
            //    }
            //    Destroy(dialog.gameObject);
            //}

            //�رնԻ���
            gameObject.SetActive(false);
        }
        
        ///<summary>
        /// ������תId ��������ת
        ///</summary>
        public void UpdateJumpIdDialog(int Id) {
            NextId = Id;
            UpdateNextDialog();
        }
        #endregion

        ///<summary>
        /// ���ضԻ����ݵ��Ի�����
        ///</summary>
        protected void UpdateDialogData(DialogRowData RowData) {
            
            CurrentId = RowData.Id;
            NextId = RowData.Jump;
            UpdateCharacterProfile(RowData.CharacterTag, RowData.NpcTag);
            TextEmergeTwn.Complete();

            if (RowData.Type == RowDataType.NormalType) {
                bool trigger = DialogTextExplainer.GetInstance().CanTextTrigger(RowData.Condition);
                if (trigger) {
                    CreateTextObj(RowData, DialogText, ContentPart.transform);
                }
                else {
                    UpdateNextDialog();
                }
            }
            else if (RowData.Type == RowDataType.ChooseType) {
                //����������ѡ��ѡ����
                IsChoosingFlag = true;
                Action<string, int>  OptionButtonEvent = new Action<string, int>(OptionEvent);
                CreateOptionObj(RowData, DialogOption, ContentPart.transform, OptionButtonEvent, ref CurrentOptions);
            }
            else if (RowData.Type == RowDataType.TriggerBattleType) {
                CancelDialog();
                DialogTextExplainer.GetInstance().ReadBattleContent(RowData.Content);
            }
            else if (RowData.Type == RowDataType.ArtType) {
                DialogTextExplainer.GetInstance().ReadArtText(RowData.Content);
                UpdateNextDialog();
            }
            else if(RowData.Type == RowDataType.SetEventPlaceType){
                //Ϊ�¼��趨��Ϣ,ֱ������
                UpdateJumpIdDialog(1);
            }

            //����Ч�� �����Ч���Ļ�
            if (RowData.ExistEffect()) {
                DialogTextExplainer.GetInstance().ReadEffectText(RowData.Effect);
            }
        }

        #region ѡ���
        private void OptionEvent(string content, int JumpId) {
            ClearCurrentOption();

            CreateTextObj("PlaceHolderTag", content, DialogText, ContentPart.transform);

            IsChoosingFlag = false;

            UpdateJumpIdDialog(JumpId);
            //NextId = JumpId;
            //UpdateNextDialog();
        }

        private void ClearCurrentOption() {
            //��յ�ǰѡ��
            if(CurrentOptions.Count > 0) {
                foreach (DialogOptionButton option in CurrentOptions)
                {
                    option.SetDisable();
                }
            }
            CurrentOptions.Clear();
        }
        #endregion
    }

    ///<summary>
    /// CSV�ı��ļ�ÿ�е�������Ϣ
    ///</summary>
    public class DialogRowData {
        public RowDataType Type;
        public int Id;
        public string CharacterTag;
        public string NpcTag;
        public string Content;
        public int Jump;
        public string Effect;
        public string Reservation;
        public string Condition;

        public DialogRowData(string type, int id, 
            string characterName, string npcName, string content, int jump, 
            string effect, string option, string condition) {

            if (type.Equals("#")) {
                //һ��Ի�
                Type = RowDataType.NormalType;
            }
            else if (type.Equals("&")) {
                //��ѡ��ĶԻ�
                Type = RowDataType.ChooseType;
            }
            else if (type.Equals("*")) {
                //��ʶ��������һ�ζԻ�����һ���¼��ڵ�
                Type = RowDataType.SetEventPlaceType;
            }
            else if (type.Equals("%")) {
                //���ǶԻ�������UI��BGMЧ��
                Type = RowDataType.ArtType;
            }
            else if (type.Equals("$")) {
                //���ǶԻ�������ս�� �����̽����Ի�
                Type = RowDataType.TriggerBattleType;
            }
            else {
                Type = RowDataType.None;
            }

            Id = id;
            CharacterTag = characterName;
            NpcTag = npcName;
            Content = content;
            Jump = jump;
            Effect = effect;
            Reservation = option;
            Condition = condition;
        }
    
        public static DialogRowData GetDialogRowData(string rowText) {
            
            //�Զ��ŷָ�
            string[] cell = rowText.Split(',');

            

            return new DialogRowData(
                cell[0], int.Parse(cell[1]), cell[2],cell[3], 
                cell[4], int.Parse(cell[5]), cell[6], cell[7], cell[8]
            );
        }

        public static string[] GetBattleContent(string Text) {
            return Text.Split('/');
        }
    
        public bool ExistCondition_NormalType() {
            if(Type != RowDataType.NormalType) {
                return false;
            }
            return Condition.Length != 0 && Condition.Length != 1;
        }
    
        public bool ExistEffect() {
            return Effect.Length != 1 && Effect != "None" && Effect != "" && Effect != "None\r";
        }
        

    }

    public enum RowDataType {
        NormalType,
        ChooseType,
        ArtType,
        SetEventPlaceType,
        TriggerBattleType,
        None
    }
}

