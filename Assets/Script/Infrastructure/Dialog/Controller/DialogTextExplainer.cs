
using System.Collections.Generic;
using UnityEngine;
using GameOff2023.Dialog.DialogModel;

namespace GameOff2023.Dialog.DialogControl
{
    using DialogRowDataList = List<DialogRowData>;
    using AndConditions = List<DialogCondition>;
    public class DialogTextExplainer {
        #region 单例模式
        public static DialogTextExplainer instance;
        public static DialogTextExplainer GetInstance() {
            if (instance == null) {
                instance = new DialogTextExplainer();
            }
            return instance;
        }
        public DialogTextExplainer() { }
        #endregion

        #region 读取文本Asset
        public DialogRowDataList ReadTextAsset(TextAsset textAsset) {
            //以换行符分割
            string[] Rows = textAsset.text.Split('\n');
            DialogRowDataList RowDataList = new DialogRowDataList();

            if (Rows.Length < 2) {
                return RowDataList;
            }

            for(int i = 1; i < Rows.Length; i++) {
                RowDataList.Add(DialogRowData.GetDialogRowData(Rows[i]));
            }

            return RowDataList;
        }

        ///<summary>
        /// 将CSV文本 转化为 组织过的数据
        ///</summary>
        public List<EventPlaceDialogData> SortEventPlaceDialogData(List<TextAsset> textAssets) {
            //存储事件节点Id事件列表的映射
            Dictionary<int, List<DialogRowDataList>> PlaceEventDic = new Dictionary<int, List<DialogRowDataList>>();
            foreach (TextAsset textAsset in textAssets)
            {
                SortDialogRowData(textAsset, ref PlaceEventDic);
            }

            //将字典信息转为 EventPlaceDialogData 列表
            List<EventPlaceDialogData> eventPlaceDialogDatas = new List<EventPlaceDialogData>();
            foreach (KeyValuePair<int, List<DialogRowDataList>> Pair in PlaceEventDic)
            {
                eventPlaceDialogDatas.Add(new EventPlaceDialogData(
                    Pair.Key, 0, textAssets[0].name, Pair.Key.ToString(), Pair.Value
                ));
            }

            return eventPlaceDialogDatas;
        }

        private void SortDialogRowData(TextAsset textAsset, ref Dictionary<int, List<DialogRowDataList>> PlaceEventDic) {
            
            //获取初步解析的文本数据
            DialogRowDataList dialogRowDatas = ReadTextAsset(textAsset);

            int count = dialogRowDatas.Count;
            for (int i = 0; i < count; i++) {
                //判断是否是事件节点分类信息
                if (dialogRowDatas[i].Type == RowDataType.SetEventPlaceType) {
                    int j = i + 1;
                    while (j < dialogRowDatas.Count && dialogRowDatas[j].Type != RowDataType.SetEventPlaceType) {
                        j ++;
                    }

                    //添加到字典映射中
                    int EventPlaceId = dialogRowDatas[i].Id;
                    if (PlaceEventDic.ContainsKey(EventPlaceId)) {
                        //已包含在字典内
                        PlaceEventDic[EventPlaceId].Add(
                            dialogRowDatas.GetRange(i, j - i)
                        );
                    }
                    else {
                        //获取两个分类标记之间的信息
                        List<DialogRowDataList> RowDataRec = new List<DialogRowDataList> {
                            dialogRowDatas.GetRange(i, j - i)
                        };
                        PlaceEventDic.Add(EventPlaceId, RowDataRec);
                    }
                    i = j - 1;
                }
            }

        }
        #endregion

        #region 文本信息处理
        ///<summary>
        /// 处理选项部分文本 返回选项文字与跳转Id的映射字典
        ///</summary>
        public Dictionary<string, KeyValuePair<int, bool>> ReadOptionText(string optionText, string conditionText) {
            DialogOption dialogOption = new DialogOption(optionText, conditionText);
            Dictionary<string, KeyValuePair<int, bool>> ans = new Dictionary<string, KeyValuePair<int, bool>>();
            foreach (KeyValuePair<string, KeyValuePair<int, string>> optionIdTrigger in dialogOption.OptionIdTriggerDic)
            {
                bool trigger = CanOptionChoose(optionIdTrigger.Value.Value);
                KeyValuePair<int, bool> rec = new KeyValuePair<int, bool>(
                    optionIdTrigger.Value.Key, trigger
                );
                ans.Add(optionIdTrigger.Key, rec);
            }

            return ans;
        }

        ///<summary>
        /// 处理条件部分文本 不能触发时返回false
        ///</summary>
        private bool ReadConditionText(string eventPlaceId, string eventPlaceName, string Text) {
            //无条件限制 直接返回true
            if(Text == null || Text.Length == 0 || Text.Length == 1 || Text == "None" || Text == "None\r") return true;
            DialogConditionList dialogConditionList = new DialogConditionList(Text);
            return ReadConditionText(eventPlaceId, eventPlaceName, dialogConditionList);
        }
        
