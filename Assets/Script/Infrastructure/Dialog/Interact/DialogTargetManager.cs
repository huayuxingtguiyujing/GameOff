using GameOff2023.Dialog.DialogControl;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameOff2023.Dialog {
    /// <summary>
    /// 管理所有的DialogTarget物体
    /// </summary>
    public class DialogTargetManager : MonoBehaviour {

        // 单例模式
        public static DialogTargetManager Instance;

        [Header("对话控制器")]
        public DialogController dialogController;

        [Header("对话文件")]
        public List<TextAsset> AreaDialogTextAssets;

        [Header("所有可对话对象")]
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

            // 将挂接的 CSV 剧情文本文件，转化为对话数据结构
            List<EventPlaceDialogData> eventPlaceDialogDatas = DialogTextExplainer.GetInstance().SortEventPlaceDialogData(
                AreaDialogTextAssets
            );

            //Debug.Log(eventPlaceDialogDatas.Count);

            //Debug.Log(dialogTargets.Count);

            // 将场景中的 DialogTarget 脚本列表，转化为字典映射
            foreach (DialogTarget target in dialogTargets) {
                
                //添加事件节点的名称-游戏物体的映射
                if (id_DialogTargetDic.ContainsKey(target.EventPlaceId)) {
                    id_DialogTargetDic[target.EventPlaceId] = target;
                    Debug.LogError("存在同Id的两个EventPlace，这是不容许的！");
                } else {
                    id_DialogTargetDic.Add(target.EventPlaceId, target);
                }
            }

            //Debug.Log(id_DialogTargetDic.Count);

            foreach (EventPlaceDialogData DialogData in eventPlaceDialogDatas) {
                
                if (id_DialogTargetDic.ContainsKey(DialogData.EventPlaceId)) {
                    //为映射中的每一个 事件节点配置赋值
                    id_DialogTargetDic[DialogData.EventPlaceId].InitDialogTarget(DialogData);
                    
                }
            }



            

        }

        public void StartDialog(EventPlaceDialogData EventData) {
            dialogController.StartDialog(EventData);
        }

    }
}