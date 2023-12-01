using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameOff2023.UI.Common {
    /// <summary>
    /// �ҽ�ui��ť��ʵ�ְ�ť�¼�����Ƶ��ui�л�����
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class UISoundComponent : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
        [Header("��Ƶ��Դ")]
        public AudioSource audioSource;
        public AudioClip hoverSound;
        public AudioClip clickSound;
        public AudioClip notificationSound;

        [Header("����")]
        public bool enableHoverSound = true;
        public bool enableClickSound = true;

        void Start() {
            if (audioSource == null) {
                try {
                    audioSource = gameObject.GetComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                } catch {
                    Debug.LogError("UI Element Sound - Cannot initalize AudioSource due to missing resources.", this);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (enableHoverSound == true && audioSource != null)
                audioSource.PlayOneShot(hoverSound);
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (enableClickSound == true && audioSource != null)
                audioSource.PlayOneShot(clickSound);
        }

        public void Notification() {
            if (audioSource != null)
                audioSource.PlayOneShot(notificationSound);
        }
    }
}