        private bool ReadConditionText(string eventPlaceId, string eventPlaceName, DialogConditionList dialogCondition) {
            foreach (AndConditions andConditions in dialogCondition.OrConditions)
            {
                //AndConditions只要有一个andConditions全部符合 即可返回true
                bool andAns = true;
                foreach (DialogCondition condition in andConditions)
                {
                    andAns = andAns;
                        //&& Global.playerRecorder.GetArchiveCondition(eventPlaceId, eventPlaceName, condition);
                }

                if(andAns) return true;
            }
            return false;
        }

        ///<summary>
        /// 处理影响效果部分文本
        ///</summary>
        public void ReadEffectText(string Text) {
            DialogEffectList dialogEffectList = new DialogEffectList(Text);
            foreach (DialogEffect effect in dialogEffectList.dialogEffects)
            {
                // 触发效果
                //Global.playerRecorder.TakeEffect(effect);
            }
        }
        
        ///<summary>
        /// 处理战斗部分文本
        ///</summary>
        public void ReadBattleContent(string Text){
            //List<EnemyStatu> enemyList = new List<EnemyStatu>();
            //string[] rec = DialogRowData.GetBattleContent(Text);
            //foreach (string enemyName in rec)
            //{
            //    enemyList.Add(ResourceData.LoadEnemy(enemyName));
            //}

            // 触发战斗
            //Global.playerRecorder.StartACombat(enemyList);
        }
        
        ///<summary>
        /// 处理美术效果控制文本
        ///</summary>
        public void ReadArtText(string Text) {
            DialogArtList dialogArt = new DialogArtList(Text);
            foreach (DialogArt art in dialogArt.artList)
            {
                // 触发美术效果
                //Global.playerRecorder.TakeArt(art);
            }
        }
        #endregion

        private bool CanOptionChoose(string Text) {
            return ReadConditionText("null", "null", Text);
        }

        public bool CanEventTrigger(string eventPlaceId, string eventPlaceName, DialogRowDataList dialog) {
            return ReadConditionText(eventPlaceId, eventPlaceName, dialog[0].Condition);
        }

        public bool CanTextTrigger(string conditionText) {
            return ReadConditionText("null", "null", conditionText);
        }

        
    }

    ///<summary>
    /// 封装事件节点信息到对话事件的映射
    ///</summary>
    public class EventPlaceDialogData {
        
        public int EventPlaceId;        //事件节点的Id 这个节点ID在在同一场景中是唯一的，需要与Unity中内容对应

        public string EventPlaceName;

        public int EventValue;

        public string EventPlaceArea;       //事件所属区域 值为事件所在的文本文件名

        public List<DialogRowDataList> PlaceEvents;         //当前Id下的所有可触发事件

        public EventPlaceDialogData(int eventPlaceId, int eventValue,string eventPlaceArea, string eventPlaceName, List<DialogRowDataList> placeEvents) {
            EventPlaceId = eventPlaceId;
            EventValue = eventValue;
            EventPlaceArea = eventPlaceArea;
            EventPlaceName = eventPlaceName;
            PlaceEvents = placeEvents;
        }

        ///<summary>
        /// 获取当前最高优先级的事件
        ///</summary>
        private DialogRowDataList GetHighestValueDialog() {
            int maxValue = 0;
            DialogRowDataList ans = null;
            foreach (DialogRowDataList DialogRowData in PlaceEvents)
            {
                //判定优先级
                if(DialogRowData[0].Jump > maxValue) {
                    //判定发生条件是否满足
                    maxValue = DialogRowData[0].Jump;
                    ans = DialogRowData;
                }
            }
            return ans;
        }

        ///<summary>
        /// 获取排序过的节点事件
        ///</summary>
        private List<DialogRowDataList> GetSortedDialog() {
            List<DialogRowDataList> ans = new List<DialogRowDataList>(PlaceEvents);
            ans.Sort((dia1, dia2) => {
                return dia2[0].Jump.CompareTo(dia1[0].Jump);
            });
            return ans;
        }

        ///<summary>
        /// 获取应当触发的节点事件
        ///</summary>
        public DialogRowDataList GetTriggerEvent() {
            DialogRowDataList rowDataList = GetHighestValueDialog();
            //按触发权重降序排序
            List<DialogRowDataList> sortedEvents = GetSortedDialog();
            bool canEventTrigger = false;
            foreach (DialogRowDataList dialog in sortedEvents)
            {
                //若有一段对话满足条件 则可以触发
                canEventTrigger = canEventTrigger || DialogTextExplainer.GetInstance().CanEventTrigger(
                    EventPlaceId.ToString(), EventPlaceName, dialog
                );
                //Debug.Log("本段对话是否满足条件：" + canEventTrigger);
                if (canEventTrigger) {
                    //找到当前可触发的优先级最高的事件 跳出循环
                    rowDataList = dialog;
                    break;
                }
            }

            //没有符合条件的对话
            if (!canEventTrigger) {
                return null;
            }

            return rowDataList;
        }

        public void DebugMessage() {
            foreach (DialogRowDataList list in PlaceEvents)
            {
                foreach (DialogRowData rowData in list) {
                    Debug.Log(rowData.Id + " " + rowData.CharacterTag + " " + rowData.NpcTag + " " 
                                + rowData.Content + " " + rowData.Jump + " ");
                }
            }
        }
    }
}