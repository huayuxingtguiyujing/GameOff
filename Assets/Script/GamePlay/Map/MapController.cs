using GameOff2023.Character;
using GameOff2023.GamePlay.Application;
using GameOff2023.GamePlay.Combat;
using GameOff2023.GamePlay.UI;
using GameOff2023.Infrastructure.Audio;
using GameOff2023.UI;
using GameOff2023.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WarGame_True.Utils;

namespace GameOff2023.GamePlay.Map {
    public class MapController : MonoBehaviour, IRoundInterface {

        public static MapController Instance { get; private set; }

        [SerializeField] HUDPanel hudPanel;
        [SerializeField] LoadingPanel loadingPanel;

        #region 地图网格 相关变量

        [Header("红色格子")]
        [SerializeField] MapGrid redMapGrid;

        [Header("生成地图网格")]
        //[SerializeField] float mapGridInterval;
        [SerializeField] int mapGridNum = 10;
        [SerializeField] Transform mapGridParent;
        [SerializeField] GameObject mapGridPrefab;

        public List<MapGrid> mapGrids = new List<MapGrid>();
        #endregion

        #region 游戏回合 相关变量
        // 当前的行动者
        public BaseController currPerformer;
        public bool IsPlayerPerformer { get => (currPerformer != null) && (currPerformer is PlayerController); }
        // 行动队列
        public Queue<BaseController> charActionQueue = new Queue<BaseController>();

        private int currPerformerIndex = 0;
        private int roundNum = 1;

        #endregion

        #region 场上单位 相关变量
        [Header("当前玩家")]
        public PlayerController playerController;

        [Header("敌人的prefab")]
        [SerializeField] GameObject EnemyPrefab;
        [SerializeField] GameObject AttackFrontPrefab;
        [SerializeField] GameObject AttackTargetPrefab;
        [SerializeField] GameObject AttackScopePrefab;
        [SerializeField] GameObject BuffEnemyPrefab;

        [Header("关卡的配置信息")]
        [SerializeField] List<MapSetting> mapSettingList;
        public MapSetting curMapSetting;

        [Header("攻击判定条")]
        [SerializeField] SliderToJudge sliderToJudge;

        #endregion

        private void Start() {
            Instance = this;

            // 初始化 关卡信息，从第一关开始
            mapSettingList = MapSetting.GetMapSettingByLevel(1);

            MapStart();
        }

        public void CreateMapGrids() {
            mapGridParent.gameObject.ClearObjChildren();

            mapGrids = new List<MapGrid>();
            float mapGridInterval = mapGridPrefab.GetComponentInChildren<SpriteRenderer>().size.x / 2 + 1;

            // 根据预先给定的grid间距、grid数目绘图
            float offset = 0;
            for (int i = 0; i < mapGridNum; i++) {
                GameObject mapGridObj = Instantiate(mapGridPrefab, mapGridParent);
                MapGrid mapGrid = mapGridObj.GetComponent<MapGrid>();

                if (i == 0) {
                    // 设定 起点为玩家生成区
                    mapGrid.SetMapGrid(this, 0, MapGridType.LeftEdge);
                } else if (i == mapGridNum - 1) {
                    // 设定终止点为 怪物生成区
                    mapGrid.SetMapGrid(this, mapGridNum - 1, MapGridType.RightEdge);
                } else {
                    mapGrid.SetMapGrid(this, i, MapGridType.Normal);
                }

                mapGrids.Add(mapGrid);
                
                mapGridObj.transform.position += new Vector3(offset,0,0);
                offset += mapGridInterval;
            }



        }

        #region 回合管理、关卡管理

        /// <summary>
        /// 判断是否所有的行动者都已经采取了行动
        /// </summary>
        public bool PerformerActionOver() {
            int allnum = charActionQueue.Count;
            if (allnum < 1) allnum = 1;
            if (roundNum % allnum == 0) {
                Debug.Log("一个大回合已经结束了，所有单位都已经行动完毕");
                return true;
            } else {
                return false;
            }
        }

