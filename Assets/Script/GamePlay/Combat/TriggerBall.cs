
using GameOff2023.Character;
using UnityEngine;
using WarGame_True.Utils;

namespace GameOff2023.GamePlay {
    /// <summary>
    /// 所有法球（弹幕）物体的的基类
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriggerBall : MonoBehaviour{

        public BallType ballType = BallType.Normal;

        [Header("自动消散时间")]
        [SerializeField] float fadeTime = 5.0f;
        private float fadeRecTime = 5.0f;

        [Header("射速")]
        [SerializeField] float shotInitSpeed = 5.0f;

        [Header("造成的伤害")]
        public float causeDamage = 1;

        [Header("LowSpeed")]
        public float lowSpeedTime = 3.0f;
        public float lowSpeedModify = 0.2f;
        [Header("Dizzy")]
        public float dizzyTime = 2.0f;
        [Header("ContinuousDamage")]
        public float damageTime = 3.0f;
        public float continuousDamage = 3.0f;
        [Header("Explosion")]
        public float explosionDetectionRadius = 3.0f;

        private LayerMask targetLayer;

        public void InitTriggerBall(LayerMask targetLayer, Vector3 shotSpeed, float damage) {
            this.targetLayer = targetLayer;
            
            Rigidbody rigidbody = GetComponentInChildren<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.velocity = shotSpeed * shotInitSpeed;

            causeDamage += damage;

            fadeRecTime = fadeTime;
        }

        public void Update() {

            fadeRecTime -= Time.deltaTime;
            if (fadeRecTime <= 0) {
                //fadeRecTime = fadeTime;
                ObjectPool.GetInstance().RecycleObject(gameObject);
            }
        }

        private void OnTriggerEnter(Collider collision) {
            OnTriggerStart(collision);
        }

        protected virtual void OnTriggerStart(Collider collision) {

            //Debug.Log("检测到了物体" + collision.gameObject.name + "_" + collision.gameObject.layer + "_" + (int)targetLayer);

            if ((targetLayer.value & 1 << collision.gameObject.layer) > 0) {
                BaseController baseController = collision.gameObject.GetComponent<BaseController>();
                if (baseController != null) {
                    baseController.Hurt(0.1f, causeDamage);

                    if (ballType == BallType.LowSpeed) {
                        ////TODO : 实现减速, 需要
                        //BaseModify baseModify = new BaseModify(
                        //    lowSpeedTime, -lowSpeedModify, 0, 999, false
                        //);
                        //baseController.AddModify(baseModify);
                    }

                    if (ballType == BallType.Dizzy) {
                        ////TODO : 实现眩晕, 需要
                        //BaseModify baseModify = new BaseModify(
                        //    dizzyTime, 0, 0, 999, true
                        //);
                        //baseController.AddModify(baseModify);
                    }

                    if(ballType == BallType.ContinuousDamage) {
                        //BaseModify baseModify = new BaseModify(
                        //    damageTime, 0, continuousDamage, 1.0f, false
                        //);
                        //baseController.AddModify(baseModify);
                    }

                    if(ballType == BallType.Explosion) {
                        Explosion(explosionDetectionRadius, targetLayer);
                    }

                    if(ballType != BallType.Penetrate) {
                        ObjectPool.GetInstance().RecycleObject(gameObject);
                    }
                }
                
                //Debug.Log("攻击到了！:" + collision.gameObject.name);
            }
        }

        public void Explosion(float detectionRadius, LayerMask targetLayer) {

            // 用一个球体投射 获取可能的物体
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

            foreach (Collider collider in colliders) {

                //物体与爆炸中心的距离(越近的受到的爆炸力越大)
                float distance = Mathf.Abs(Vector2.Distance(transform.position, collider.gameObject.transform.position));
                Vector3 mid = transform.position;
                Vector3 dir = - (mid - collider.gameObject.transform.position).normalized;
                //rec.GetComponent<Rigidbody2D>().gravityScale = 1;

                Rigidbody rigidBody = collider.gameObject.GetComponent<Rigidbody>();
                if (rigidBody != null) {
                    rigidBody.AddForce(dir * 3.0f / distance);
                }
               
            }

            //Debug.Log("爆炸！");
        }


    }

    /// <summary>
    /// 不同的子弹类型
    /// </summary>
    public enum BallType {
        Normal,
        Penetrate,
        LowSpeed,
        Dizzy,
        ContinuousDamage,
        Explosion
    }
}