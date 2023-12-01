
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

        [Header("敌人ID，通过ID读取数据")]
        public int enemyID = 401;

        private float fadeDuration = 1.0f;

        [Header("敌人的ui")]
        [SerializeField] SliderBar hpSliderBar;
        [SerializeField] TMP_Text nextStepText;
        public float enemyOffset = 0.5f;

        protected override void InitController() {
            base.InitController();

            //baseData = new BaseData(30, 30);
        }

        /// <summary>
        /// 获取下一步敌人可以行走的步数
        /// </summary>
        public override int GetNextStep() {
            base.GetNextStep();
            System.Random random = new System.Random();
            return random.Next(3) + 1;
        }

        /// <summary>
        /// 设置ui
        /// </summary>
        public override void SetCharUI(int nextStep) {
            base.SetCharUI(nextStep);
            hpSliderBar.UpdateSliderBar(baseData.MaxHP, baseData.currentHP);
            nextStepText.text = "nextstep: " + nextStep.ToString();
        }

        public override void Hurt(float addExtent, float damage) {
            base.Hurt(addExtent, damage);

            // 变化数值
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

            // 其他
        }

        public override void Death() {
            base.Death();

            _animator.SetTrigger("Death");
            _animator.CrossFade("Death", 0.1f);

            // 离开当前单元格
            currMapGrid.RemoveCharFromGrid();
            currMapGrid = null;

            // 敌人消散
            StartCoroutine(FadeOutAndDestroyObject(fadeDuration));

        }

        IEnumerator FadeOutAndDestroyObject(float fadeDuration) {

            // 淡出的逻辑
            Renderer renderer = GetComponentInChildren<Renderer>();
            Color startColor = renderer.material.color;
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

            float startTime = Time.time;
            float elapsedTime = 0f;

            // 逐渐消失
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
        Shot,               // 子弹射击（可以替换子弹）
        Melee,              // 近战攻击
        Melee_Shift,        // 近战攻击 有位移技能
        AreaEffect          // 路过的区域留下持续伤害范围
    }
}