using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.GamePlay.Map {
    [System.Serializable]
    public class MapSetting {

        [Header("关卡次序、名称")]
        public int levelSortNum;
        public string levelName;

        [Header("分界线回合，不同阶段产怪速度不同")]
        public int level01Num = 18;
        public int level02Num = 34;
        
        [Header("敌人的数目")]
        // 关卡开始时敌人的数目
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
        /// 本回合结束后，是否应当生成敌人（在一个回合结束后调用）
        /// </summary>
        /// <param name="roundNum"></param>
        /// <param name="playerIndexToDistination"></param>
        public bool ShouldGenerateNewEnemy(int roundNum, int playerIndexToDistination) {

            // 10 回合内 每4回合生成一次
            if ((roundNum % 4 == 0) && (roundNum < level01Num)) {
                return true;
            } else if ((roundNum % 3 == 0) && (roundNum >= level01Num) && (roundNum < level02Num)) {
                return true;
            } else if ((roundNum % 2 == 0) && (roundNum > level02Num)) {
                return true;
            }

            // 只有5步到达终点时
            if(playerIndexToDistination < 4) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取下一个应当生成的对象
        /// </summary>
        public EnemyType GetNextGenerateEnemy() {
            List<EnemyType> enemyTypes = new List<EnemyType>();

            // 随机刷新敌人
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
            
            // 处理越界
            if (RandonIndex > enemyTypes.Count - 1) {
                RandonIndex = enemyTypes.Count - 1;
            } else if(RandonIndex < 0) {
                RandonIndex = 0;
            }

            // 减掉要生成的敌人数目
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
        /// 获取关卡开始时 应当生成的敌人数目
        /// </summary>
        public List<EnemyType> GetEnemyWhenMapStart() {
            List<EnemyType> enemyTypes = new List<EnemyType>();
            for(int i = 0; i < startEnemyNum; i++) {
                enemyTypes.Add (GetNextGenerateEnemy());
            }
            return enemyTypes;
        }


        #region 获取关卡配置信息

        public static List<MapSetting> GetAllMapSetting() {
            List<MapSetting> mapSettings = new List<MapSetting>();

            // 在这里写每个关卡的配置
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
        /// 获得关卡配置信息
        /// </summary>
        public static List<MapSetting> GetMapSettingByLevel(int level) {
            List<MapSetting> rec = GetAllMapSetting();
            // 移除一些关卡
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