        private BaseController GetNextPerformer() {
            if (currPerformer != null) {
                currPerformer.SetNormal();
                charActionQueue.Enqueue(currPerformer);
            }

            // 获得出队的单位作为当前行动者
            currPerformer = charActionQueue.Dequeue();
            while(currPerformer == null) {
                currPerformer = charActionQueue.Dequeue();
            }
            currPerformer.SetCurrentActor();
            currPerformerIndex++;

            // 一整个回合轮结束
            if (currPerformerIndex > charActionQueue.Count) {
                currPerformerIndex = 0;
                roundNum++;
            }
            //Debug.Log("Next Round! : " + currPerformer.name);
            return currPerformer;
        }

        private void AddChar(BaseController baseController) {
            if (charActionQueue == null) {
                charActionQueue = new Queue<BaseController>();
            }
            if (charActionQueue.Contains(baseController)) {
                return;
            }
            charActionQueue.Enqueue(baseController);
        }

        private MapSetting GetNextLevel() {
            if (mapSettingList.Count >= 1) {
                curMapSetting = mapSettingList[0];
                mapSettingList.RemoveAt(0);
            } else {
                curMapSetting = MapSetting.GetDefaultMapSetting();
            }
            
            return curMapSetting;
        }

        public void MapStart() {
            AudioManager.PlayAudio("BattleLoop2");

            InitMap();
            // 进入下一个关卡
            GetNextLevel();

            // 产生初始敌人
            List<EnemyType> enemyTypes = curMapSetting.GetEnemyWhenMapStart();
            foreach (var enemyType in enemyTypes) {
                GenerateEnemy(enemyType);
            }

            RoundStart();
        }

        private void InitMap() {

            loadingPanel.gameObject.SetActive(true);
            // 设置关卡的初始值
            currPerformerIndex = 0;
            roundNum = 1;
            ClearAllEnemy();

            // 初始化队列
            AddChar(playerController);
            //playerController.currMapGrid = GetLeftEdgeGrid();
            GetLeftEdgeGrid().AddCharToGrid(playerController, false);

            // 取消唤醒所有操作面板
            foreach (MapGrid grid in mapGrids) {
                grid.CloseGridHandle();
            }
            StartCoroutine(HideLoading());
        }

        IEnumerator HideLoading() {
            yield return new WaitForSeconds(0.6f);
            loadingPanel.gameObject.SetActive(false);
        }

        public void RoundStart() {

            playerActionNum = playerMaxActionNum;
            
            GetNextPerformer();

            hudPanel.UpdateRoundMes(
                    roundNum, currPerformer.name, RedMoveExtent, playerActionNum, curMapSetting.enemyNum, curMapSetting.levelName);

            // 判断是否完成了一个大周期
            if (PerformerActionOver()) {
                int playerIndexToDestination = mapGrids.Count - GetPlayerGridIndex();
                Debug.Log("playerindex:" + playerIndexToDestination +  ",本轮可以生成: " + curMapSetting.ShouldGenerateNewEnemy(roundNum, playerIndexToDestination));
                // 判断是否要生成敌人单位
                if (curMapSetting.ShouldGenerateNewEnemy(roundNum, playerIndexToDestination)) {
                GenerateEnemy(curMapSetting.GetNextGenerateEnemy());
                }
                roundNum++;
            }
            
            // 刷新ui、执行玩家/敌人的ai
            if (currPerformer is PlayerController) {
                // 是玩家 根据玩家当前的位置 唤醒所有操作面板
                InvokeHUDHandle();
                //Debug.Log(" player's round start! " + "player can choose an action");
            } else {
                
                // 攻击范围与 到玩家的距离
                int attackScope = currPerformer.GetNormalAttackScope();
                int distance = GetMapGridIndex(currPerformer.currMapGrid) - GetPlayerGridIndex();

                MapGrid leftGrid = GetMapGrid_Neighbor(GetMapGridIndex(currPerformer.currMapGrid), 1, false);
                if (distance <= attackScope) {
                    // 玩家在攻击范围内 则进行攻击
                    Debug.Log("检测到玩家 进行攻击!");
                    // 根据使用的技能 切换攻击动画
                    //currPerformer.SetAttaking(1);
                    UseSkill(currPerformer, currPerformer.GetUseSkill(), false, 1);

                } else if (leftGrid.mapGridType == MapGridType.Execute && CanPushRedGrid(false)) {
                    
                    // 前方是红色格子 且可以推进 则推进之
                    leftGrid.MoveToGrid(currPerformer, false);
                    MoveRedGrid(false);
                } else {
                    
                    List<MapGrid> pathGrid = GetMapGrids_ByStep(
                        GetMapGridIndex(currPerformer.currMapGrid),
                        currPerformer.nextStepNum, false
                    );
                    // 移动到最远的可移动格子
                    MapGrid farAndWalkableGrid = GetWalkableGrid(pathGrid, currPerformer.nextStepNum);
                    if (farAndWalkableGrid != null) {
                        farAndWalkableGrid.MoveToGrid(currPerformer, false);
                        Debug.Log(currPerformer.name + "'s round start! " +
                            "; move step: " + currPerformer.nextStepNum.ToString() +
                            "; move des: " + farAndWalkableGrid.gridIndex);
                    }
                }
                RoundOver();
            }

        }

