using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameOff2023.UI.Common {
    /// <summary>
    /// 挂接ui按钮，实现按钮事件的音频、ui切换功能
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class UISoundComponent : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
        [Header("音频资源")]
        public AudioSource audioSource;
        public AudioClip hoverSound;
        public AudioClip clickSound;
        public AudioClip notificationSound;

        [Header("设置")]
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
