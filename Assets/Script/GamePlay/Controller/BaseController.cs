using GameOff2023.GamePlay;
using GameOff2023.GamePlay.Character;
using GameOff2023.GamePlay.Combat;
using GameOff2023.GamePlay.Map;
using GameOff2023.Infrastructure.Audio;
using GameOff2023.Utils.Achieve;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using WarGame_True.Utils;

namespace GameOff2023.Character
{
    //[RequireComponent(typeof(Animator))]
    //[RequireComponent(typeof(Rigidbody))]
    //[RequireComponent(typeof(Collider))]
    public class BaseController : MonoBehaviour, IDamagable {
        protected Animator _animator;
        protected Rigidbody _rigidbody;

        [Header("音频资源")]
        [SerializeField] string stepAudio;
        [SerializeField] string hurtAudio;
        [SerializeField] string deathAudio;
        [SerializeField] string normalAttackAudio;
        [SerializeField] string rangeAttackAudio;
        [SerializeField] string criticalAttackAudio;
        [SerializeField] string qteFailAudio;
        [SerializeField] string qteSucessAudio;

        [Header("运动参数")]
        public CharacterState characterState;
        [SerializeField] protected float moveSpeed = 7.0f;
        [SerializeField] protected float jumpForce = 5.0f;

        #region 其他控制参数
        protected bool IsInGround = true;
        protected bool IsTalking = false;
        private float hurtExtents = 0;

        [Header("攻击时间间隔"), SerializeField]
        protected float attackTime = 1.0f;
        protected float attackRecTime = 1.0f;

        [Header("冲刺时间间隔"), SerializeField]
        protected float shiftTime = 1.0f;
        protected float shiftRecTime = 1.0f;
        [SerializeField] protected float shiftForce = 3.0f;
        #endregion

        #region MonoBehaviour
        private void Awake() {
            InitController();

        }

        protected virtual void InitController() {
            //baseData = new BaseData(10,10);         // 需要自定义的话请在派生类里设置
            _animator = GetComponentInChildren<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update() {
            ExcuteInUpdate();

            // 管理modify
            //UpdateModify();
        }

        private void FixedUpdate() {
            ExcuteInFixedUpdate();
        }

        protected virtual void ExcuteInFixedUpdate() {

        }

        protected virtual void ExcuteInUpdate() {
            UpdateStateMachine();
        } 
        #endregion

        #region 人物可执行的行为
        protected void Move(Vector2 moveInput) {
            if (moveInput ==  null|| (moveInput.x == 0 && moveInput.y == 0)) {
                float rec = Mathf.Sqrt(moveInput.x * moveInput.x + moveInput.y * moveInput.y);
                _animator.SetFloat("WalkSpeed", Mathf.Abs(rec));
                return;
            }

            if (!TryTransformState(CharacterState.Walking)) {
                return;
            }

            //播放移动动画
            _animator.SetFloat("WalkSpeed", Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y));
            _animator.SetFloat("xInput",moveInput.x);
            _animator.SetFloat("yInput", moveInput.y);

            //确定移动速度
            float speed = GetMoveSpeed();

            transform.Translate(
                new Vector3(moveInput.x * speed * Time.deltaTime, 0,
                moveInput.y * speed * Time.deltaTime), Space.World
            );
        }

        protected void ChangeMoveTowards(float direction) {

            // 根据传入的移动输入 改变移动的方向
            if (direction > 0)
                transform.localScale = new Vector3(
                    -Mathf.Abs(transform.localScale.x),
                    transform.localScale.y, transform.localScale.z
                );
            else if (direction < 0)
                transform.localScale = new Vector3(
                    Mathf.Abs(transform.localScale.x),
                    transform.localScale.y, transform.localScale.z
                );
        }

        protected void SetTransformTowards(Vector3 faceTowards) {
            Vector3 targetDir = faceTowards - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            transform.rotation = targetRotation;

            //Quaternion targetRotation = Quaternion.LookRotation(faceTowards);
            //// 应用旋转到物体的 Transform
            //transform.rotation = targetRotation;
        }

        private float GetMoveSpeed() {
            float speed = moveSpeed;
            switch (characterState) {
                case CharacterState.Jumping:
                    speed *= 0.8f;
                    break;
                case CharacterState.Attacking:
                    speed *= 0.5f;
                    break;
                case CharacterState.Defensing:
                    speed *= 0.1f;
                    break;
            }
            return speed;
        }

        protected void CheckInGround() {
            LayerMask ignoreLayer = 1 << LayerMask.NameToLayer("Default")
                | 1 << LayerMask.NameToLayer("Characters");
            //
            RaycastHit2D raycastHit = Physics2D.Linecast(
                transform.position, 
                new Vector2(transform.position.x, transform.position.y - 0.1f),
                ignoreLayer
            );

            if (raycastHit) {
                // && raycastHit.collider.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast")
                //Debug.Log( raycastHit.collider.name);
                IsInGround = true;
            } else {
                IsInGround = false;
            }

            _animator.SetBool("IsInGround", IsInGround);
        }

        public virtual void Hurt(float addExtent, float damage) {
            if (!TryTransformState(CharacterState.Hurt)) {
                return;
            }

            //读取animator中的hurtExtents值 并更新之
            hurtExtents = _animator.GetFloat("HurtExtents");
            hurtExtents += addExtent;
            _animator.SetTrigger("HurtFlag");
            _animator.SetFloat("HurtExtents", hurtExtents);

            Debug.Log(name + " get hurted, damage: " + damage);
            //baseData.ModifyHP(-damage);
            //_animator.SetFloat("HP", 1 - baseData.currentHP/baseData.MaxHP);
            //if (baseData.IsDead()) {
            //    Death();
            //}

            AudioManager.PlayAudio(hurtAudio);
        }

