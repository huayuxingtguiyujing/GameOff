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

        // �����˺�����
        public int damageRatio = 1;
        // ���ܷ���
        public bool ToRight = true;

        private MapController mapController;

        public SkillExcuter(BaseSkill curSkill, int damageRatio, MapController mapController, bool ToRight) {
            this.curSkill = curSkill;
            this.damageRatio = damageRatio;
            this.mapController = mapController;
            this.ToRight = ToRight;
        }

        public void ExcuteSkill(BaseController skillPerformer, BaseController skillDamageTarget) {
            
            Debug.Log(skillPerformer.name + "ʹ���˼���: " + curSkill.skillName + ",��������: " + ToRight);

            // �����ƽ�ֵ

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
            // TODO: ��Ҫʵ��
            if (skillPerformer is PlayerController) {
                mapController.InvokeDamageTargetChoose(skillPerformer, damage, (enemyIndex) => {
                    mapController.InvokeSlideJudge((damageRatio) => {
                        // ��������
                        skillPerformer.SetAttaking(damageRatio);
                        // curPerformer.Attack(), damageValue
                        mapController.CauseDamageInGrid(damage, enemyIndex, enemyIndex, skillPerformer, ToRight);
                        // target ���͵ļ��ܣ�Ӧ���ڻص��д���
                        // �����ƽ�ֵ
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
            // ѡ��ǰ��һ����Ϊ����Ŀ�� ( TODO���ĳɶ�������ΪĿ�� )
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
                // ��������ѻ�����
                mapController.InvokeSlideJudge(attackLogic);
            } else {
                attackLogic.Invoke(1);
            }
        }

        private void ExcuteScopeAttackSkill(int damage, int pushExtent, BaseController skillPerformer, BaseController skillDamageTarget, bool ToRight) {
            // ��Χ���� Ĭ����ǰ����һ����Χ���й���
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
                // ��������ѻ�����
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
                // ѡ������ֵ��С��һ����λ���ָ�����ֵ/�������ӹ�����
                MapGrid minHPGrid = mapController.GetGrid_CharHPTheLast();
                if(minHPGrid != null) {
                    mapController.RecoverHPInGrid(baseSkill.recoverHP, minHPGrid);
                    // ʵ�����ӹ��������߼�
                    minHPGrid.AddDamageCharInGrid(baseSkill.addDamage);
                }
                
            } else if (curSkill.scopeType == ScopeType.Scope) {
                // ѡ��һ����Χ
                MapGrid mapGrid = skillPerformer.currMapGrid;
                mapController.RecoverHPInScope(baseSkill.recoverHP, mapGrid, baseSkill.skillScopeValue, ToRight);
            } else if (curSkill.scopeType == ScopeType.AllGrid) {
                // ���и���,ִ��buff
                bool recoverEnemy = skillPerformer is EnemyController;
                mapController.RecoverHPInAll(baseSkill.recoverHP, recoverEnemy);
            }
        }
    }
}