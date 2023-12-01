using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace GameOff2023.Dialog.DialogModel {
    ///<summary>
    /// 封装对话信息中的condition字段
    ///</summary>
    using AndConditions = List<DialogCondition>;
    public class DialogConditionList {
        public List<AndConditions> OrConditions;             //包含了所有或条件 列表中只要有一个条件满足 则成立

        public DialogConditionList(string conditionStr) {
            conditionStr = conditionStr.Replace("\r", "");
            OrConditions = new List<AndConditions>();
            string[] orConditions = conditionStr.Split("|");
            List<string[]> andConditions = new List<string[]>();
            //分类或条件
            foreach (string or in orConditions)
            {
                AndConditions andRec = new AndConditions();
                string[] and = or.Split("&");
                //分类与条件
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

        #region 比较型条件
        public bool isCompare;
        public bool isGreater;
        public int num;
        #endregion

        #region 判断型条件
        public bool isJudge;
        public bool isTrue;
        #endregion

        public DialogCondition() { }

        public DialogCondition(string text) {
            string[] rec = text.Split("@");
            if(rec.Length != 2) {
                //格式不正确
                return;
            }

            if (rec[1].Contains(">")) {
                //比较型条件 > 
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
                //比较型条件 <
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
                //判断型条件 非
                isCompare = false;
                isJudge = true;
                isTrue = false;
                conditionType = TransformConditionBase(rec[0]);
                variableName = rec[1].Replace("!", "");
            }
            else {
                //判断型条件 是
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
                    //格式不正确
                    return ConditionType.Error;
            }
        }

        public bool GetCompareResult(int targetVar) {
            if (!isCompare) {
                //不是比较型条件
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
                //不是判断型条件
                return false;
            }

            //同或逻辑
            if (isTrue) {
                return targetVar;
            }
            else {
                return !targetVar;
            }
        }
    }

        ///<summary>
        /// 条件类型枚举
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