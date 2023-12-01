
using GameOff2023.Character;
using GameOff2023.GamePlay.Character;
using GameOff2023.GamePlay.Map;
using GameOff2023.GamePlay.UI;
using GameOff2023.Utils.Achieve;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using WarGame_True.Utils;

namespace GameOff2023.GamePlay {
    //[RequireComponent(typeof(BaseEnemyAI))]
    public class EnemyController : BaseController, IMapManageObject {

        [Header("����ID��ͨ��ID��ȡ����")]
        public int enemyID = 401;

        private float fadeDuration = 1.0f;

        [Header("���˵�ui")]
        [SerializeField] SliderBar hpSliderBar;
        [SerializeField] TMP_Text nextStepText;
        public float enemyOffset = 0.5f;

        protected override void InitController() {
            base.InitController();

            //baseData = new BaseData(30, 30);
        }

        /// <summary>
        /// ��ȡ��һ�����˿������ߵĲ���
        /// </summary>
        public override int GetNextStep() {
            base.GetNextStep();
            System.Random random = new System.Random();
            return random.Next(3) + 1;
        }

        /// <summary>
        /// ����ui
        /// </summary>
        public override void SetCharUI(int nextStep) {
            base.SetCharUI(nextStep);
            hpSliderBar.UpdateSliderBar(baseData.MaxHP, baseData.currentHP);
            nextStepText.text = "nextstep: " + nextStep.ToString();
        }

        public override void Hurt(float addExtent, float damage) {
            base.Hurt(addExtent, damage);

            // �仯��ֵ
            baseData.ModifyHP( - damage);
            hpSliderBar.UpdateSliderBar(baseData.MaxHP, baseData.currentHP);

            if (baseData.IsDead()) {
                Death();
            }
        }

        public override void Recover(float addExtent, float recover) {
            base.Recover(addExtent, recover); 
            baseData.ModifyHP( recover);
            hpSliderBar.UpdateSliderBar(baseData.MaxHP, baseData.currentHP);

            // ����
        }

        public override void Death() {
            base.Death();

            _animator.SetTrigger("Death");
            _animator.CrossFade("Death", 0.1f);

            // �뿪��ǰ��Ԫ��
            currMapGrid.RemoveCharFromGrid();
            currMapGrid = null;

            // ������ɢ
            StartCoroutine(FadeOutAndDestroyObject(fadeDuration));

        }

        IEnumerator FadeOutAndDestroyObject(float fadeDuration) {

            // �������߼�
            Renderer renderer = GetComponentInChildren<Renderer>();
            Color startColor = renderer.material.color;
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

            float startTime = Time.time;
            float elapsedTime = 0f;

            // ����ʧ
            while (elapsedTime < fadeDuration) {
                elapsedTime = Time.time - startTime;
                float t = elapsedTime / fadeDuration;
                renderer.material.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }

            // Ensure the object is completely invisible
            renderer.material.color = targetColor;

            Destroy(gameObject);
        }

        public void SetManageObject() {
            throw new NotImplementedException();
        }
    }

    public enum AttackType {
        Shot,               // �ӵ�����������滻�ӵ���
        Melee,              // ��ս����
        Melee_Shift,        // ��ս���� ��λ�Ƽ���
        AreaEffect          // ·�����������³����˺���Χ
    }
}