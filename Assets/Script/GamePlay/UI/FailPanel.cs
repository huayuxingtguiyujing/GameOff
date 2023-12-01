using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameOff2023.UI {
    public class FailPanel : MonoBehaviour {

        [SerializeField] Image bgPartImage;
        [SerializeField] Image redPartImage;

        [SerializeField] Button restartLevelButton;
        [SerializeField] Button backToMenuButton;

        [Header("结算信息")]
        [SerializeField] TMP_Text failTextText;
        [SerializeField] TMP_Text bestGradeText;
        [SerializeField] TMP_Text curGradeText;

        // 只绑定一次事件
        static bool hasInit = false;

        // 背景颜色
        private Color startBgColor = new Color(0, 0, 0, 0);
        private Color endBgColor = new Color(0, 0, 0, 0.6f);

        Tweener tweener;

        /// <summary>
        /// 唤醒失败面板，配置失败面板的所有ui组件
        /// </summary>
        public void InvokeFailPanel(string levelName) {

            // 界面逐渐变黑
            bgPartImage.color = startBgColor;
            bgPartImage.DOColor(endBgColor, 1.5f);

            // 扩张结束后激活按钮
            restartLevelButton.gameObject.SetActive(false);
            backToMenuButton.gameObject.SetActive(false);

            // 出场动画，redpart扩张
            float originWidth = redPartImage.GetComponent<RectTransform>().sizeDelta.x;
            Vector2 originWAndH = new Vector2(originWidth, 100);
            Vector2 widthAndHeight = new Vector2(originWidth, 300);
            redPartImage.GetComponent<RectTransform>().sizeDelta = originWAndH;
            tweener = redPartImage.GetComponent<RectTransform>().DOSizeDelta(widthAndHeight, 1.5f);
            tweener.OnComplete(() => {
                restartLevelButton.gameObject.SetActive(true);
                backToMenuButton.gameObject.SetActive(true);
            });

            // 当前战绩
            //bestGradeText.text = "最好成绩: 1-1";
            curGradeText.text = "cur grade: " + levelName;

            if (!hasInit) {
                restartLevelButton.onClick.AddListener(RestartLevel);
                backToMenuButton.onClick.AddListener(BackToMenu);
            }

            Debug.Log("player fails!");
        }

        public void RestartLevel() {
            SceneManager.LoadScene("Menu");
        }

        public void BackToMenu() {
            SceneManager.LoadScene("Menu");
        }


    }
}