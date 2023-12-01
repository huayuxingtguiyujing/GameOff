using DG.Tweening;
using GameOff2023.Character;
using GameOff2023.GamePlay.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameOff2023.GamePlay.Map {
    public class MapGrid : MonoBehaviour {

        public MapController mapController;

        [Header("地图格类型")]
        public MapGridType mapGridType;
        
        public int gridIndex;

        [Header("ui组件")]
        [SerializeField] SpriteRenderer mapGridImage;
        [SerializeField] Button moveButton;
        [SerializeField] Button attackButton;
        [SerializeField] Button excuteButton;

        /// <summary>
        /// 该格子上是否存在单位
        /// </summary>
        public bool existCharInGrid { get => (curCharInGrid != null); }
        public bool existPlayerInGrid { get => (curCharInGrid != null) && (curCharInGrid is PlayerController); }
        public bool existEnemyInGrid { get => (curCharInGrid != null) && (curCharInGrid is EnemyController); }
        public BaseController curCharInGrid;
        /// <summary>
        /// 添加角色到格子上
        /// </summary>
        public bool AddCharToGrid(BaseController charCtrl, bool goRight) {
            if(charCtrl.currMapGrid != null) {
                charCtrl.currMapGrid.RemoveCharFromGrid();
            }
            
            if (curCharInGrid != null) {
                return false;
            }

            // 开启移动动画
            if (!goRight && charCtrl is PlayerController) {
                charCtrl.SetBacking();
            } else {
                charCtrl.SetMoving();
            }

            curCharInGrid = charCtrl;
            charCtrl.currMapGrid = this;
            //float moveTime = transform.position.x * 1.5f;
            Tweener tweener = charCtrl.transform.DOMoveX(transform.position.x, 1.0f);

            // 关闭移动动画
            tweener.OnComplete(() => { charCtrl.SetStop(); });

            return true;
        }
        public void RemoveCharFromGrid() {
            curCharInGrid = null;
        }
        public void AddDamageCharInGrid(int modify) {
            if (!existCharInGrid) return;
            curCharInGrid.SetSkillDamageModify(modify);
        }

        BaseController curPerformer;
        float curDamage;
        UnityAction<int> buttonCallback;
        /// <summary>
        /// 初始化配置mapgrid
        /// </summary>
        public void SetMapGrid(MapController mapController, int index, MapGridType mapGridType = MapGridType.Normal) {
            gridIndex = index;
            this.mapGridType = mapGridType;

            this.mapController = mapController;

            attackButton.onClick.AddListener(AttackButtonEvent);
            //moveButton.onClick.AddListener(MoveToGrid);
        }

        /// <summary>
        /// 唤醒操作ui
        /// </summary>
        public void InvokeGridHandle(BaseController curPerformer, float curDamage, UnityAction<int> buttonCallback) {
            if(curCharInGrid != null) {
                attackButton.gameObject.SetActive(true);
            }
            //moveButton.gameObject.SetActive(true);
            
            this.curDamage = curDamage;

            this.buttonCallback = buttonCallback;
            // 处决区域可以发动处决
            //if(mapGridType == MapGridType.Execute && existEnemyInGrid) {
            //    excuteButton.gameObject.SetActive(true);
            //}

            this.curPerformer = curPerformer;
        }

        public void CloseGridHandle() {
            attackButton.gameObject.SetActive(false);
            moveButton.gameObject.SetActive(false);
            excuteButton.gameObject.SetActive(false);

            this.curPerformer = null;
            this.buttonCallback = null;
        }

        #region 格子上的操作：移动、攻击、处决
        /// <summary>
        /// 移动到该格子上
        /// </summary>
        public void MoveToGrid() {
            if (curPerformer == null) {
                Debug.LogError("没有获取到当前的执行者！");
                return;
            }

            MoveToGrid(curPerformer, true);
        }
        public void MoveToGrid(BaseController rec, bool toRight) {
            curPerformer = rec;

            AddCharToGrid(curPerformer, toRight);

            //Debug.Log("you click the move, and char will move:" + gridIndex + "_");

            //ActionComplete();
            // 当前的按钮回调
            if (buttonCallback != null) {
                buttonCallback.Invoke(gridIndex);
            }
        }

        /// <summary>
        /// 攻击该格子上的敌人
        /// </summary>
        /*public void AttackOnGridChar() {
            if (curPerformer == null) {
                Debug.LogError("没有获取到当前的执行者！");
            }
            AttackOnGridChar((int)curDamage, curPerformer);
        }*/
        public void AttackOnGridChar(int damage, BaseController rec, bool pushChar = true, bool pushToLeft = true) {

            curPerformer = rec;
            Debug.Log(gridIndex.ToString() + "|攻击触发者: " + rec.name + ", 攻击接收者" + curCharInGrid + ", 伤害大小: " + damage.ToString());
            // TODO: 执行攻击逻辑
            if (rec is PlayerController) {
                if (curCharInGrid != null) {
                    curCharInGrid.Hurt(0.1f, 1.0f * damage);
                }
                /*// 推动角色向指定方向移动
                if (pushChar && curCharInGrid != null) {
                    if (pushToLeft) {
                        MapGrid pushDes = MapController.Instance.GetMapGrid_Neighbor(gridIndex, 1, false);
                        if (!pushDes.existCharInGrid) {

                            pushDes.AddCharToGrid(curCharInGrid);
                        }
                    } else {
                        MapGrid pushDes = MapController.Instance.GetMapGrid_Neighbor(gridIndex, 1, true);
                        if (!pushDes.existCharInGrid) {

                            pushDes.AddCharToGrid(curCharInGrid);
                        }
                    }
                }*/
            }
            else {
                // 是敌人 直接进行攻击
                if (curCharInGrid != null && !(curCharInGrid is EnemyController)) {
                    curCharInGrid.Hurt(0.1f, damage);
                }
            }
            
        }
        public void AttackButtonEvent() {
            // 当前的按钮回调
            if (buttonCallback != null) {
                buttonCallback.Invoke(gridIndex);
            }
        }

        /// <summary>
        /// 执行处决
        /// </summary>
        public void ExcuteOnGridChar() {
            if (mapGridType == MapGridType.Execute) {
                if (existEnemyInGrid) {
                    curCharInGrid.Death();
                }
            }

            // 当前的按钮回调
            if (buttonCallback != null) {
                buttonCallback.Invoke(gridIndex);
            }
        }
        #endregion

        /// <summary>
        /// 结束回合
        /// </summary>
        private void ActionComplete() {
            MapController.Instance.RoundOver();
        }

        /// <summary>
        /// 设置格子为红色格子
        /// </summary>
        public void SetAsRedGrid() {
            mapGridImage.color = Color.red;
            mapGridType = MapGridType.Execute;
        }
        public void SetAsNormalGrid() {
            mapGridImage.color = Color.white;
            mapGridType = MapGridType.Normal;
        }

    }

    public enum MapGridType {
        Normal,
        LeftEdge,
        RightEdge,
        Execute
    }
}