        int playerActionNum = 2;
        int playerMaxActionNum = 2;
        public void RoundOver() {

            // 玩家一回合内可以进行多次行动
            if (currPerformer is PlayerController) {
                playerActionNum--;
                hudPanel.UpdateRoundMes(
                    roundNum, currPerformer.name, RedMoveExtent, playerActionNum, curMapSetting.enemyNum, curMapSetting.levelName);

                if (playerActionNum > 0) {
                    // 还有剩余的行动数
                    return;
                }
            }
            
            hudPanel.UpdateRoundMes(
                roundNum, currPerformer.name, RedMoveExtent, playerActionNum, curMapSetting.enemyNum, curMapSetting.levelName);

            Debug.Log("一个回合已经结束了！:" + roundNum);

            // 关闭操作面板
            CloseHUDHandle();
            currPerformer.SetCharUI(currPerformer.GetNextStep());

            // TODO: 判断是否满足了结束关卡条件
            int playerIndex = GetPlayerGridIndex();
            if (playerIndex >= mapGrids.Count - 1) {
                // 条件1: 玩家当前位置处于最后一格
                MapOver();
            } else if (curMapSetting.enemyNum <= 0) {
                // 条件2: 所有敌人均被消灭
                MapOver();
            } else if (playerController.baseData.IsDead()) {
                // 条件3: 玩家死亡，触发fail面板
                ApplicationController.Instance.PlayerFail(curMapSetting.levelName);
                return;
            } else {
                Invoke("RoundStart", 1.0f);
            }

        }

        public void MapOver() {

            Debug.Log("跳转到下一关！");

            InitMap();

            // TODO：跳转到下一关
            GetNextLevel();
            if (curMapSetting != null) {
                // 游戏结束！
                ApplicationController.Instance.GameOver();
            }

            // 产生初始敌人
            List<EnemyType> enemyTypes = curMapSetting.GetEnemyWhenMapStart();
            foreach (var enemyType in enemyTypes) {
                GenerateEnemy(enemyType);
            }

            // 刷新ui，更换背景图
            hudPanel.UpdateRoundMes(
                    roundNum, currPerformer.name, RedMoveExtent, playerActionNum, curMapSetting.enemyNum, curMapSetting.levelName);

            RoundStart();
        }

        #endregion

        #region 地图格子管理

