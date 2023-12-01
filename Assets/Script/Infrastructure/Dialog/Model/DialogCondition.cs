using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace GameOff2023.Dialog.DialogModel {
    ///<summary>
    /// ��װ�Ի���Ϣ�е�condition�ֶ�
    ///</summary>
    using AndConditions = List<DialogCondition>;
    public class DialogConditionList {
        public List<AndConditions> OrConditions;             //���������л����� �б���ֻҪ��һ���������� �����

        public DialogConditionList(string conditionStr) {
            conditionStr = conditionStr.Replace("\r", "");
            OrConditions = new List<AndConditions>();
            string[] orConditions = conditionStr.Split("|");
            List<string[]> andConditions = new List<string[]>();
            //���������
            foreach (string or in orConditions)
            {
                AndConditions andRec = new AndConditions();
                string[] and = or.Split("&");
                //����������
                foreach (string condition in and)
                {
                    andRec.Add(new DialogCondition(condition));
                }
                OrConditions.Add(andRec);
            }
        }

    }

    public class DialogCondition {
        public ConditionType conditionType;
        public string variableName;

        #region �Ƚ�������
        public bool isCompare;
        public bool isGreater;
        public int num;
        #endregion

        #region �ж�������
        public bool isJudge;
        public bool isTrue;
        #endregion

        public DialogCondition() { }

        public DialogCondition(string text) {
            string[] rec = text.Split("@");
            if(rec.Length != 2) {
                //��ʽ����ȷ
                return;
            }

            if (rec[1].Contains(">")) {
                //�Ƚ������� > 
                if (rec[1].Split(">").Length != 2) {
                    return;
                }
                isCompare = true;
                isGreater = true;
                isJudge = false;
                conditionType = TransformConditionBase(rec[0]);
                variableName = rec[1].Split(">")[0];
                num = int.Parse(rec[1].Split(">")[1]);
            }
            else if(rec[1].Contains("<")){
                //�Ƚ������� <
                if (rec[1].Split("<").Length != 2) {
                    return;
                }
                isCompare = true;
                isGreater = false;
                isJudge = false;
                conditionType = TransformConditionBase(rec[0]) ;
                variableName = rec[1].Split("<")[0];
                num = int.Parse(rec[1].Split("<")[1]);
            }
            else if (rec[1].Contains("!")) {
                //�ж������� ��
                isCompare = false;
                isJudge = true;
                isTrue = false;
                conditionType = TransformConditionBase(rec[0]);
                variableName = rec[1].Replace("!", "");
            }
            else {
                //�ж������� ��
                isCompare = false;
                isJudge = true;
                isTrue = true;
                conditionType = TransformConditionBase(rec[0]);
                variableName = rec[1];
            }
        }
        
        private ConditionType TransformConditionBase(string text) {
            switch (text) {
                case "Local":
                    return ConditionType.Local;
                case"EventBool":
                    return ConditionType.EventBool;
                case"EventInt":
                    return ConditionType.EventInt;
                case"ItemInt":
                    return ConditionType.ItemInt;
                case"Statu":
                    return ConditionType.Statu;
                case"Team":
                    return ConditionType.Team;
                default:
                    //��ʽ����ȷ
                    return ConditionType.Error;
            }
        }

        public bool GetCompareResult(int targetVar) {
            if (!isCompare) {
                //���ǱȽ�������
                return false;
            }

            if (isGreater) {
                return targetVar >= num;
            }
            else {
                return targetVar < num;
            }
        }

        public bool GetJudgeResult(bool targetVar) {
            if (!isJudge) {
                //�����ж�������
                return false;
            }

            //ͬ���߼�
            if (isTrue) {
                return targetVar;
            }
            else {
                return !targetVar;
            }
        }
    }

        ///<summary>
        /// ��������ö��
        ///</summary>
        public enum ConditionType {
            Local,
            EventBool,
            EventInt,
            ItemInt,
            Statu,
            Team,
            Error
        }
}