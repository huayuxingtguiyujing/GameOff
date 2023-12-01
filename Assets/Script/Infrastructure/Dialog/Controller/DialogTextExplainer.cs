
using System.Collections.Generic;
using UnityEngine;
using GameOff2023.Dialog.DialogModel;

namespace GameOff2023.Dialog.DialogControl
{
    using DialogRowDataList = List<DialogRowData>;
    using AndConditions = List<DialogCondition>;
    public class DialogTextExplainer {
        #region ����ģʽ
        public static DialogTextExplainer instance;
        public static DialogTextExplainer GetInstance() {
            if (instance == null) {
                instance = new DialogTextExplainer();
            }
            return instance;
        }
        public DialogTextExplainer() { }
        #endregion

        #region ��ȡ�ı�Asset
        public DialogRowDataList ReadTextAsset(TextAsset textAsset) {
            //�Ի��з��ָ�
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
        /// ��CSV�ı� ת��Ϊ ��֯��������
        ///</summary>
        public List<EventPlaceDialogData> SortEventPlaceDialogData(List<TextAsset> textAssets) {
            //�洢�¼��ڵ�Id�¼��б��ӳ��
            Dictionary<int, List<DialogRowDataList>> PlaceEventDic = new Dictionary<int, List<DialogRowDataList>>();
            foreach (TextAsset textAsset in textAssets)
            {
                SortDialogRowData(textAsset, ref PlaceEventDic);
            }

            //���ֵ���ϢתΪ EventPlaceDialogData �б�
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
            
            //��ȡ�����������ı�����
            DialogRowDataList dialogRowDatas = ReadTextAsset(textAsset);

            int count = dialogRowDatas.Count;
            for (int i = 0; i < count; i++) {
                //�ж��Ƿ����¼��ڵ������Ϣ
                if (dialogRowDatas[i].Type == RowDataType.SetEventPlaceType) {
                    int j = i + 1;
                    while (j < dialogRowDatas.Count && dialogRowDatas[j].Type != RowDataType.SetEventPlaceType) {
                        j ++;
                    }

                    //��ӵ��ֵ�ӳ����
                    int EventPlaceId = dialogRowDatas[i].Id;
                    if (PlaceEventDic.ContainsKey(EventPlaceId)) {
                        //�Ѱ������ֵ���
                        PlaceEventDic[EventPlaceId].Add(
                            dialogRowDatas.GetRange(i, j - i)
                        );
                    }
                    else {
                        //��ȡ����������֮�����Ϣ
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

        #region �ı���Ϣ����
        ///<summary>
        /// ����ѡ����ı� ����ѡ����������תId��ӳ���ֵ�
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
        /// �������������ı� ���ܴ���ʱ����false
        ///</summary>
        private bool ReadConditionText(string eventPlaceId, string eventPlaceName, string Text) {
            //���������� ֱ�ӷ���true
            if(Text == null || Text.Length == 0 || Text.Length == 1 || Text == "None" || Text == "None\r") return true;
            DialogConditionList dialogConditionList = new DialogConditionList(Text);
            return ReadConditionText(eventPlaceId, eventPlaceName, dialogConditionList);
        }
        
        private bool ReadConditionText(string eventPlaceId, string eventPlaceName, DialogConditionList dialogCondition) {
            foreach (AndConditions andConditions in dialogCondition.OrConditions)
            {
                //AndConditionsֻҪ��һ��andConditionsȫ������ ���ɷ���true
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
        /// ����Ӱ��Ч�������ı�
        ///</summary>
        public void ReadEffectText(string Text) {
            DialogEffectList dialogEffectList = new DialogEffectList(Text);
            foreach (DialogEffect effect in dialogEffectList.dialogEffects)
            {
                // ����Ч��
                //Global.playerRecorder.TakeEffect(effect);
            }
        }
        
        ///<summary>
        /// ����ս�������ı�
        ///</summary>
        public void ReadBattleContent(string Text){
            //List<EnemyStatu> enemyList = new List<EnemyStatu>();
            //string[] rec = DialogRowData.GetBattleContent(Text);
            //foreach (string enemyName in rec)
            //{
            //    enemyList.Add(ResourceData.LoadEnemy(enemyName));
            //}

            // ����ս��
            //Global.playerRecorder.StartACombat(enemyList);
        }
        
        ///<summary>
        /// ��������Ч�������ı�
        ///</summary>
        public void ReadArtText(string Text) {
            DialogArtList dialogArt = new DialogArtList(Text);
            foreach (DialogArt art in dialogArt.artList)
            {
                // ��������Ч��
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
    /// ��װ�¼��ڵ���Ϣ���Ի��¼���ӳ��
    ///</summary>
    public class EventPlaceDialogData {
        
        public int EventPlaceId;        //�¼��ڵ��Id ����ڵ�ID����ͬһ��������Ψһ�ģ���Ҫ��Unity�����ݶ�Ӧ

        public string EventPlaceName;

        public int EventValue;

        public string EventPlaceArea;       //�¼��������� ֵΪ�¼����ڵ��ı��ļ���

        public List<DialogRowDataList> PlaceEvents;         //��ǰId�µ����пɴ����¼�

        public EventPlaceDialogData(int eventPlaceId, int eventValue,string eventPlaceArea, string eventPlaceName, List<DialogRowDataList> placeEvents) {
            EventPlaceId = eventPlaceId;
            EventValue = eventValue;
            EventPlaceArea = eventPlaceArea;
            EventPlaceName = eventPlaceName;
            PlaceEvents = placeEvents;
        }

        ///<summary>
        /// ��ȡ��ǰ������ȼ����¼�
        ///</summary>
        private DialogRowDataList GetHighestValueDialog() {
            int maxValue = 0;
            DialogRowDataList ans = null;
            foreach (DialogRowDataList DialogRowData in PlaceEvents)
            {
                //�ж����ȼ�
                if(DialogRowData[0].Jump > maxValue) {
                    //�ж����������Ƿ�����
                    maxValue = DialogRowData[0].Jump;
                    ans = DialogRowData;
                }
            }
            return ans;
        }

        ///<summary>
        /// ��ȡ������Ľڵ��¼�
        ///</summary>
        private List<DialogRowDataList> GetSortedDialog() {
            List<DialogRowDataList> ans = new List<DialogRowDataList>(PlaceEvents);
            ans.Sort((dia1, dia2) => {
                return dia2[0].Jump.CompareTo(dia1[0].Jump);
            });
            return ans;
        }

        ///<summary>
        /// ��ȡӦ�������Ľڵ��¼�
        ///</summary>
        public DialogRowDataList GetTriggerEvent() {
            DialogRowDataList rowDataList = GetHighestValueDialog();
            //������Ȩ�ؽ�������
            List<DialogRowDataList> sortedEvents = GetSortedDialog();
            bool canEventTrigger = false;
            foreach (DialogRowDataList dialog in sortedEvents)
            {
                //����һ�ζԻ��������� ����Դ���
                canEventTrigger = canEventTrigger || DialogTextExplainer.GetInstance().CanEventTrigger(
                    EventPlaceId.ToString(), EventPlaceName, dialog
                );
                //Debug.Log("���ζԻ��Ƿ�����������" + canEventTrigger);
                if (canEventTrigger) {
                    //�ҵ���ǰ�ɴ��������ȼ���ߵ��¼� ����ѭ��
                    rowDataList = dialog;
                    break;
                }
            }

            //û�з��������ĶԻ�
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