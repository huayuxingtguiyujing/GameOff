using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.GamePlay.Combat {
    [CreateAssetMenu(fileName = "skill", menuName = "GameOffObj/Skill")]
    public class BaseSkill : ScriptableObject {
        // 标识符
        public string skillName;
        public int skillID;

        [Header("技能的类型")]
        public SkillType skillType;
        public ScopeType scopeType;              // 技能影响范围

        [Header("范围型技能")]
        public int skillScopeValue;                        

        [Header("伤害类型技能")]
        public int skillDamage;             // 技能造成的伤害
        public int skillPushExtent;         // 推进的程度

        [Header("buff类型技能")]
        public int recoverHP;               // 恢复的生命

        public int addDamage;               // 增加的伤害
        public int buffExistRound;          // buff持续回合

    }

    public enum ScopeType {
        Single,         // 前方的一个范围
        Scope,              // 一个范围
        AllGrid             // 地图上所有
    }

    public enum SkillType {
        TargetAttack,             // 单个的指向型
        FrontAttack,              // 前方的格子（2格）
        ScopeAttack,              // 可选的一个区域
        AllAttack,
        // buff型
        Buff
    }
}