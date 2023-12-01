using GameOff2023.Dialog.DialogControl;
using GameOff2023.GamePlay.Character;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameOff2023.Dialog {
    /// <summary>
    /// ���˽ű��ҽӵ����ԶԻ���Ŀ����
    /// </summary>
    public class DialogTarget : MonoBehaviour , IInteractable {

        [Header("�ڵ���Ϣ"), Tooltip("�ڵ�Id��ֵ����Ψһ")]
        public int EventPlaceId;

        [Header("��ʹ���ֶ�")]
        public string EventPlaceName;
        public EventPlaceArea PlaceArea;

        [Tooltip("�ڵ㸺�ص��¼�")]
        private EventPlaceDialogData eventPlaceData;
        public EventPlaceDialogData EventPlaceData { get => eventPlaceData; private set => eventPlaceData = value; }
        
        private bool interactable = true;
        public bool Interactable { get => interactable; private set => interactable = value; }
        

        public void InitDialogTarget(EventPlaceDialogData EventPlaceData) {
            //DialogTargetManager.Instance.RegisterTargets(this);
            this.EventPlaceData = EventPlaceData;
        }

        /// <summary>
        /// ���Ⱪ¶�Ĵ����Ի��ӿ�
        /// </summary>
        public void TakeInteract() {
            DialogTargetManager.Instance.StartDialog(EventPlaceData);
        }


    }

    public enum EventPlaceArea {
        StartScene,        //��Ϸ���ֵĵ�ͼ
    }
}