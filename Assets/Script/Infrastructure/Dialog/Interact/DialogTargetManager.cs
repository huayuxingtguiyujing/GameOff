using GameOff2023.Dialog.DialogControl;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameOff2023.Dialog {
    /// <summary>
    /// �������е�DialogTarget����
    /// </summary>
    public class DialogTargetManager : MonoBehaviour {

        // ����ģʽ
        public static DialogTargetManager Instance;

        [Header("�Ի�������")]
        public DialogController dialogController;

        [Header("�Ի��ļ�")]
        public List<TextAsset> AreaDialogTextAssets;

        [Header("���пɶԻ�����")]
        public List<DialogTarget> dialogTargets;
        Dictionary<int, DialogTarget> id_DialogTargetDic;

        public void RegisterTargets(DialogTarget dialogTarget) {
            if(dialogTargets == null) {
                dialogTargets = new List<DialogTarget>();
            }
            dialogTargets.Add(dialogTarget);
        }

        private void Awake() {
            InitDialogTargetManager();
        }

        public void InitDialogTargetManager() {
            Instance = this;

            if (dialogTargets == null) {
                dialogTargets = new List<DialogTarget>();
            }
            id_DialogTargetDic = new Dictionary<int, DialogTarget>();

            // ���ҽӵ� CSV �����ı��ļ���ת��Ϊ�Ի����ݽṹ
            List<EventPlaceDialogData> eventPlaceDialogDatas = DialogTextExplainer.GetInstance().SortEventPlaceDialogData(
                AreaDialogTextAssets
            );

            //Debug.Log(eventPlaceDialogDatas.Count);

            //Debug.Log(dialogTargets.Count);

            // �������е� DialogTarget �ű��б�ת��Ϊ�ֵ�ӳ��
            foreach (DialogTarget target in dialogTargets) {
                
                //����¼��ڵ������-��Ϸ�����ӳ��
                if (id_DialogTargetDic.ContainsKey(target.EventPlaceId)) {
                    id_DialogTargetDic[target.EventPlaceId] = target;
                    Debug.LogError("����ͬId������EventPlace�����ǲ�����ģ�");
                } else {
                    id_DialogTargetDic.Add(target.EventPlaceId, target);
                }
            }

            //Debug.Log(id_DialogTargetDic.Count);

            foreach (EventPlaceDialogData DialogData in eventPlaceDialogDatas) {
                
                if (id_DialogTargetDic.ContainsKey(DialogData.EventPlaceId)) {
                    //Ϊӳ���е�ÿһ�� �¼��ڵ����ø�ֵ
                    id_DialogTargetDic[DialogData.EventPlaceId].InitDialogTarget(DialogData);
                    
                }
            }



            

        }

        public void StartDialog(EventPlaceDialogData EventData) {
            dialogController.StartDialog(EventData);
        }

    }
}