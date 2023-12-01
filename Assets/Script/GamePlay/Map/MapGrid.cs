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

        [Header("��ͼ������")]
        public MapGridType mapGridType;
        
        public int gridIndex;

        [Header("ui���")]
        [SerializeField] SpriteRenderer mapGridImage;
        [SerializeField] Button moveButton;
        [SerializeField] Button attackButton;
        [SerializeField] Button excuteButton;

        /// <summary>
        /// �ø������Ƿ���ڵ�λ
        /// </summary>
        public bool existCharInGrid { get => (curCharInGrid != null); }
        public bool existPlayerInGrid { get => (curCharInGrid != null) && (curCharInGrid is PlayerController); }
        public bool existEnemyInGrid { get => (curCharInGrid != null) && (curCharInGrid is EnemyController); }
        public BaseController curCharInGrid;
        /// <summary>
        /// ��ӽ�ɫ��������
        /// </summary>
        public bool AddCharToGrid(BaseController charCtrl, bool goRight) {
            if(charCtrl.currMapGrid != null) {
                charCtrl.currMapGrid.RemoveCharFromGrid();
            }
            
            if (curCharInGrid != null) {
                return false;
            }

            // �����ƶ�����
            if (!goRight && charCtrl is PlayerController) {
                charCtrl.SetBacking();
            } else {
                charCtrl.SetMoving();
            }

            curCharInGrid = charCtrl;
            charCtrl.currMapGrid = this;
            //float moveTime = transform.position.x * 1.5f;
            Tweener tweener = charCtrl.transform.DOMoveX(transform.position.x, 1.0f);

            // �ر��ƶ�����
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
        /// ��ʼ������mapgrid
        /// </summary>
        public void SetMapGrid(MapController mapController, int index, MapGridType mapGridType = MapGridType.Normal) {
            gridIndex = index;
            this.mapGridType = mapGridType;

            this.mapController = mapController;

            attackButton.onClick.AddListener(AttackButtonEvent);
            //moveButton.onClick.AddListener(MoveToGrid);
        }

        /// <summary>
        /// ���Ѳ���ui
        /// </summary>
        public void InvokeGridHandle(BaseController curPerformer, float curDamage, UnityAction<int> buttonCallback) {
            if(curCharInGrid != null) {
                attackButton.gameObject.SetActive(true);
            }
            //moveButton.gameObject.SetActive(true);
            
            this.curDamage = curDamage;

            this.buttonCallback = buttonCallback;
            // ����������Է�������
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

        #region �����ϵĲ������ƶ�������������
        /// <summary>
        /// �ƶ����ø�����
        /// </summary>
        public void MoveToGrid() {
            if (curPerformer == null) {
                Debug.LogError("û�л�ȡ����ǰ��ִ���ߣ�");
                return;
            }

            MoveToGrid(curPerformer, true);
        }
        public void MoveToGrid(BaseController rec, bool toRight) {
            curPerformer = rec;

            AddCharToGrid(curPerformer, toRight);

            //Debug.Log("you click the move, and char will move:" + gridIndex + "_");

            //ActionComplete();
            // ��ǰ�İ�ť�ص�
            if (buttonCallback != null) {
                buttonCallback.Invoke(gridIndex);
            }
        }

        /// <summary>
        /// �����ø����ϵĵ���
        /// </summary>
        /*public void AttackOnGridChar() {
            if (curPerformer == null) {
                Debug.LogError("û�л�ȡ����ǰ��ִ���ߣ�");
            }
            AttackOnGridChar((int)curDamage, curPerformer);
        }*/
        public void AttackOnGridChar(int damage, BaseController rec, bool pushChar = true, bool pushToLeft = true) {

            curPerformer = rec;
            Debug.Log(gridIndex.ToString() + "|����������: " + rec.name + ", ����������" + curCharInGrid + ", �˺���С: " + damage.ToString());
            // TODO: ִ�й����߼�
            if (rec is PlayerController) {
                if (curCharInGrid != null) {
                    curCharInGrid.Hurt(0.1f, 1.0f * damage);
                }
                /*// �ƶ���ɫ��ָ�������ƶ�
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
                // �ǵ��� ֱ�ӽ��й���
                if (curCharInGrid != null && !(curCharInGrid is EnemyController)) {
                    curCharInGrid.Hurt(0.1f, damage);
                }
            }
            
        }
        public void AttackButtonEvent() {
            // ��ǰ�İ�ť�ص�
            if (buttonCallback != null) {
                buttonCallback.Invoke(gridIndex);
            }
        }

        /// <summary>
        /// ִ�д���
        /// </summary>
        public void ExcuteOnGridChar() {
            if (mapGridType == MapGridType.Execute) {
                if (existEnemyInGrid) {
                    curCharInGrid.Death();
                }
            }

            // ��ǰ�İ�ť�ص�
            if (buttonCallback != null) {
                buttonCallback.Invoke(gridIndex);
            }
        }
        #endregion

        /// <summary>
        /// �����غ�
        /// </summary>
        private void ActionComplete() {
            MapController.Instance.RoundOver();
        }

        /// <summary>
        /// ���ø���Ϊ��ɫ����
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