        /// <summary>
        /// 获得最远的可以行走的格子
        /// </summary>
        public MapGrid GetWalkableGrid(List<MapGrid> mapGrids, int curStep, bool IsPlayer = false) {
            
            MapGrid ans = null;
            int count = mapGrids.Count;
            for (int i = 0; i < count; i++) {
                if (i > curStep) {
                    break;
                } else {
                    // 不能走到处决格子上
                    if (mapGrids[i].mapGridType == MapGridType.Execute) {
                        return ans;
                    }

                    if (IsPlayer) {
                        // 是玩家 则检查是否有敌人在格子上 无则设置它为移动目标
                        if (!mapGrids[i].existEnemyInGrid) {
                            ans = mapGrids[i];
                        } else {
                            return ans;
                        }

                    } else {
                        if (!mapGrids[i].existPlayerInGrid) {
                            ans = mapGrids[i];
                        } else {
                            return ans;
                        }
                    }
                }
            }
            return ans;

        }
        /// <summary>
        /// 通过步数和方向获得可行走的地图格子
        /// </summary>
        public List<MapGrid> GetMapGrids_ByStep(int index, int step = 1, bool toRight = true) {
            List<MapGrid> ans = new List<MapGrid>();
            if (toRight) {
                for (int i = 1; i <= step; i++) {
                    if (index + i >= mapGrids.Count || index + i < 0) {
                        continue;
                    } else {
                        // 添加顺序为 自index的中心(低序位) 向两边(高序位)
                        ans.Add(mapGrids[index + i]);
                    }
                }

            } else {
                for (int i = 1; i <= step; i++) {
                    if (index - i >= mapGrids.Count || index - i < 0) {
                        continue;
                    } else {
                        ans.Add(mapGrids[index - i]);
                    }
                }
            }
            return ans;
        }
        public MapGrid GetMapGrid_Neighbor(int index, int step = 1, bool toRight = true) {
            if (toRight) {
                if (index + step >= mapGrids.Count || index + step < 0) {
                    return null;
                } else {
                    // 添加顺序为 自index的中心(低序位) 向两边(高序位)
                    return mapGrids[index + step];
                }

            } else {
                if (index - step >= mapGrids.Count || index - step < 0) {
                    return null;
                } else {
                    return mapGrids[index - step];
                }
            }
        }
        public MapGrid GetLeftEdgeGrid() {
            if (mapGrids.Count <= 0) {
                return null;
            }
            return mapGrids[0];
        }
        public MapGrid GetRightEdgeGrid() {
            if (mapGrids.Count <= 0) {
                return null;
            }
            int index = mapGrids.Count - 1;
            return mapGrids[index];
        }
        
