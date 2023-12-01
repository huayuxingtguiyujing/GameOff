using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.Utils.Achieve {
    /// <summary>
    /// �����ҵ�����
    /// </summary>
    [System.Serializable]
    public class PlayerData : BaseData {

        
        [Header("����")]
        public float score;
        public int killEnemyNum;

        public float placeHolder;

        // �ȼ��뾭��ֵ
        // TODO : д�����߼�
        public int curentLevel { get; private set; }
        private void TryRiseLevel() {
            while (currentExp >= needExp) {
                currentExp = currentExp - needExp;
                curentLevel++;

                // TODO��ˢ���������辭��ֵ

            }
        }
        public float currentExp { get; private set; }
        public void RiseExp(float addRise) {
            currentExp += addRise;
            TryRiseLevel();
        }
        public float needExp { get; private set; }

        public PlayerData(float currentHP, float maxHP, float maxDrawResource, float currentDrawResource, 
            float score, int killEnemyNum, float placeHolder, 
            int curentLevel, float currentExp, float needExp) : base(maxHP, currentHP) {
            this.maxResource = maxDrawResource;
            this.currentResource = currentDrawResource;
            this.score = score;
            this.killEnemyNum = killEnemyNum;
            this.placeHolder = placeHolder;
            this.curentLevel = curentLevel;
            this.currentExp = currentExp;
            this.needExp = needExp;
        }

        public PlayerData() { }
    }
}