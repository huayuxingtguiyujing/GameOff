using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.GamePlay.UI {
    /// <summary>
    /// 弹出隐藏ui功能的简单封装，ui面板可以直接继承该类，需要挂接CanvasGroup组件
    /// </summary>
    public class BasePopUI : MonoBehaviour {

        //TODO: Pop栈，可以使用ESC快捷键关闭BasePopUI

        [Space]
        [Header("BasePopUI")]
        [SerializeField]
        protected CanvasGroup m_CanvasGroup;

        public bool Visible = false;

        public virtual void Show() {
            m_CanvasGroup.alpha = 1f;
            m_CanvasGroup.blocksRaycasts = true;
            Visible = true;
        }

        public virtual void Hide() {
            m_CanvasGroup.alpha = 0f;
            m_CanvasGroup.blocksRaycasts = false;
            Visible = false;
        }

    }
}