        /// <summary>
        /// 判断一个grid中是否有敌人/玩家
        /// </summary>
        public bool ExistEnemyInGrids(List<MapGrid> recs) {
            foreach (var grid in recs)
            {
                if (grid.existEnemyInGrid) {
                    return true;
                }
            }
            return false;
        }
        public bool ExistPlayerInGrids(List<MapGrid> recs) {
            foreach (var grid in recs) {
                if (grid.existPlayerInGrid) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 根据mapgrid获取到该mapgrid的index
        /// </summary>
        public int GetMapGridIndex(MapGrid mapGrid) {
            for (int i = 0; i < mapGrids.Count; i++) {
                if (mapGrids[i] == mapGrid) {
                    return i;
                }
            }
            return 0;
        }
        public int GetPlayerGridIndex() {
            for(int i = 0; i < mapGrids.Count; i++) {
                if (mapGrids[i].existPlayerInGrid) {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 唤醒grid的操作面板
        /// </summary>
        public void InvokeGridHandle_ExistEnemy(BaseController currPerformer, float curDamage, UnityAction<int> buttonCallback) {
            foreach (var grid in mapGrids) {
                if(grid.existEnemyInGrid) {
                    grid.InvokeGridHandle(currPerformer, curDamage, buttonCallback);
                }
            }
        }
        public void CloseGridHandle() {
            for (int i = 0; i < mapGrids.Count; i++) {
                mapGrids[i].CloseGridHandle();
            }
        }

        /// <summary>
        /// 允许玩家操作HUD界面
        /// </summary>
        public void InvokeHUDHandle() {
            HUDPanel.Instance.SetPanelActive(true);
        }
        public void CloseHUDHandle() {
            HUDPanel.Instance.SetPanelActive(false);
        }

        public int RedMoveExtent = 0;
        public void AddMoveExtent(int extent) {
            RedMoveExtent += extent;
            if(RedMoveExtent <= -6) {
                // 向左移动
                RedMoveExtent = 0;
                MoveRedGrid(false);
            } else if (RedMoveExtent >= 6) {
                // 向右移动
                RedMoveExtent = 0;
                MoveRedGrid(true);
            }
        }
        /// <summary>
        /// 移动红色处决格子
        /// </summary>
        public void MoveRedGrid(bool ToRight, int step = 1) {
            int redIndex = GetMapGridIndex(redMapGrid);
            MapGrid moveTarget = GetMapGrid_Neighbor(redIndex, step, ToRight);
            if (moveTarget != null) {
                redMapGrid.SetAsNormalGrid();
                redMapGrid = moveTarget;
                redMapGrid.SetAsRedGrid();
                // 立刻杀死格子上的怪物，将玩家推后一格
                if (redMapGrid.existEnemyInGrid) {
                    redMapGrid.ExcuteOnGridChar();
                } else if(redMapGrid.existPlayerInGrid){
                    int redIndex_new = GetMapGridIndex(redMapGrid);
                    MapGrid pushDes = GetMapGrid_Neighbor(redIndex_new, 1, false);
                    if (pushDes == null) {
                        // 推到了最边上 游戏结束
                        //MapOver();
                    }else if (!pushDes.existCharInGrid) {
                        BaseController baseController = redMapGrid.curCharInGrid;
                        pushDes.AddCharToGrid(baseController, false);
                    }
                    Debug.Log("红色格子上有玩家！！！推动之");
                } else {
                    // 其他情况 不做处理
                }
            } else {
                // 越界的情况 （推到左边 玩家死）（推到右边 玩家赢）
                if(redIndex == 0) {

                }else if(redIndex == mapGrids.Count - 1) {

                }
                Debug.LogError("无法移动格子！");
            }
            Debug.Log("推动了红色格子！");
        }
        public bool CanPushRedGrid(bool ToRight) {
            // 检查红色格子 左边/右边 有无单位
            int redGridIndex = GetMapGridIndex(redMapGrid);
            MapGrid grid = GetMapGrid_Neighbor(redGridIndex, 1, ToRight);
            if (grid.existCharInGrid) {
                return false;
            } else {
                return true;
            }
        }
        // 什么时候需要移动红色格子？玩家被攻击、敌人被攻击、玩家敌人推动格子

        #endregion

        #region 单位可进行的操作

        /// <summary>
        /// 将一个指定的角色移动的左边一格
        /// </summary>
        public void MoveCharToLeft() {

            // 超过了当前可移动数目

            if (!IsPlayerPerformer) return;
            int curPlayerIndex = GetMapGridIndex(currPerformer.currMapGrid);
            // 获取左边的格子 移动到该格子上
            MapGrid leftGrid = GetMapGrid_Neighbor(curPlayerIndex, 1, false);
            if (leftGrid != null) {
                leftGrid.MoveToGrid(currPerformer, false);
                //RoundOver();
            }
            RoundOver();
        }

        public void MoveCharToRight() {
            if (!IsPlayerPerformer) return;
            int curPlayerIndex = GetMapGridIndex(currPerformer.currMapGrid);
            MapGrid rightGrid = GetMapGrid_Neighbor(curPlayerIndex, 1, true);

            if (rightGrid == null) {
                RoundOver();
                return;
            }

            if(rightGrid.mapGridType == MapGridType.Execute && !CanPushRedGrid(true)) {
                // 是红色格子 且格子右边有敌人单位 则不能走到上面

            }else if (rightGrid.mapGridType == MapGridType.Execute && CanPushRedGrid(true)) {
                // 是红色格子 且格子右边没有敌人单位 则推进之
                rightGrid.MoveToGrid(currPerformer, true);
                MoveRedGrid(true);
            } else {
                // 其他情况 正常向右走
                rightGrid.MoveToGrid(currPerformer, true);
            }

            RoundOver();
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="skill">要执行的技能</param>
        /// <param name="ToRight">技能的方向</param>
        public void UseSkill(BaseController skillPerformer, BaseSkill skill, bool ToRight, int ratio) {
            
            Debug.Log(skillPerformer.name + "使用了技能: " + skill.skillName + ",方向向右: " + ToRight);

            SkillExcuter skillExcuter = new SkillExcuter(skill, ratio, this, ToRight);
            skillExcuter.ExcuteSkill(skillPerformer, null);

            return;
        }

        public void UsePlayerSkill(int skillIndex = 0) {
            UseSkill(playerController, playerController.baseSkills[skillIndex], true, 1);
            if (playerController.baseSkills[skillIndex].skillType == SkillType.TargetAttack) {

            } else {
                RoundOver();
            }
            return;
        }

        /// <summary>
        /// 让玩家选择格子，传入回调，执行操作
        /// </summary>
        public void PlayerChooseGrid(UnityAction<MapGrid> callback) {

            // TODO: 激活所有mapgrid，让他们让玩家选择

            // TODO: 传入回调

        }

        /// <summary>
        /// 在指定的格子上造成伤害
        /// </summary>
        public void CauseDamageInGrid(int damage, int performerIndex, BaseController performer, bool ToRight, int step = 1) {
            //if (!IsPlayerPerformer) return;
            // 根据玩家的位置，向指定的几个格子造成伤害
            //int playerIndex = GetMapGridIndex(currPerformer.currMapGrid);

            while (step > 0) {
                if (ToRight) {
                    // 向右遍历 造成伤害
                    if (performerIndex + step >= 0 && performerIndex + step < mapGrids.Count - 1) {
                        mapGrids[performerIndex + step].AttackOnGridChar(damage, performer);
                    }
                } else {
                    if (performerIndex - step >= 0 && performerIndex - step < mapGrids.Count - 1) {
                        mapGrids[performerIndex - step].AttackOnGridChar(damage, performer);
                    }
                }
                step--;
            }
            //RoundOver();
        }

        public void CauseDamageInGrid(int damage, int left, int right, BaseController currPerformer, bool ToRight) {

            // left 和 right ，为mapgrids的边界
            if (left < 0) {
                left = 0;
            }

            if (right > mapGrids.Count) {
                right = mapGrids.Count - 1;
            }

            if (left > right) {
                return;
            }

            while (left <= right) {

                // TODO： 要获取玩家的伤害数值 对敌人造成伤害

                // 默认是玩家造成的伤害
                mapGrids[left].AttackOnGridChar(damage, currPerformer, ToRight, false);
                left++;
            }

            //RoundOver();
        }
        
        public void CauseDamageInAllGrid(int damage, BaseController curPerformer, bool ToRight) {
            foreach (var grid in mapGrids)
            {
                if (grid.existEnemyInGrid) {
                    grid.AttackOnGridChar(damage, curPerformer);
                }
            }
        }
        
        /// <summary>
        /// 恢复一格格子上单位的生命值
        /// </summary>
        public void RecoverHPInGrid(int recoverHP, MapGrid mapGrid) {
            if(mapGrid.curCharInGrid != null) {
                mapGrid.curCharInGrid.Recover(0.1f, -recoverHP);
            }
        }
        public void RecoverHPInScope(int recoverHP, MapGrid startGrid, int scope, bool ToRight) {
            int index = GetMapGridIndex(startGrid);
            if (ToRight) {
                while(scope >= 0) {
                    scope--;
                    if (index + scope > mapGrids.Count - 1 || index + scope < 0) {
                        return;
                    }
                    RecoverHPInGrid(recoverHP, mapGrids[index + scope]);
                }
            } else {
                while (scope >= 0) {
                    scope--;
                    if (index - scope > mapGrids.Count - 1 || index - scope < 0) {
                        return;
                    }
                    RecoverHPInGrid(recoverHP, mapGrids[index - scope]);
                }
            }
        }
        public void RecoverHPInAll(int recoverHP, bool recoverEnemy) {
            foreach (var grid in mapGrids)
            {
                if (grid.existEnemyInGrid && recoverEnemy) {
                    grid.curCharInGrid.Recover(0.1f, -recoverHP);
                } else if(grid.existPlayerInGrid && !recoverEnemy) {
                    grid.curCharInGrid.Recover(0.1f, -recoverHP);
                }
            }
        }

        /// <summary>
        /// 获取到当前场上生命值最低的敌人单位所在的格子
        /// </summary>
        public MapGrid GetGrid_CharHPTheLast() {
            MapGrid ans = null;
            int curHP = 1000;
            foreach (var grid in mapGrids)
            {
                if (grid.existEnemyInGrid) {
                    if(grid.curCharInGrid.baseData.currentHP < curHP) {
                        ans = grid;
                    }
                }
            }
            return ans;
        }

        
        #endregion

        #region 管理场上单位
        /// <summary>
        /// 在地图终点生成敌人
        /// </summary>
        public void GenerateEnemy() {
            // TODO: 引入更多种类的敌人
            //GenerateEnemy(EnemyPrefab);
            Debug.Log("功能已经不可以使用");
        }

        public void GenerateEnemy(EnemyType enemyType) {
            if(enemyType == EnemyType.AttackFront) {
                GenerateEnemy(AttackFrontPrefab);
            } else if (enemyType == EnemyType.AttackTarget) {
                GenerateEnemy(AttackTargetPrefab);
            } else if (enemyType == EnemyType.AttackScope) {
                GenerateEnemy(AttackScopePrefab);
            } else if (enemyType == EnemyType.BuffEnemy) {
                GenerateEnemy(BuffEnemyPrefab);
            } else if (enemyType == EnemyType.NoEnemy) {
                // NoEnemy 不执行生成
            } else {
                GenerateEnemy(AttackFrontPrefab);
            }
        }

        private void GenerateEnemy(GameObject EnemyPrefab) {
            MapGrid destinationGrid = GetRightEdgeGrid();

            int count = 3;
            // 如果最右边的格子已经满员 则向左找位置
            while (destinationGrid.existCharInGrid && count > 0) {
                int desIndex = GetMapGridIndex(destinationGrid);
                destinationGrid = GetMapGrid_Neighbor(desIndex, 1, false);
                if (destinationGrid.existCharInGrid) {
                    continue;
                }
                count--;
            }

            // 没有合适的格子
            if (destinationGrid == null || destinationGrid.existCharInGrid) {
                return;
            }

            // 生成敌人 物体 并且设置ui
            GameObject enemyObj = Instantiate(EnemyPrefab, null);
            EnemyController enemyController = enemyObj.GetComponent<EnemyController>();
            enemyController.transform.position = GetRightEdgeGrid().transform.position;
            // 移动偏移值 以在同一条水平线上
            enemyController.transform.position += new Vector3(0, enemyController.enemyOffset, 0);
            enemyController.SetCharUI(enemyController.GetNextStep());

            // 尝试添加
            if (destinationGrid.AddCharToGrid(enemyController, true)) {
                AddChar(enemyController);
            } else {
                ObjectPool.GetInstance().RecycleObject(enemyObj);
            }
        }

        public void ClearAllEnemy() {
            foreach (var grid in mapGrids) {
                ClearEnemy_ByGrid(grid);
            }
        }

        public void ClearEnemy_ByGrid(MapGrid mapGrid) {
            if (mapGrid.existEnemyInGrid) {
                mapGrid.curCharInGrid.Death();
                mapGrid.RemoveCharFromGrid();
            }
        }

        #endregion

        /// <summary>
        /// 唤醒滑动判定条
        /// </summary>
        public void InvokeSlideJudge(UnityAction<float> callback) {
            sliderToJudge.InitSliderJudge(callback);
            sliderToJudge.gameObject.SetActive(true);
        }
    
        public void InvokeDamageTargetChoose(BaseController currPerformer, float curDamage, UnityAction<int> callback) {
            InvokeGridHandle_ExistEnemy(currPerformer, curDamage, callback);
        }

    }
}