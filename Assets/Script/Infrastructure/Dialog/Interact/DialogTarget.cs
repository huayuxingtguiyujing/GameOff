using GameOff2023.Dialog.DialogControl;
using GameOff2023.GamePlay.Character;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameOff2023.Dialog {
    /// <summary>
    /// 将此脚本挂接到可以对话的目标上
    /// </summary>
    public class DialogTarget : MonoBehaviour , IInteractable {

        [Header("节点信息"), Tooltip("节点Id，值必须唯一")]
        public int EventPlaceId;

        [Header("待使用字段")]
        public string EventPlaceName;
        public EventPlaceArea PlaceArea;

        [Tooltip("节点负载的事件")]
        private EventPlaceDialogData eventPlaceData;
        public EventPlaceDialogData EventPlaceData { get => eventPlaceData; private set => eventPlaceData = value; }
        
        private bool interactable = true;
        public bool Interactable { get => interactable; private set => interactable = value; }
        

        public void InitDialogTarget(EventPlaceDialogData EventPlaceData) {
            //DialogTargetManager.Instance.RegisterTargets(this);
            this.EventPlaceData = EventPlaceData;
        }

        /// <summary>
        /// 向外暴露的触发对话接口
        /// </summary>
        public void TakeInteract() {
            DialogTargetManager.Instance.StartDialog(EventPlaceData);
        }


    }

    public enum EventPlaceArea {
        StartScene,        //游戏开局的地图
    }
}