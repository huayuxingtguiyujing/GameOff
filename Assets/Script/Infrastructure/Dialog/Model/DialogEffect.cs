using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.Dialog.DialogModel
{
    public class DialogEffectList {
        public List<DialogEffect> dialogEffects;

        public DialogEffectList(string Text) {
            dialogEffects = new List<DialogEffect>();
            //�ָ����Ч��
            string[] recs = Text.Split('/');
            foreach (string effectText in recs)
            {
                dialogEffects.Add(new DialogEffect(effectText));
            }
        }
    }

    public class DialogEffect {
        public EffectType effectType;
        public string variableName;
        public int variableModify;
        public bool IsCharacterOp = false;

        public DialogEffect(string Text) {
            string[] rec = Text.Split('@');
            if (rec.Length != 2) {
                Debug.LogError("��ʽ����");
            }

            switch (rec[0]) {
                case "Statu":
                    effectType = EffectType.Statu;
                    break;
                case "EventBool":
                    effectType = EffectType.EventBool;
                    break;
                case "EventInt":
                    effectType = EffectType.EventInt;
                    break;
                case "Item":
                    effectType = EffectType.Item;
                    break;
                case "AddCharacter":
                    IsCharacterOp = true;
                    effectType = EffectType.AddCharacter;
                    break;
                case "LoseCharacter":
                    IsCharacterOp = true;
                    effectType = EffectType.LoseCharacter;
                    break;
                default:
                    effectType = EffectType.Error;
                    break;
            }

            if (IsCharacterOp) {
                //�ǽ�ɫ�����
                variableModify = 0;
                variableName = rec[1];
            }
            else {
                string[] recVar = rec[1].Split(':');
                if(recVar.Length != 2) {
                    Debug.LogError("��ʽ����");
                }

                variableModify = int.Parse(recVar[1]);
                variableName = recVar[0];
            }
        }
    }

    public enum EffectType {
        Statu,
        EventBool,
        EventInt,
        Item,
        AddCharacter,
        LoseCharacter,
        None,
        Error
    }
}