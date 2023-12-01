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

        [Header("��Ƶ��Դ")]
        [SerializeField] string stepAudio;
        [SerializeField] string hurtAudio;
        [SerializeField] string deathAudio;
        [SerializeField] string normalAttackAudio;
        [SerializeField] string rangeAttackAudio;
        [SerializeField] string criticalAttackAudio;
        [SerializeField] string qteFailAudio;
        [SerializeField] string qteSucessAudio;

        [Header("�˶�����")]
        public CharacterState characterState;
        [SerializeField] protected float moveSpeed = 7.0f;
        [SerializeField] protected float jumpForce = 5.0f;

        #region �������Ʋ���
        protected bool IsInGround = true;
        protected bool IsTalking = false;
        private float hurtExtents = 0;

        [Header("����ʱ����"), SerializeField]
        protected float attackTime = 1.0f;
        protected float attackRecTime = 1.0f;

        [Header("���ʱ����"), SerializeField]
        protected float shiftTime = 1.0f;
        protected float shiftRecTime = 1.0f;
        [SerializeField] protected float shiftForce = 3.0f;
        #endregion

        #region MonoBehaviour
        private void Awake() {
            InitController();

        }

        protected virtual void InitController() {
            //baseData = new BaseData(10,10);         // ��Ҫ�Զ���Ļ�����������������
            _animator = GetComponentInChildren<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update() {
            ExcuteInUpdate();

            // ����modify
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

        #region �����ִ�е���Ϊ
        protected void Move(Vector2 moveInput) {
            if (moveInput ==  null|| (moveInput.x == 0 && moveInput.y == 0)) {
                float rec = Mathf.Sqrt(moveInput.x * moveInput.x + moveInput.y * moveInput.y);
                _animator.SetFloat("WalkSpeed", Mathf.Abs(rec));
                return;
            }

            if (!TryTransformState(CharacterState.Walking)) {
                return;
            }

            //�����ƶ�����
            _animator.SetFloat("WalkSpeed", Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y));
            _animator.SetFloat("xInput",moveInput.x);
            _animator.SetFloat("yInput", moveInput.y);

            //ȷ���ƶ��ٶ�
            float speed = GetMoveSpeed();

            transform.Translate(
                new Vector3(moveInput.x * speed * Time.deltaTime, 0,
                moveInput.y * speed * Time.deltaTime), Space.World
            );
        }

        protected void ChangeMoveTowards(float direction) {

            // ���ݴ�����ƶ����� �ı��ƶ��ķ���
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
            //// Ӧ����ת������� Transform
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

            //��ȡanimator�е�hurtExtentsֵ ������֮
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
        
        #region ��ɫ ״̬��
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
                //����ʱ�޷���������������Ծ
                rec = (targetState != CharacterState.Defensing)
                    && (targetState != CharacterState.Attacking)
                    && (targetState != CharacterState.Jumping);
                return rec;
            } else if (characterState == CharacterState.Defensing) {
                //����ʱ�޷���������������Ծ
                rec = (targetState != CharacterState.Defensing)
                    && (targetState != CharacterState.Attacking)
                    && (targetState != CharacterState.Jumping);
                return rec;
            } else if (characterState == CharacterState.Jumping) {
                rec = targetState != CharacterState.Jumping;
            } else if (characterState == CharacterState.Hurt
            || characterState == CharacterState.Death
            || characterState == CharacterState.Forbidden) {
                //��ֹ����һ���ж�
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


        #region �غ����ж�

        [Header("�غ� �����ж�ָʾ��")]
        public MapGrid currMapGrid;
        [SerializeField] GameObject actionIndicator;

        [Header("��һ�ε��ƶ�����")]
        public int nextStepNum = 1;

        public int skillDamageModify = 0;

        [Header("������Χ")]
        public int attackDetectScope = 4;

        [Header("������")]
        public List<BaseSkill> baseSkills;
        public BaseSkill GetUseSkill() {
            return baseSkills[0];
        }
        /// <summary>
        /// ��ü��ܵĹ�����Χ
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

        [Header("��ɫ��ֵ")]
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
        /// �趨��charΪ��ǰ�Ļغ�ִ����
        /// </summary>
        public void SetCurrentActor() {
            actionIndicator.SetActive(true);
        }

        public void SetNormal() {
            actionIndicator.SetActive(false);
        }
        
        /// <summary>
        /// ִ�ж�Ӧ�Ķ������ӿ�
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
