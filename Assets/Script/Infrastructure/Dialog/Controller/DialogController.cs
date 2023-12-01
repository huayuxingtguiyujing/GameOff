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
        #region 对话控制 需要的变量
        DialogRowDataList dialogRowDatas;     //文本信息

        [Header("对话控制")]
        public int CurrentId;
        public int NextId;
        public bool IsChoosingFlag = false;      //是否正在选择选项

        List<DialogOptionButton> CurrentOptions = new List<DialogOptionButton>();       //当前选项

        #endregion

        #region 控制对话 方法
        ///<summary>
        /// 开始对话
        ///</summary>
        public void StartDialog(EventPlaceDialogData eventData) {
            //载入对话内容
            dialogRowDatas = eventData.GetTriggerEvent();

            if(dialogRowDatas == null) {
                return;
            }
            else {
                // 正在对话中
                //Global.IsTalking = true;
            }

            gameObject.SetActive(true);

            //重置对话框方位
            ResetContentHeight();

            //清空当前对话内容（即上一次对话内容）
            ContentPart.gameObject.ClearObjChildren();

            //载入第一段对话
            UpdateDialogData(dialogRowDatas[0]);
        }

        ///<summary>
        /// 加载下一段对话
        ///</summary>
        public void UpdateNextDialog() {
            if (NextId < 0 || NextId > dialogRowDatas.Count) {
                //越界或正常结束，退出对话
                CancelDialog();
                return;
            }

            if (IsChoosingFlag) {
                //正处于选择选项状态
                return;
            }
            //跳转文本 nextId不要修正减一 因为数组0为事件设置信息
            UpdateDialogData(dialogRowDatas[NextId]);

            //下移文本框
            MoveContentUpWard();
        }

        ///<summary>
        /// 结束对话 --提供给PlayerController
        ///</summary>
        public void CancelDialog() {
            //Global.IsTalking = false;

            //清空当前对话内容（ContentPart下的）
            ContentPart.ClearObjChildren();
            //Transform[] ContentDialogs = ContentPart.transform.GetComponentsInChildren<Transform>(true);
            //foreach (Transform dialog in ContentDialogs)
            //{
            //    if(dialog.name.Equals(ContentPart.name)) {
            //        continue;
            //    }
            //    Destroy(dialog.gameObject);
            //}

            //关闭对话框
            gameObject.SetActive(false);
        }
        
        ///<summary>
        /// 设置跳转Id 并立刻跳转
        ///</summary>
        public void UpdateJumpIdDialog(int Id) {
            NextId = Id;
            UpdateNextDialog();
        }
        #endregion

        ///<summary>
        /// 加载对话数据到对话框中
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
                //设置正处于选择选项中
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
                //为事件设定信息,直接跳过
                UpdateJumpIdDialog(1);
            }

            //更新效果 如果有效果的话
            if (RowData.ExistEffect()) {
                DialogTextExplainer.GetInstance().ReadEffectText(RowData.Effect);
            }
        }

        #region 选项处理
        private void OptionEvent(string content, int JumpId) {
            ClearCurrentOption();

            CreateTextObj("PlaceHolderTag", content, DialogText, ContentPart.transform);

            IsChoosingFlag = false;

            UpdateJumpIdDialog(JumpId);
            //NextId = JumpId;
            //UpdateNextDialog();
        }

        private void ClearCurrentOption() {
            //清空当前选项
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
    /// CSV文本文件每行的数据信息
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
                //一般对话
                Type = RowDataType.NormalType;
            }
            else if (type.Equals("&")) {
                //有选项的对话
                Type = RowDataType.ChooseType;
            }
            else if (type.Equals("*")) {
                //标识接下来的一段对话属于一个事件节点
                Type = RowDataType.SetEventPlaceType;
            }
            else if (type.Equals("%")) {
                //并非对话，而是UI、BGM效果
                Type = RowDataType.ArtType;
            }
            else if (type.Equals("$")) {
                //并非对话，触发战斗 会立刻结束对话
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
            
            //以逗号分割
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

