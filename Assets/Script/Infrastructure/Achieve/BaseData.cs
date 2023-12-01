using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.Utils.Achieve {
    [System.Serializable]
    public class BaseData {

        [Header("��Դ/����")]
        public float maxResource;
        public float currentResource;
        public bool CostResource(float num) {
            //base.CostDrawResource(num);
            if (currentResource - num < 0) {
                return false;
            } else {
                currentResource -= num;
                return true;
            }

        }
        public void AddResource(float num) {
            //base.AddResource(num);
            currentResource += num;
        }


        [Header("��ɫ����ֵ")]
        [SerializeField] private float maxHP = 10.0f;
        public float MaxHP { get => maxHP; private set => maxHP = value; }
        public float currentHP;
        public void ModifyHP(float value) {
            currentHP = Mathf.Clamp(currentHP + value, 0, maxHP);
            Debug.Log("hp changes: current hp:" + currentHP.ToString() + ", max hp:" + maxHP.ToString());
        }
        public void AddMaxHP(float value) { 
            maxHP += value; 
        }
        public bool IsDead() {
            return currentHP <= 0;
        }

        // ����Һ��ӵ�����Ŀ(����)
        public int dropResource;
        public string dropBallName;
        public int dropBallNum;

        public BaseData(float maxHP, float currentHP) {
            MaxHP = maxHP;
            this.currentHP = currentHP;
        }
    
        public BaseData() { }

        public virtual void ResetData() {
            currentHP = maxHP;
        }

        //public virtual bool CostDrawResource(float num) { return false; }
        //public virtual void AddResource(float num) { }

    }
}