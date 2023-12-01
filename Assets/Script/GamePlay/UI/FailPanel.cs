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

        [Header("������Ϣ")]
        [SerializeField] TMP_Text failTextText;
        [SerializeField] TMP_Text bestGradeText;
        [SerializeField] TMP_Text curGradeText;

        // ֻ��һ���¼�
        static bool hasInit = false;

        // ������ɫ
        private Color startBgColor = new Color(0, 0, 0, 0);
        private Color endBgColor = new Color(0, 0, 0, 0.6f);

        Tweener tweener;

        /// <summary>
        /// ����ʧ����壬����ʧ����������ui���
        /// </summary>
        public void InvokeFailPanel(string levelName) {

            // �����𽥱��
            bgPartImage.color = startBgColor;
            bgPartImage.DOColor(endBgColor, 1.5f);

            // ���Ž����󼤻ť
            restartLevelButton.gameObject.SetActive(false);
            backToMenuButton.gameObject.SetActive(false);

            // ����������redpart����
            float originWidth = redPartImage.GetComponent<RectTransform>().sizeDelta.x;
            Vector2 originWAndH = new Vector2(originWidth, 100);
            Vector2 widthAndHeight = new Vector2(originWidth, 300);
            redPartImage.GetComponent<RectTransform>().sizeDelta = originWAndH;
            tweener = redPartImage.GetComponent<RectTransform>().DOSizeDelta(widthAndHeight, 1.5f);
            tweener.OnComplete(() => {
                restartLevelButton.gameObject.SetActive(true);
                backToMenuButton.gameObject.SetActive(true);
            });

            // ��ǰս��
            //bestGradeText.text = "��óɼ�: 1-1";
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