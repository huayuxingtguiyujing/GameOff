
using GameOff2023.Character;
using UnityEngine;
using WarGame_True.Utils;

public class DamageArea : MonoBehaviour
{
    [Header("�Զ���ɢʱ��")]
    [SerializeField] float fadeTime = 5.0f;
    private float fadeRecTime = 5.0f;

    [Header("����˺����")]
    [SerializeField] float damageTime = 2.0f;
    private float damageRecTime = 2.0f;

    [Header("�˺���ֵ")]
    [SerializeField] float causeDamage;

    [Header("Ŀ��㼶")]
    [SerializeField] LayerMask targetLayer;
    private int targetLayerIndex;

    public void InitDamageArea() {
        targetLayer = LayerMask.NameToLayer("Player");
        fadeRecTime = fadeTime;
    }

    public void Update() {

        fadeRecTime -= Time.deltaTime;
        if (fadeRecTime <= 0) {
            //fadeRecTime = fadeTime;
            ObjectPool.GetInstance().RecycleObject(gameObject);
        }
    }

    private void OnTriggerStay(Collider other) {
        TriggerStay(other);
    }


    protected bool RecoverCauseDamage() {
        damageRecTime -= Time.deltaTime;
        if (damageRecTime < -10.0f) {
            damageRecTime = -1.0f;
        }

        if (damageRecTime > 0) {
            return false;
        } else {
            return true;
        }
    }

    protected virtual void TriggerStay(Collider collision) {

        if (!RecoverCauseDamage()) {
            return;
        }

        //Debug.Log("��⵽������:" + collision.gameObject.name + "_" + collision.gameObject.layer + "_" + targetLayer.value);

        if (collision.gameObject.tag == "Player"
            && targetLayer.value == collision.gameObject.layer ) {
            BaseController baseController = collision.gameObject.GetComponent<BaseController>();
            if (baseController != null) {
                baseController.Hurt(0.1f, causeDamage);
                Debug.Log("����˳����˺���:" + collision.gameObject.name);
                damageRecTime = damageTime;
            }
        }

    }

}
