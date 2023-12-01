using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.GamePlay.Combat {
    [CreateAssetMenu(fileName = "skill", menuName = "GameOffObj/Skill")]
    public class BaseSkill : ScriptableObject {
        // ��ʶ��
        public string skillName;
        public int skillID;

        [Header("���ܵ�����")]
        public SkillType skillType;
        public ScopeType scopeType;              // ����Ӱ�췶Χ

        [Header("��Χ�ͼ���")]
        public int skillScopeValue;                        

        [Header("�˺����ͼ���")]
        public int skillDamage;             // ������ɵ��˺�
        public int skillPushExtent;         // �ƽ��ĳ̶�

        [Header("buff���ͼ���")]
        public int recoverHP;               // �ָ�������

        public int addDamage;               // ���ӵ��˺�
        public int buffExistRound;          // buff�����غ�

    }

    public enum ScopeType {
        Single,         // ǰ����һ����Χ
        Scope,              // һ����Χ
        AllGrid             // ��ͼ������
    }

    public enum SkillType {
        TargetAttack,             // ������ָ����
        FrontAttack,              // ǰ���ĸ��ӣ�2��
        ScopeAttack,              // ��ѡ��һ������
        AllAttack,
        // buff��
        Buff
    }
}