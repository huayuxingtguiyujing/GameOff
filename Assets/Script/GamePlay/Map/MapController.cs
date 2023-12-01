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

        #region ��ͼ���� ��ر���

        [Header("��ɫ����")]
        [SerializeField] MapGrid redMapGrid;

        [Header("���ɵ�ͼ����")]
        //[SerializeField] float mapGridInterval;
        [SerializeField] int mapGridNum = 10;
        [SerializeField] Transform mapGridParent;
        [SerializeField] GameObject mapGridPrefab;

        public List<MapGrid> mapGrids = new List<MapGrid>();
        #endregion

        #region ��Ϸ�غ� ��ر���
        // ��ǰ���ж���
        public BaseController currPerformer;
        public bool IsPlayerPerformer { get => (currPerformer != null) && (currPerformer is PlayerController); }
        // �ж�����
        public Queue<BaseController> charActionQueue = new Queue<BaseController>();

        private int currPerformerIndex = 0;
        private int roundNum = 1;

        #endregion

        #region ���ϵ�λ ��ر���
        [Header("��ǰ���")]
        public PlayerController playerController;

        [Header("���˵�prefab")]
        [SerializeField] GameObject EnemyPrefab;
        [SerializeField] GameObject AttackFrontPrefab;
        [SerializeField] GameObject AttackTargetPrefab;
        [SerializeField] GameObject AttackScopePrefab;
        [SerializeField] GameObject BuffEnemyPrefab;

        [Header("�ؿ���������Ϣ")]
        [SerializeField] List<MapSetting> mapSettingList;
        public MapSetting curMapSetting;

        [Header("�����ж���")]
        [SerializeField] SliderToJudge sliderToJudge;

        #endregion

        private void Start() {
            Instance = this;

            // ��ʼ�� �ؿ���Ϣ���ӵ�һ�ؿ�ʼ
            mapSettingList = MapSetting.GetMapSettingByLevel(1);

            MapStart();
        }

        public void CreateMapGrids() {
            mapGridParent.gameObject.ClearObjChildren();

            mapGrids = new List<MapGrid>();
            float mapGridInterval = mapGridPrefab.GetComponentInChildren<SpriteRenderer>().size.x / 2 + 1;

            // ����Ԥ�ȸ�����grid��ࡢgrid��Ŀ��ͼ
            float offset = 0;
            for (int i = 0; i < mapGridNum; i++) {
                GameObject mapGridObj = Instantiate(mapGridPrefab, mapGridParent);
                MapGrid mapGrid = mapGridObj.GetComponent<MapGrid>();

                if (i == 0) {
                    // �趨 ���Ϊ���������
                    mapGrid.SetMapGrid(this, 0, MapGridType.LeftEdge);
                } else if (i == mapGridNum - 1) {
                    // �趨��ֹ��Ϊ ����������
                    mapGrid.SetMapGrid(this, mapGridNum - 1, MapGridType.RightEdge);
                } else {
                    mapGrid.SetMapGrid(this, i, MapGridType.Normal);
                }

                mapGrids.Add(mapGrid);
                
                mapGridObj.transform.position += new Vector3(offset,0,0);
                offset += mapGridInterval;
            }



        }

        #region �غϹ����ؿ�����

        /// <summary>
        /// �ж��Ƿ����е��ж��߶��Ѿ���ȡ���ж�
        /// </summary>
        public bool PerformerActionOver() {
            int allnum = charActionQueue.Count;
            if (allnum < 1) allnum = 1;
            if (roundNum % allnum == 0) {
                Debug.Log("һ����غ��Ѿ������ˣ����е�λ���Ѿ��ж����");
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

            // ��ó��ӵĵ�λ��Ϊ��ǰ�ж���
            currPerformer = charActionQueue.Dequeue();
            while(currPerformer == null) {
                currPerformer = charActionQueue.Dequeue();
            }
            currPerformer.SetCurrentActor();
            currPerformerIndex++;

            // һ�����غ��ֽ���
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
            // ������һ���ؿ�
            GetNextLevel();

            // ������ʼ����
            List<EnemyType> enemyTypes = curMapSetting.GetEnemyWhenMapStart();
            foreach (var enemyType in enemyTypes) {
                GenerateEnemy(enemyType);
            }

            RoundStart();
        }

        private void InitMap() {

            loadingPanel.gameObject.SetActive(true);
            // ���ùؿ��ĳ�ʼֵ
            currPerformerIndex = 0;
            roundNum = 1;
            ClearAllEnemy();

            // ��ʼ������
            AddChar(playerController);
            //playerController.currMapGrid = GetLeftEdgeGrid();
            GetLeftEdgeGrid().AddCharToGrid(playerController, false);

            // ȡ���������в������
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

            // �ж��Ƿ������һ��������
            if (PerformerActionOver()) {
                int playerIndexToDestination = mapGrids.Count - GetPlayerGridIndex();
                Debug.Log("playerindex:" + playerIndexToDestination +  ",���ֿ�������: " + curMapSetting.ShouldGenerateNewEnemy(roundNum, playerIndexToDestination));
                // �ж��Ƿ�Ҫ���ɵ��˵�λ
                if (curMapSetting.ShouldGenerateNewEnemy(roundNum, playerIndexToDestination)) {
                GenerateEnemy(curMapSetting.GetNextGenerateEnemy());
                }
                roundNum++;
            }
            
            // ˢ��ui��ִ�����/���˵�ai
            if (currPerformer is PlayerController) {
                // ����� ������ҵ�ǰ��λ�� �������в������
                InvokeHUDHandle();
                //Debug.Log(" player's round start! " + "player can choose an action");
            } else {
                
                // ������Χ�� ����ҵľ���
                int attackScope = currPerformer.GetNormalAttackScope();
                int distance = GetMapGridIndex(currPerformer.currMapGrid) - GetPlayerGridIndex();

                MapGrid leftGrid = GetMapGrid_Neighbor(GetMapGridIndex(currPerformer.currMapGrid), 1, false);
                if (distance <= attackScope) {
                    // ����ڹ�����Χ�� ����й���
                    Debug.Log("��⵽��� ���й���!");
                    // ����ʹ�õļ��� �л���������
                    //currPerformer.SetAttaking(1);
                    UseSkill(currPerformer, currPerformer.GetUseSkill(), false, 1);

                } else if (leftGrid.mapGridType == MapGridType.Execute && CanPushRedGrid(false)) {
                    
                    // ǰ���Ǻ�ɫ���� �ҿ����ƽ� ���ƽ�֮
                    leftGrid.MoveToGrid(currPerformer, false);
                    MoveRedGrid(false);
                } else {
                    
                    List<MapGrid> pathGrid = GetMapGrids_ByStep(
                        GetMapGridIndex(currPerformer.currMapGrid),
                        currPerformer.nextStepNum, false
                    );
                    // �ƶ�����Զ�Ŀ��ƶ�����
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

            // ���һ�غ��ڿ��Խ��ж���ж�
            if (currPerformer is PlayerController) {
                playerActionNum--;
                hudPanel.UpdateRoundMes(
                    roundNum, currPerformer.name, RedMoveExtent, playerActionNum, curMapSetting.enemyNum, curMapSetting.levelName);

                if (playerActionNum > 0) {
                    // ����ʣ����ж���
                    return;
                }
            }
            
            hudPanel.UpdateRoundMes(
                roundNum, currPerformer.name, RedMoveExtent, playerActionNum, curMapSetting.enemyNum, curMapSetting.levelName);

            Debug.Log("һ���غ��Ѿ������ˣ�:" + roundNum);

            // �رղ������
            CloseHUDHandle();
            currPerformer.SetCharUI(currPerformer.GetNextStep());

            // TODO: �ж��Ƿ������˽����ؿ�����
            int playerIndex = GetPlayerGridIndex();
            if (playerIndex >= mapGrids.Count - 1) {
                // ����1: ��ҵ�ǰλ�ô������һ��
                MapOver();
            } else if (curMapSetting.enemyNum <= 0) {
                // ����2: ���е��˾�������
                MapOver();
            } else if (playerController.baseData.IsDead()) {
                // ����3: �������������fail���
                ApplicationController.Instance.PlayerFail(curMapSetting.levelName);
                return;
            } else {
                Invoke("RoundStart", 1.0f);
            }

        }

        public void MapOver() {

            Debug.Log("��ת����һ�أ�");

            InitMap();

            // TODO����ת����һ��
            GetNextLevel();
            if (curMapSetting != null) {
                // ��Ϸ������
                ApplicationController.Instance.GameOver();
            }

            // ������ʼ����
            List<EnemyType> enemyTypes = curMapSetting.GetEnemyWhenMapStart();
            foreach (var enemyType in enemyTypes) {
                GenerateEnemy(enemyType);
            }

            // ˢ��ui����������ͼ
            hudPanel.UpdateRoundMes(
                    roundNum, currPerformer.name, RedMoveExtent, playerActionNum, curMapSetting.enemyNum, curMapSetting.levelName);

            RoundStart();
        }

        #endregion

        #region ��ͼ���ӹ���

        /// <summary>
        /// �����Զ�Ŀ������ߵĸ���
        /// </summary>
        public MapGrid GetWalkableGrid(List<MapGrid> mapGrids, int curStep, bool IsPlayer = false) {
            
            MapGrid ans = null;
            int count = mapGrids.Count;
            for (int i = 0; i < count; i++) {
                if (i > curStep) {
                    break;
                } else {
                    // �����ߵ�����������
                    if (mapGrids[i].mapGridType == MapGridType.Execute) {
                        return ans;
                    }

                    if (IsPlayer) {
                        // ����� �����Ƿ��е����ڸ����� ����������Ϊ�ƶ�Ŀ��
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
        /// ͨ�������ͷ����ÿ����ߵĵ�ͼ����
        /// </summary>
        public List<MapGrid> GetMapGrids_ByStep(int index, int step = 1, bool toRight = true) {
            List<MapGrid> ans = new List<MapGrid>();
            if (toRight) {
                for (int i = 1; i <= step; i++) {
                    if (index + i >= mapGrids.Count || index + i < 0) {
                        continue;
                    } else {
                        // ���˳��Ϊ ��index������(����λ) ������(����λ)
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
                    // ���˳��Ϊ ��index������(����λ) ������(����λ)
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
        /// �ж�һ��grid���Ƿ��е���/���
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
        /// ����mapgrid��ȡ����mapgrid��index
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
        /// ����grid�Ĳ������
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
        /// ������Ҳ���HUD����
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
                // �����ƶ�
                RedMoveExtent = 0;
                MoveRedGrid(false);
            } else if (RedMoveExtent >= 6) {
                // �����ƶ�
                RedMoveExtent = 0;
                MoveRedGrid(true);
            }
        }
        /// <summary>
        /// �ƶ���ɫ��������
        /// </summary>
        public void MoveRedGrid(bool ToRight, int step = 1) {
            int redIndex = GetMapGridIndex(redMapGrid);
            MapGrid moveTarget = GetMapGrid_Neighbor(redIndex, step, ToRight);
            if (moveTarget != null) {
                redMapGrid.SetAsNormalGrid();
                redMapGrid = moveTarget;
                redMapGrid.SetAsRedGrid();
                // ����ɱ�������ϵĹ��������ƺ�һ��
                if (redMapGrid.existEnemyInGrid) {
                    redMapGrid.ExcuteOnGridChar();
                } else if(redMapGrid.existPlayerInGrid){
                    int redIndex_new = GetMapGridIndex(redMapGrid);
                    MapGrid pushDes = GetMapGrid_Neighbor(redIndex_new, 1, false);
                    if (pushDes == null) {
                        // �Ƶ�������� ��Ϸ����
                        //MapOver();
                    }else if (!pushDes.existCharInGrid) {
                        BaseController baseController = redMapGrid.curCharInGrid;
                        pushDes.AddCharToGrid(baseController, false);
                    }
                    Debug.Log("��ɫ����������ң������ƶ�֮");
                } else {
                    // ������� ��������
                }
            } else {
                // Խ������ ���Ƶ���� ����������Ƶ��ұ� ���Ӯ��
                if(redIndex == 0) {

                }else if(redIndex == mapGrids.Count - 1) {

                }
                Debug.LogError("�޷��ƶ����ӣ�");
            }
            Debug.Log("�ƶ��˺�ɫ���ӣ�");
        }
        public bool CanPushRedGrid(bool ToRight) {
            // ����ɫ���� ���/�ұ� ���޵�λ
            int redGridIndex = GetMapGridIndex(redMapGrid);
            MapGrid grid = GetMapGrid_Neighbor(redGridIndex, 1, ToRight);
            if (grid.existCharInGrid) {
                return false;
            } else {
                return true;
            }
        }
        // ʲôʱ����Ҫ�ƶ���ɫ���ӣ���ұ����������˱���������ҵ����ƶ�����

        #endregion

        #region ��λ�ɽ��еĲ���

        /// <summary>
        /// ��һ��ָ���Ľ�ɫ�ƶ������һ��
        /// </summary>
        public void MoveCharToLeft() {

            // �����˵�ǰ���ƶ���Ŀ

            if (!IsPlayerPerformer) return;
            int curPlayerIndex = GetMapGridIndex(currPerformer.currMapGrid);
            // ��ȡ��ߵĸ��� �ƶ����ø�����
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
                // �Ǻ�ɫ���� �Ҹ����ұ��е��˵�λ �����ߵ�����

            }else if (rightGrid.mapGridType == MapGridType.Execute && CanPushRedGrid(true)) {
                // �Ǻ�ɫ���� �Ҹ����ұ�û�е��˵�λ ���ƽ�֮
                rightGrid.MoveToGrid(currPerformer, true);
                MoveRedGrid(true);
            } else {
                // ������� ����������
                rightGrid.MoveToGrid(currPerformer, true);
            }

            RoundOver();
        }

        /// <summary>
        /// ʹ�ü���
        /// </summary>
        /// <param name="skill">Ҫִ�еļ���</param>
        /// <param name="ToRight">���ܵķ���</param>
        public void UseSkill(BaseController skillPerformer, BaseSkill skill, bool ToRight, int ratio) {
            
            Debug.Log(skillPerformer.name + "ʹ���˼���: " + skill.skillName + ",��������: " + ToRight);

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
        /// �����ѡ����ӣ�����ص���ִ�в���
        /// </summary>
        public void PlayerChooseGrid(UnityAction<MapGrid> callback) {

            // TODO: ��������mapgrid�������������ѡ��

            // TODO: ����ص�

        }

        /// <summary>
        /// ��ָ���ĸ���������˺�
        /// </summary>
        public void CauseDamageInGrid(int damage, int performerIndex, BaseController performer, bool ToRight, int step = 1) {
            //if (!IsPlayerPerformer) return;
            // ������ҵ�λ�ã���ָ���ļ�����������˺�
            //int playerIndex = GetMapGridIndex(currPerformer.currMapGrid);

            while (step > 0) {
                if (ToRight) {
                    // ���ұ��� ����˺�
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

            // left �� right ��Ϊmapgrids�ı߽�
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

                // TODO�� Ҫ��ȡ��ҵ��˺���ֵ �Ե�������˺�

                // Ĭ���������ɵ��˺�
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
        /// �ָ�һ������ϵ�λ������ֵ
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
        /// ��ȡ����ǰ��������ֵ��͵ĵ��˵�λ���ڵĸ���
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

        #region �����ϵ�λ
        /// <summary>
        /// �ڵ�ͼ�յ����ɵ���
        /// </summary>
        public void GenerateEnemy() {
            // TODO: �����������ĵ���
            //GenerateEnemy(EnemyPrefab);
            Debug.Log("�����Ѿ�������ʹ��");
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
                // NoEnemy ��ִ������
            } else {
                GenerateEnemy(AttackFrontPrefab);
            }
        }

        private void GenerateEnemy(GameObject EnemyPrefab) {
            MapGrid destinationGrid = GetRightEdgeGrid();

            int count = 3;
            // ������ұߵĸ����Ѿ���Ա ��������λ��
            while (destinationGrid.existCharInGrid && count > 0) {
                int desIndex = GetMapGridIndex(destinationGrid);
                destinationGrid = GetMapGrid_Neighbor(desIndex, 1, false);
                if (destinationGrid.existCharInGrid) {
                    continue;
                }
                count--;
            }

            // û�к��ʵĸ���
            if (destinationGrid == null || destinationGrid.existCharInGrid) {
                return;
            }

            // ���ɵ��� ���� ��������ui
            GameObject enemyObj = Instantiate(EnemyPrefab, null);
            EnemyController enemyController = enemyObj.GetComponent<EnemyController>();
            enemyController.transform.position = GetRightEdgeGrid().transform.position;
            // �ƶ�ƫ��ֵ ����ͬһ��ˮƽ����
            enemyController.transform.position += new Vector3(0, enemyController.enemyOffset, 0);
            enemyController.SetCharUI(enemyController.GetNextStep());

            // �������
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
        /// ���ѻ����ж���
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