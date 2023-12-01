using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarGame_True.Utils {
    public class ObjectPool {

        #region ����ģʽ
        private static ObjectPool instance;
        public static ObjectPool GetInstance() {
            if(instance == null) {
                instance = new ObjectPool();
            }
            return instance;
        }
        public ObjectPool() {
            pool = new Dictionary<string, List<ObjectPoolMark>>();
        }
        #endregion

        // �����:�����ڶ���ص������ǿɸ��õ�
        private Dictionary<string, List<ObjectPoolMark>> pool;

        // ����������洢�Ķ�����Ŀ
        private int maxPoolNum = 10;

        /// <summary>
        /// �Ӷ���ػ�ȡ����
        /// </summary>
        /// <remarks>
        /// �ú������ڴ���Instantiate������ʹ�ú���ʱ�������Ԥ�Ƽ���������
        /// </remarks>
        public GameObject GetObject(GameObject prefabObject, Transform parentTransform, Vector3 position, Quaternion quaternion) {
            string prefabName = prefabObject.name;
            GameObject result = null;

            if (pool.ContainsKey(prefabName)) {
                if (pool[prefabName] == null) {
                    pool[prefabName] = new List<ObjectPoolMark>();
                } else if (pool[prefabName].Count > 0) {
                    // ������к��иö�������壬�� ȡ�� ���Ҹ���
                    int i = 0;
                    ObjectPoolMark objectPoolMark = pool[prefabName][i];
                    while (objectPoolMark == null) {
                        i ++;
                        objectPoolMark = pool[prefabName][i];
                    }
                    pool[prefabName].Remove(objectPoolMark);

                    result = objectPoolMark.gameObject;
                    result.SetActive(true);
                    result.transform.parent = parentTransform;
                    result.transform.localPosition = position;
                    result.transform.localRotation = quaternion;
                    //Debug.Log("���ö���:" + prefabName + "_" + pool[prefabName].Count);
                    return result;
                }
            } else {
                pool.Add(prefabName, new List<ObjectPoolMark>());
            }

            //Debug.Log("��������:" + prefabName + "_" + pool[prefabName].Count);

            result = GameObject.Instantiate(prefabObject, parentTransform);
            result.transform.localPosition = position;
            result.transform.localRotation = quaternion;

            // Ϊ���ɵ���Ϸ����ҽ��ϱ�ǽű������ñ�ǽű���prefabName
            result.AddComponent<ObjectPoolMark>();
            result.GetComponent<ObjectPoolMark>().PrefabName = prefabName;
            //result.SetActive(true);

            return result;
        }

        public GameObject GetObject(GameObject prefabObject, Transform parentTransform) {
            return GetObject(prefabObject, parentTransform, Vector3.zero, Quaternion.identity);
        }

        public GameObject GetObject(GameObject prefabObject) {
            return GetObject(prefabObject, null, Vector3.zero, Quaternion.identity);
        }

        public GameObject GetObject_Origin(GameObject prefabObject, Transform parentTransform, Vector3 position, Quaternion quaternion) {
            return GameObject.Instantiate(prefabObject, position, quaternion, parentTransform);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <remarks>
        /// �ú������ڴ���Destroy����
        /// </remarks>
        public void RecycleObject(GameObject obj) {
            if (obj.GetComponent<ObjectPoolMark>() == null) {
                // δ�ҽӶ���ر��
                GameObject.Destroy(obj);
                return;
            }

            obj.SetActive(false);
            ObjectPoolMark objectPoolMark = obj.GetComponent<ObjectPoolMark>();

            if (pool.ContainsKey(objectPoolMark.PrefabName)) {
                if (pool[objectPoolMark.PrefabName].Count > maxPoolNum) {
                    // �̳߳�������̫�ֱ࣬������
                    //Debug.Log("����̫�ֱ࣬������");

                    GameObject.Destroy(obj);
                } else {
                    // ���ո����嵽�̳߳���
                    pool[objectPoolMark.PrefabName].Add(objectPoolMark);

                    //Debug.Log("����ػ���:" + objectPoolMark.PrefabName + "_" + pool[objectPoolMark.PrefabName].Count);
                }
            } else {
                pool.Add(objectPoolMark.PrefabName, new List<ObjectPoolMark>() { objectPoolMark });

                //Debug.Log("����ػ���:" + objectPoolMark.PrefabName + "_" + pool[objectPoolMark.PrefabName].Count);
            }
        }

        public void RecycleObject_Origin(GameObject obj) {
            GameObject.Destroy(obj);
        }

    }
}