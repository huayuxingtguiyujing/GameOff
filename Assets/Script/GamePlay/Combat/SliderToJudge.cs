using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameOff2023.GamePlay.Combat {
    /// <summary>
    /// �������������󲻶ϻ���������space����ȡһ������ֵ
    /// </summary>
    public class SliderToJudge : MonoBehaviour {

        [Header("ָʾ������")]
        [SerializeField] Transform sliderIndicator;

        [Header("ָʾ���ƶ��ٶ�")]
        [SerializeField] float indicatorMoveSpeed = 10.0f;

        [Header("�߽�")]
        [SerializeField] Image areaBG;
        [SerializeField] Transform leftEdge;
        [SerializeField] Transform rightEdge;

        [Header("��������")]
        [SerializeField] Image specialAreaBG;
        [SerializeField] Transform leftSpecialEgde;
        [SerializeField] Transform rightSpecialEgde;

        bool runToRight = false;

        public float SliderValue {get; private set; }
        private UnityAction<float> sliderValueCallback;

        /// <summary>
        /// ��ʼ��������
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

            // �����߽� ������н�����
            if (indicatorValue <= leftEdgeValue) {
                runToRight = true;
            } else if (indicatorValue >= rightEdgeValue) {
                runToRight = false;
            }

            // ���¿ո� ��ȡ����ֵ,��������
            if (Input.GetKeyDown(KeyCode.Space)) {

                if (leftEdgeValue <= indicatorValue && indicatorValue <= specialLeftEdgeValue) {
                    // ������ֵͨ
                    SliderValue = 1;
                } else if (specialRightEdgeValue <= indicatorValue && indicatorValue <= rightEdgeValue) {
                    // ������ֵͨ
                    SliderValue = 1;
                } else if (specialLeftEdgeValue <= indicatorValue && indicatorValue <= specialRightEdgeValue) {
                    // ���ر���ֵ
                    SliderValue = 2;
                }
                Debug.Log("press the space! and the callback invokes");
                sliderValueCallback.Invoke(SliderValue);
                gameObject.SetActive(false);
            }
        }


    }
}