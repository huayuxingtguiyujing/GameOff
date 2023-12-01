using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameOff2023.Dialog.DialogModel
{
    public class DialogArtList {
        public List<DialogArt> artList;

        public DialogArtList(string Text) {
            artList = new List<DialogArt>();
            string[] artStrs = Text.Split('/');
            foreach (string artStr in artStrs)
            {
                DialogArt art = new DialogArt(artStr);
                artList.Add(art);
            }
        }

        //public IEnumerator GetEnumerator() {
        //    return artList.ToArray();
        //}
    }

    public class DialogArt{
        public ArtType artType;
        public string artVariableName;

        public bool hasParameter;
        //artVariableNameΪ����ʱ��ֵ��������
        //artVariableNameΪ����Ч��ʱ��ֵ�ǳ̶�
        //artVariableNameΪ��ͼЧ��ʱ��ֵ��չ�ֵ�ͼ����/���ص�ͼ����
        public int artLevel = 0;        

        public DialogArt(string Text) {
            string[] rec = Text.Split('@');
            if(rec.Length != 2) {
                Debug.LogError("��ʽ����ȷ");
            }

            switch (rec[0]) {
                case "BGM":
                    artType = ArtType.BGM;
                    hasParameter = true;
                    break;
                case "MainTheme":
                    artType = ArtType.MainTheme;
                    hasParameter = true;
                    break;
                case "Gallery":
                    artType = ArtType.Gallery;
                    hasParameter = false;
                    break;
                case "Environment":
                    artType = ArtType.Environment;
                    hasParameter = true;
                    break;
                case "Map":
                    artType = ArtType.Map;
                    hasParameter = true;
                    break;
                case "EventPlace":
                    artType = ArtType.EventPlace;
                    hasParameter = true;
                    break;
                case "NoticeUI":
                    artType = ArtType.NoticeUI;
                    hasParameter = false;
                    break;
                default:
                    artType = ArtType.None;
                    return;
            }

            if (hasParameter) {
                //���ڿ��Ʋ���
                string[] rec2 = rec[1].Split(':');
                if(rec2.Length != 2) {
                    Debug.LogError("��ʽ����ȷ");
                }
                artVariableName = rec2[0];
                artLevel = int.Parse(rec2[1]);
            }
            else {
                artVariableName = rec[1];
            }

        }

    }

    public enum ArtType {
        BGM,
        MainTheme,
        Gallery,
        Environment,
        Map,
        NoticeUI,
        EventPlace,
        None
    }
}