        public virtual void Recover(float addExtent, float recover) {

        }

        public virtual void Death() {
            if (!TryTransformState(CharacterState.Death)) {
                return;
            }
            _animator.SetTrigger("Death");
            //_animator.Play("Death");
            _animator.CrossFade("Death", 0.1f);

            AudioManager.PlayAudio(deathAudio);
        }
        
        #endregion
        
        #region 角色 状态机
        protected void UpdateStateMachine() {
            if (characterState == CharacterState.Forbidden) {
                return;
            }

            AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(0);
            if (currentState.IsName("Idle")) {
                characterState = CharacterState.Idle;
            } else if (currentState.IsName("Walk")) {
                characterState = CharacterState.Walking;
            } else if (currentState.IsName("Attack")) {
                characterState = CharacterState.Attacking;
            } else if (currentState.IsName("Jump")) { 
                characterState = CharacterState.Jumping;
            } else if (currentState.IsName("Defense")) {
                characterState = CharacterState.Defensing;
            } else if (currentState.IsName("Hurt") 
                    || currentState.IsName("Recover")) {
                characterState = CharacterState.Hurt;
            } else if (currentState.IsName("Death")) {
                characterState = CharacterState.Death;
            }

        }

        protected bool TryTransformState(CharacterState targetState) {
            bool rec = true;
            if (characterState == CharacterState.Attacking) {
                //攻击时无法攻击、防御、跳跃
                rec = (targetState != CharacterState.Defensing)
                    && (targetState != CharacterState.Attacking)
                    && (targetState != CharacterState.Jumping);
                return rec;
            } else if (characterState == CharacterState.Defensing) {
                //防御时无法攻击、防御、跳跃
                rec = (targetState != CharacterState.Defensing)
                    && (targetState != CharacterState.Attacking)
                    && (targetState != CharacterState.Jumping);
                return rec;
            } else if (characterState == CharacterState.Jumping) {
                rec = targetState != CharacterState.Jumping;
            } else if (characterState == CharacterState.Hurt
            || characterState == CharacterState.Death
            || characterState == CharacterState.Forbidden) {
                //禁止其他一切行动
                return false;
            }

            return rec;
        } 
        
        public void SetForbidenState(bool flag) {
            if (flag) {
                characterState = CharacterState.Forbidden;
            } else {
                characterState = CharacterState.Idle;
            }
        }

        public void SetTalking(bool flag) {
            SetForbidenState(flag);
            IsTalking = flag;
        }
        #endregion


        #region 回合制行动

        [Header("回合 网格、行动指示器")]
        public MapGrid currMapGrid;
        [SerializeField] GameObject actionIndicator;

        [Header("下一次的移动步数")]
        public int nextStepNum = 1;

        public int skillDamageModify = 0;

        [Header("攻击范围")]
        public int attackDetectScope = 4;

        [Header("技能栏")]
        public List<BaseSkill> baseSkills;
        public BaseSkill GetUseSkill() {
            return baseSkills[0];
        }
        /// <summary>
        /// 获得技能的攻击范围
        /// </summary>
        public int GetNormalAttackScope() {
            return attackDetectScope;
        }
        public int GetRemoteAttackScope() {
            return 5;
        }

        public int GetSkillDamageModify() {
            return skillDamageModify;
        }
        public void SetSkillDamageModify(int modify) {
            skillDamageModify = modify;
        }

        [Header("角色数值")]
        public BaseData baseData;
        
        public virtual int GetNextStep() { 
            return 1; 
        }

        public virtual void SetCharUI(int nextStep) {
            this.nextStepNum = nextStep;
            if(GetComponentInChildren<Canvas>().renderMode == RenderMode.WorldSpace) {
                GetComponentInChildren<Canvas>().worldCamera = Camera.main;
            }
        }

        /// <summary>
        /// 设定该char为当前的回合执行者
        /// </summary>
        public void SetCurrentActor() {
            actionIndicator.SetActive(true);
        }

        public void SetNormal() {
            actionIndicator.SetActive(false);
        }
        
        /// <summary>
        /// 执行对应的动画，接口
        /// </summary>
        public virtual void SetMoving() {
            _animator.SetBool("MoveFlag", true);
            //Debug.Log("animtor: " + (_animator == null) + ", move start");
            AudioManager.PlayAudio(stepAudio);
        }

        public virtual void SetBacking() {
            _animator.SetBool("BackFlag", true);
            //Debug.Log("animtor: " + (_animator == null) + ", move start");
        }

        public virtual void SetStop() {
            _animator.SetBool("MoveFlag", false);
            _animator.SetBool("BackFlag", false);
            AudioManager.StopAudio(stepAudio);
        }

        public virtual void SetAttaking(float damageRatio ) {
            if (damageRatio > 1) {
                AudioManager.PlayAudio(criticalAttackAudio);
            } else {
                AudioManager.PlayAudio(normalAttackAudio);
            } 
            _animator.SetTrigger("AttackFlag01");
        }

        public virtual void SetAttacking2() {
            _animator.SetTrigger("AttackFlag02");
        }
        #endregion

    }


    public enum CharacterState {
        None,
        Idle,
        Walking,
        Attacking,
        Jumping,
        Defensing,
        Hurt,
        Death,
        Forbidden
    }
}
