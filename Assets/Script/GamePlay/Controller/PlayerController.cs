
using GameOff2023.Dialog;
using GameOff2023.GamePlay;
using GameOff2023.GamePlay.UI;
using GameOff2023.Utils.Achieve;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Screen = UnityEngine.Screen;

namespace GameOff2023.Character {
    [RequireComponent(typeof(InputHandler))]
    public class PlayerController : BaseController
    {
        InputHandler inputHandler;

        //public PlayerData playerData;          //玩家属性（生命值、移速在basecontroller里）
        [Header("player的ui")]
        [SerializeField] SliderBar hpSliderBar;
        //[SerializeField] TMP_Text nextStepText;

        protected override void ExcuteInUpdate() {
            base.ExcuteInUpdate();
        }

        public override void SetCharUI(int nextStep) {
            base.SetCharUI(nextStep);
            hpSliderBar.UpdateSliderBar(baseData.MaxHP, baseData.currentHP);
        }

        public override void Hurt(float addExtent, float damage) {
            
            base.Hurt(addExtent, damage);

            //Debug.Log("player hurt");

            baseData.ModifyHP(-damage);
            if (baseData.IsDead()) {
                Death();
            }
            SetCharUI(1);
            //更改ui上的属性值
            //UIMgr.Instance.GetPanel<BattlePanel>().SetHealth(Convert.ToInt32(baseData.currentHP));
        }

        public override void Recover(float addExtent, float recover) {
            base.Recover(addExtent, recover);
            baseData.ModifyHP( recover);
            SetCharUI(1);
        }
        public override void Death() {
            base.Death();

            // TODO: 弹出死亡界面，执行玩家失败的逻辑
            Time.timeScale = 0;
            //BattleManager.Instance.Fail();
        }

    }
}
