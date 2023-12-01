
using GameOff2023.Utils;
using GameOff2023.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.Dialog.DialogModel
{
    public class DialogOption: MonoBehaviour {
        public Dictionary<string, KeyValuePair<int, string>> OptionIdTriggerDic;

        public DialogOption(string optionText, string conditionText) {
            OptionIdTriggerDic = new Dictionary<string, KeyValuePair<int, string>>();

            string[] OptionList = optionText.Split('/');

            string[] OptionTrigger;
            if (!conditionText.IsValidDialogItem()) {
                OptionTrigger = new string[OptionList.Length];
            }
            else {
                OptionTrigger = conditionText.Split('/');
            }

            if(OptionTrigger.Length == 0) {
                //OptionTrigger = new ;
            }
            
            if (OptionTrigger.Length != OptionList.Length) {
                Debug.LogError("option字段格式出错！");
                return;
            }

            for (int i = 0; i < OptionList.Length; i++) {
                string[] optionItem = OptionList[i].Split('!');
                if (optionItem.Length != 2) {
                    Debug.LogError("option字段格式出错！");
                    return;
                }

                string OptionText = optionItem[0];
                int JumpId = int.Parse(optionItem[1]);
                string optionCondition = OptionTrigger[i];

                if (!OptionIdTriggerDic.ContainsKey(OptionText)) {
                    KeyValuePair<int, string> keyValuePair = new KeyValuePair<int, string>(JumpId, optionCondition);
                    OptionIdTriggerDic.Add(OptionText, keyValuePair);
                }
                else {
                    KeyValuePair<int, string> keyValuePair = new KeyValuePair<int, string>(JumpId, optionCondition);
                    OptionIdTriggerDic[OptionText] = keyValuePair;
                }
            }
        }
    }
}