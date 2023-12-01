using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.GamePlay.Map {
    [System.Serializable]
    public class MapSetting {

        [Header("�ؿ���������")]
        public int levelSortNum;
        public string levelName;

        [Header("�ֽ��߻غϣ���ͬ�׶β����ٶȲ�ͬ")]
        public int level01Num = 18;
        public int level02Num = 34;
        
        [Header("���˵���Ŀ")]
        // �ؿ���ʼʱ���˵���Ŀ
        public int startEnemyNum = 3;

        public int attackFrontEnemyNum = 1;
        private bool GetAttackFrontEnemy() {
            if(attackFrontEnemyNum > 0) {
                attackFrontEnemyNum--;
                return true;
            }
            return false;
        }

        public int attackTargetEnemyNum = 1;
        private bool GetAttackTargetEnemy() {
            if (attackTargetEnemyNum > 0) {
                attackTargetEnemyNum--;
                return true;
            }
            return false;
        }

        public int attackScopeEnemyNum = 1;
        private bool GetAttackScopeEnemy() {
            if (attackScopeEnemyNum > 0) {
                attackScopeEnemyNum--;
                return true;
            }
            return false;
        }

        public int buffEnemyNum = 1;
        private bool GetBuffEnemy() {
            if (buffEnemyNum > 0) {
                buffEnemyNum--;
                return true;
            }
            return false;
        }

        public int enemyNum {
            get {
                int rec = attackFrontEnemyNum + attackTargetEnemyNum 
                    + attackScopeEnemyNum + buffEnemyNum;
                return rec;
            }
        }

        public MapSetting(int levelSortNum, string levelName, int level01Num, int level02Num, 
            int startEnemyNum, int attackFrontEnemyNum, int attackTargetEnemyNum, int attackScopeEnemyNum, int buffEnemyNum) {
            this.levelSortNum = levelSortNum;
            this.levelName = levelName;
            this.level01Num = level01Num;
            this.level02Num = level02Num;
            this.startEnemyNum = startEnemyNum;
            this.attackFrontEnemyNum = attackFrontEnemyNum;
            this.attackTargetEnemyNum = attackTargetEnemyNum;
            this.attackScopeEnemyNum = attackScopeEnemyNum;
            this.buffEnemyNum = buffEnemyNum;
        }

        public static MapSetting GetDefaultMapSetting() {
            return new MapSetting(99, "0-0", 16, 34, 3, 7, 5, 5, 0);
        }


        /// <summary>
        /// ���غϽ������Ƿ�Ӧ�����ɵ��ˣ���һ���غϽ�������ã�
        /// </summary>
        /// <param name="roundNum"></param>
        /// <param name="playerIndexToDistination"></param>
        public bool ShouldGenerateNewEnemy(int roundNum, int playerIndexToDistination) {

            // 10 �غ��� ÿ4�غ�����һ��
            if ((roundNum % 4 == 0) && (roundNum < level01Num)) {
                return true;
            } else if ((roundNum % 3 == 0) && (roundNum >= level01Num) && (roundNum < level02Num)) {
                return true;
            } else if ((roundNum % 2 == 0) && (roundNum > level02Num)) {
                return true;
            }

            // ֻ��5�������յ�ʱ
            if(playerIndexToDistination < 4) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// ��ȡ��һ��Ӧ�����ɵĶ���
        /// </summary>
        public EnemyType GetNextGenerateEnemy() {
            List<EnemyType> enemyTypes = new List<EnemyType>();

            // ���ˢ�µ���
            for(int i = 0; i < attackFrontEnemyNum; i++) {
                enemyTypes.Add(EnemyType.AttackFront);
            }
            for (int i = 0; i < attackTargetEnemyNum; i++) {
                enemyTypes.Add(EnemyType.AttackTarget);
            }
            for (int i = 0; i < attackScopeEnemyNum; i++) {
                enemyTypes.Add(EnemyType.AttackScope);
            }
            for (int i = 0; i < buffEnemyNum; i++) {
                enemyTypes.Add(EnemyType.BuffEnemy);
            }

            int RandonIndex = Random.Range(0, enemyTypes.Count);
            
            // ����Խ��
            if (RandonIndex > enemyTypes.Count - 1) {
                RandonIndex = enemyTypes.Count - 1;
            } else if(RandonIndex < 0) {
                RandonIndex = 0;
            }

            // ����Ҫ���ɵĵ�����Ŀ
            switch (enemyTypes[RandonIndex]) {
                case EnemyType.AttackFront:
                    GetAttackFrontEnemy();
                    break;
                case EnemyType.AttackTarget:
                    GetAttackTargetEnemy();
                    break;
                case EnemyType.AttackScope:
                    GetAttackScopeEnemy();
                    break;
                case EnemyType.BuffEnemy:
                    GetBuffEnemy();
                    break;
            }

            return enemyTypes[RandonIndex];
        }

        /// <summary>
        /// ��ȡ�ؿ���ʼʱ Ӧ�����ɵĵ�����Ŀ
        /// </summary>
        public List<EnemyType> GetEnemyWhenMapStart() {
            List<EnemyType> enemyTypes = new List<EnemyType>();
            for(int i = 0; i < startEnemyNum; i++) {
                enemyTypes.Add (GetNextGenerateEnemy());
            }
            return enemyTypes;
        }


        #region ��ȡ�ؿ�������Ϣ

        public static List<MapSetting> GetAllMapSetting() {
            List<MapSetting> mapSettings = new List<MapSetting>();

            // ������дÿ���ؿ�������
            MapSetting mapSetting1 = new MapSetting(
                1, "1-1", 18, 34, 3, 5, 3, 0, 0    
            );
            MapSetting mapSetting2 = new MapSetting(
                1, "1-2", 18, 34, 2, 5, 0, 3, 0
            );
            MapSetting mapSetting3 = new MapSetting(
                1, "1-3", 18, 34, 3, 3, 3, 0, 3
            );
            mapSettings.Add(mapSetting1);
            mapSettings.Add(mapSetting2);
            mapSettings.Add(mapSetting3);

            MapSetting mapSetting4 = new MapSetting(
                1, "2-1", 18, 34, 3, 5, 0, 0, 5
            );
            MapSetting mapSetting5 = new MapSetting(
                1, "2-2", 18, 34, 3, 3, 3, 3, 0
            );
            MapSetting mapSetting6 = new MapSetting(
                1, "2-3", 18, 34, 3, 5, 3, 0, 1
            );
            mapSettings.Add(mapSetting4);
            mapSettings.Add(mapSetting5);
            mapSettings.Add(mapSetting6);

            MapSetting mapSetting7 = new MapSetting(
                1, "3-1", 18, 34, 3, 4, 3, 2, 2
            );
            MapSetting mapSetting8 = new MapSetting(
                1, "3-2", 18, 34, 3, 3, 2, 3, 3
            );
            MapSetting mapSetting9 = new MapSetting(
                1, "3-3", 18, 34, 3, 5, 3, 3, 3
            );
            mapSettings.Add(mapSetting7);
            mapSettings.Add(mapSetting8);
            mapSettings.Add(mapSetting9);

            return mapSettings;
        }

        /// <summary>
        /// ��ùؿ�������Ϣ
        /// </summary>
        public static List<MapSetting> GetMapSettingByLevel(int level) {
            List<MapSetting> rec = GetAllMapSetting();
            // �Ƴ�һЩ�ؿ�
            for (int i = 1; i < level; i ++) {
                rec.RemoveAt(0);
            }

            return rec;
        }

        #endregion

    }

    public enum EnemyType {
        AttackFront,
        AttackTarget,
        AttackScope,
        BuffEnemy,
        NoEnemy
    }

}