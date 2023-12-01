using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarGame_True.Utils {
    public class ObjectPool {

        #region 单例模式
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

        // 对象池:包含在对象池的物体是可复用的
        private Dictionary<string, List<ObjectPoolMark>> pool;

        // 对象池中最大存储的对象数目
        private int maxPoolNum = 10;

        /// <summary>
        /// 从对象池获取物体
        /// </summary>
        /// <remarks>
        /// 该函数用于代替Instantiate函数，使用函数时，输入的预制件不能重名
        /// </remarks>
        public GameObject GetObject(GameObject prefabObject, Transform parentTransform, Vector3 position, Quaternion quaternion) {
            string prefabName = prefabObject.name;
            GameObject result = null;

            if (pool.ContainsKey(prefabName)) {
                if (pool[prefabName] == null) {
                    pool[prefabName] = new List<ObjectPoolMark>();
                } else if (pool[prefabName].Count > 0) {
                    // 对象池中含有该对象的物体，则 取出 并且复用
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
                    //Debug.Log("复用对象:" + prefabName + "_" + pool[prefabName].Count);
                    return result;
                }
            } else {
                pool.Add(prefabName, new List<ObjectPoolMark>());
            }

            //Debug.Log("对象池添加:" + prefabName + "_" + pool[prefabName].Count);

            result = GameObject.Instantiate(prefabObject, parentTransform);
            result.transform.localPosition = position;
            result.transform.localRotation = quaternion;

            // 为生成的游戏物体挂接上标记脚本，设置标记脚本的prefabName
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
        /// 回收物体
        /// </summary>
        /// <remarks>
        /// 该函数用于代替Destroy函数
        /// </remarks>
        public void RecycleObject(GameObject obj) {
            if (obj.GetComponent<ObjectPoolMark>() == null) {
                // 未挂接对象池标记
                GameObject.Destroy(obj);
                return;
            }

            obj.SetActive(false);
            ObjectPoolMark objectPoolMark = obj.GetComponent<ObjectPoolMark>();

            if (pool.ContainsKey(objectPoolMark.PrefabName)) {
                if (pool[objectPoolMark.PrefabName].Count > maxPoolNum) {
                    // 线程池里物体太多，直接销毁
                    //Debug.Log("物体太多，直接销毁");

                    GameObject.Destroy(obj);
                } else {
                    // 回收该物体到线程池中
                    pool[objectPoolMark.PrefabName].Add(objectPoolMark);

                    //Debug.Log("对象池回收:" + objectPoolMark.PrefabName + "_" + pool[objectPoolMark.PrefabName].Count);
                }
            } else {
                pool.Add(objectPoolMark.PrefabName, new List<ObjectPoolMark>() { objectPoolMark });

                //Debug.Log("对象池回收:" + objectPoolMark.PrefabName + "_" + pool[objectPoolMark.PrefabName].Count);
            }
        }

        public void RecycleObject_Origin(GameObject obj) {
            GameObject.Destroy(obj);
        }

    }
}