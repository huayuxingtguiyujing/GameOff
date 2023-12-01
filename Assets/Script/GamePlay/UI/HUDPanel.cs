using GameOff2023.GamePlay.Application;
using GameOff2023.GamePlay.Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameOff2023.GamePlay.UI {
    public class HUDPanel : MonoBehaviour {

        public static HUDPanel Instance;

        [SerializeField] Button generateEnemyButton;
        [SerializeField] Button clearAllButton;
        [SerializeField] Button overRoundButton;
        [SerializeField] Button ExitGameButton;

        [Header("��ӡ��Ϣ����Ļ��")]
        [SerializeField] GameObject messageParent;
        [SerializeField] GameObject messagePrefab;

        [Header("ui���")]
        [SerializeField] TMP_Text curPerformerText;
        [SerializeField] TMP_Text roundNumText;
        [SerializeField] TMP_Text moveExtentText;
        [SerializeField] TMP_Text playerActionNumText;
        [SerializeField] TMP_Text levelMesText;
        // ���ܰ�ť
        [SerializeField] Button skill01Button;
        [SerializeField] Button skill02Button;
        [SerializeField] Button skill03Button;
        [SerializeField] Button skill04Button;
        // �ƶ���ť
        [SerializeField] Button moveToLeftButton;
        [SerializeField] Button moveToRightButton;

        [Header("�ж���ָʾ��")]
        [SerializeField] ActionStatus actionStatus;

        [Header("�ƶ�ֵָʾ��")]
        [SerializeField] MoveValueBar moveValueBar;

        [Header("ʣ�����")]
        [SerializeField] TMP_Text lastEnemyText;

        private void Awake() {
            Instance = this;

            generateEnemyButton.onClick.AddListener(GenerateEnemy);
            clearAllButton.onClick.AddListener(ClearAllEnemy);
            overRoundButton.onClick.AddListener(OverRound);
            ExitGameButton.onClick.AddListener(ExitGame);

            skill01Button.onClick.AddListener(Skill01);
            skill02Button.onClick.AddListener(Skill02);
            skill03Button.onClick.AddListener(Skill03);
            skill04Button.onClick.AddListener(Skill04);

            moveToLeftButton.onClick.AddListener(MoveToLeft);
            moveToRightButton.onClick.AddListener(MoveToRight);
        }

        public void SetPanelActive(bool active) {
            skill01Button.enabled = active;
            skill02Button.enabled = active;
            skill03Button.enabled = active;
            skill04Button.enabled = active;

            moveToLeftButton.enabled = active;
            moveToRightButton.enabled = active;
        }

        #region ��ť�¼�
        private void GenerateEnemy() {
            MapController.Instance.GenerateEnemy();
        }

        private void ClearAllEnemy() {

        }

        private void OverRound() {
            //if (MapController.Instance.IsPlayerPerformer) {
            //    MapController.Instance.RoundOver();
            //}
            //ApplicationController.Instance.PlayerFail();
        }

        private void ExitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }


        private void MoveToLeft() {
            MapController.Instance.MoveCharToLeft();
        }

        private void MoveToRight() {
            MapController.Instance.MoveCharToRight();
        }

        /// <summary>
        /// ʹ����ҵļ���
        /// </summary>
        private void Skill01() {
            MapController.Instance.UsePlayerSkill(0);
        }

        private void Skill02() {
            MapController.Instance.UsePlayerSkill(1);
        }

        private void Skill03() {
            MapController.Instance.UsePlayerSkill(2);
        }

        private void Skill04() {
            MapController.Instance.UsePlayerSkill(3);
        }
        #endregion

        #region ui��Ϣ
        public void UpdateRoundMes(int roundNum, string curPerformer, int curMoveExtent, int playerActionNum, int lastEnemy, string levelMes) {
            curPerformerText.text = "Round: " + curPerformer;
            roundNumText.text = "Round Num: " + roundNum.ToString();
            moveValueBar.SetMoveValueBar(curMoveExtent);
            moveExtentText.text = "Move: " + curMoveExtent.ToString();
            actionStatus.SetActionStatus(playerActionNum);
            //playerActionNumText.text = "action: " + playerActionNum.ToString();
            lastEnemyText.text = "Enemy Num:" + lastEnemy.ToString();
            levelMesText.text = "Level:" + levelMes;
        }

        public void LogMesToHUD(string message) {

        }
        #endregion

    }
}