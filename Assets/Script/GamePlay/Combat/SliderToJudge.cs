using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameOff2023.GamePlay.Combat {
    /// <summary>
    /// 滑动条，开启后不断滑动，按下space键获取一个返回值
    /// </summary>
    public class SliderToJudge : MonoBehaviour {

        [Header("指示器物体")]
        [SerializeField] Transform sliderIndicator;

        [Header("指示器移动速度")]
        [SerializeField] float indicatorMoveSpeed = 10.0f;

        [Header("边界")]
        [SerializeField] Image areaBG;
        [SerializeField] Transform leftEdge;
        [SerializeField] Transform rightEdge;

        [Header("暴击区域")]
        [SerializeField] Image specialAreaBG;
        [SerializeField] Transform leftSpecialEgde;
        [SerializeField] Transform rightSpecialEgde;

        bool runToRight = false;

        public float SliderValue {get; private set; }
        private UnityAction<float> sliderValueCallback;

        /// <summary>
        /// 初始化滑动条
        /// </summary>
        public void InitSliderJudge(UnityAction<float> callback) {
            sliderValueCallback = callback;
            sliderIndicator.position = areaBG.transform.position;
        }

        private void Update() {

            float leftEdgeValue = leftEdge.position.x;
            float rightEdgeValue = rightEdge.position.x;
            float specialLeftEdgeValue = leftSpecialEgde.position.x;
            float specialRightEdgeValue = rightSpecialEgde.position.x;
            float indicatorValue = sliderIndicator.position.x;

            if (runToRight) {
                sliderIndicator.position += new Vector3(Time.deltaTime * indicatorMoveSpeed, 0,0);
            } else {
                sliderIndicator.position -= new Vector3(Time.deltaTime * indicatorMoveSpeed, 0, 0);
            }

            // 超出边界 则更换行进方向
            if (indicatorValue <= leftEdgeValue) {
                runToRight = true;
            } else if (indicatorValue >= rightEdgeValue) {
                runToRight = false;
            }

            // 按下空格 获取返回值,并且隐藏
            if (Input.GetKeyDown(KeyCode.Space)) {

                if (leftEdgeValue <= indicatorValue && indicatorValue <= specialLeftEdgeValue) {
                    // 返回普通值
                    SliderValue = 1;
                } else if (specialRightEdgeValue <= indicatorValue && indicatorValue <= rightEdgeValue) {
                    // 返回普通值
                    SliderValue = 1;
                } else if (specialLeftEdgeValue <= indicatorValue && indicatorValue <= specialRightEdgeValue) {
                    // 返回暴击值
                    SliderValue = 2;
                }
                Debug.Log("press the space! and the callback invokes");
                sliderValueCallback.Invoke(SliderValue);
                gameObject.SetActive(false);
            }
        }


    }
}