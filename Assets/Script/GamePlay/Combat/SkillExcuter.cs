using GameOff2023.Character;
using GameOff2023.GamePlay.Map;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

namespace GameOff2023.GamePlay.Combat {
    public class SkillExcuter {
        
        public BaseSkill curSkill { get; private set; }
        public BaseController skillPerformer { get; private set; }
        public BaseController skillDamageTarget { get; private set; }

        // 技能伤害倍率
        public int damageRatio = 1;
        // 技能方向
        public bool ToRight = true;

        private MapController mapController;

        public SkillExcuter(BaseSkill curSkill, int damageRatio, MapController mapController, bool ToRight) {
            this.curSkill = curSkill;
            this.damageRatio = damageRatio;
            this.mapController = mapController;
            this.ToRight = ToRight;
        }

        public void ExcuteSkill(BaseController skillPerformer, BaseController skillDamageTarget) {
            
            Debug.Log(skillPerformer.name + "使用了技能: " + curSkill.skillName + ",方向向右: " + ToRight);

            // 计算推进值

            switch (curSkill.skillType) {
                case SkillType.TargetAttack:
                    ExcuteTargetAttackSkill(
                        curSkill.skillDamage * damageRatio, curSkill.skillPushExtent, 
                        skillPerformer, skillDamageTarget, ToRight
                    );
                    break;
                case SkillType.FrontAttack:
                    ExcuteFrontAttackSkill(
                        curSkill.skillDamage * damageRatio + skillPerformer.GetSkillDamageModify(), 
                        curSkill.skillPushExtent, skillPerformer, skillDamageTarget, ToRight
                    );
                    break;
                case SkillType.ScopeAttack:
                    ExcuteScopeAttackSkill(
                        curSkill.skillDamage * damageRatio + skillPerformer.GetSkillDamageModify(), 
                        curSkill.skillPushExtent, skillPerformer, skillDamageTarget, ToRight
                    );
                    break;
                case SkillType.AllAttack:
                    ExcuteAllAttackSkill(
                        curSkill.skillDamage * damageRatio + skillPerformer.GetSkillDamageModify(), 
                        curSkill.skillPushExtent, skillPerformer, skillDamageTarget, ToRight
                    );
                    break;
                case SkillType.Buff:
                    ExcuteBuffSkill(
                        curSkill, skillPerformer, skillDamageTarget, ToRight
                    );

                    break;
            }
        }
    
        private void ExcuteTargetAttackSkill(int damage, int pushExtent, BaseController skillPerformer, BaseController skillDamageTarget, bool ToRight) {
            // TODO: 需要实现
            if (skillPerformer is PlayerController) {
                mapController.InvokeDamageTargetChoose(skillPerformer, damage, (enemyIndex) => {
                    mapController.InvokeSlideJudge((damageRatio) => {
                        // 攻击动画
                        skillPerformer.SetAttaking(damageRatio);
                        // curPerformer.Attack(), damageValue
                        mapController.CauseDamageInGrid(damage, enemyIndex, enemyIndex, skillPerformer, ToRight);
                        // target 类型的技能，应当在回调中触发
                        // 计算推进值
                        mapController.AddMoveExtent(pushExtent);
                        mapController.RoundOver();
                        mapController.CloseGridHandle();
                    });
                });
            } else {
                int playerIndex = mapController.GetPlayerGridIndex();
                mapController.CauseDamageInGrid(damage, playerIndex, playerIndex, skillPerformer, ToRight);
                mapController.AddMoveExtent(-pushExtent);
            }

        }

        private void ExcuteFrontAttackSkill(int damage, int pushExtent, BaseController skillPerformer, BaseController skillDamageTarget, bool ToRight) {
            // 选择前方一格作为攻击目标 ( TODO：改成对两格作为目标 )
            UnityAction<float> attackLogic = (damageRatio) => {
                skillPerformer.SetAttaking(damageRatio);
                int performerIndex = mapController.GetMapGridIndex(skillPerformer.currMapGrid);
                mapController.CauseDamageInGrid(damage, performerIndex, skillPerformer, ToRight);

                if (skillPerformer is PlayerController) {
                    mapController.AddMoveExtent(pushExtent);
                } else {
                    mapController.AddMoveExtent(-pushExtent);
                }
            };

            if (skillPerformer is PlayerController) {
                // 是玩家则唤醒滑动条
                mapController.InvokeSlideJudge(attackLogic);
            } else {
                attackLogic.Invoke(1);
            }
        }

        private void ExcuteScopeAttackSkill(int damage, int pushExtent, BaseController skillPerformer, BaseController skillDamageTarget, bool ToRight) {
            // 范围攻击 默认以前方的一个范围进行攻击
            UnityAction<float> attackLogic = (damageRatio) => {
                skillPerformer.SetAttaking(damageRatio);
                int performerIndex = mapController.GetMapGridIndex(skillPerformer.currMapGrid);
                int rec = curSkill.skillScopeValue;
                while (rec > 0) {
                    mapController.CauseDamageInGrid(damage, performerIndex, skillPerformer, ToRight, rec);
                    rec--;
                }

                if (skillPerformer is PlayerController) {
                    mapController.AddMoveExtent(pushExtent);
                } else {
                    mapController.AddMoveExtent(-pushExtent);
                }
            };

            if(skillPerformer is PlayerController) {
                // 是玩家则唤醒滑动条
                mapController.InvokeSlideJudge(attackLogic);
            } else {
                attackLogic.Invoke(1);
            }
            
        }

        private void ExcuteAllAttackSkill(int damage, int pushExtent, BaseController skillPerformer, BaseController skillDamageTarget, bool ToRight) {
            skillPerformer.SetAttaking(damageRatio);
            mapController.CauseDamageInAllGrid(damage, skillPerformer, ToRight);
        }

        private void ExcuteBuffSkill(BaseSkill baseSkill, BaseController skillPerformer, BaseController skillDamageTarget, bool ToRight) {
            
            if (curSkill.scopeType == ScopeType.Single) {
                // 选择生命值最小的一个单位，恢复生命值/或者增加攻击力
                MapGrid minHPGrid = mapController.GetGrid_CharHPTheLast();
                if(minHPGrid != null) {
                    mapController.RecoverHPInGrid(baseSkill.recoverHP, minHPGrid);
                    // 实现增加攻击力的逻辑
                    minHPGrid.AddDamageCharInGrid(baseSkill.addDamage);
                }
                
            } else if (curSkill.scopeType == ScopeType.Scope) {
                // 选择一个范围
                MapGrid mapGrid = skillPerformer.currMapGrid;
                mapController.RecoverHPInScope(baseSkill.recoverHP, mapGrid, baseSkill.skillScopeValue, ToRight);
            } else if (curSkill.scopeType == ScopeType.AllGrid) {
                // 所有格子,执行buff
                bool recoverEnemy = skillPerformer is EnemyController;
                mapController.RecoverHPInAll(baseSkill.recoverHP, recoverEnemy);
            }
        }
    }
}