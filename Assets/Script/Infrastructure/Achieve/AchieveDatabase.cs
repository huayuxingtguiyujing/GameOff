
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;

namespace GameOff2023.Utils.Achieve {
    public class AchieveDatabase {
        private static AchieveDatabase instance;
        public static AchieveDatabase Instance { get => instance; private set => instance = value; }
        public static AchieveDatabase GetInstance() {
            if(Instance == null) {
                Instance = new AchieveDatabase();
            }
            return Instance;
        }

        public static string DatabasePath = Application.persistentDataPath + "/Achieve";


        #region 存档操作
        /// <summary>
        /// 根据存档名称获取当前存档
        /// </summary>
        public AchieveData LoadAchieve(string achieveName) {
            List<AchieveData> achieveDataList = LoadAllAchieve();
            return achieveDataList.Find(achieve => achieve.achieveName == achieveName);
        }

        public List<AchieveData> LoadAllAchieve() {
            List<AchieveData> achieveDataList = new List<AchieveData>();
            DirectoryInfo achieveDirectory = Directory.CreateDirectory(DatabasePath);
            if (!achieveDirectory.Exists) {
                Debug.LogError("存档文件夹不存在");
                return achieveDataList;
            }

            //读取所有存档文件 转化为存档数据
            foreach (FileInfo fileInfo in achieveDirectory.GetFiles())
            {
                AchieveData achieveData = ReadBinaryData<AchieveData>(fileInfo.FullName);
                achieveDataList.Add(achieveData);
            }

            return achieveDataList;
        }

        public PlayerData LoadBaseAchieve() {
            Debug.Log(DatabasePath);
            return new PlayerData();
        }

        /// <summary>
        /// 新建存档
        /// </summary>
        public void NewAchieve() {
            List<AchieveData> achieveDataList = LoadAllAchieve();
            string achieveName = "newAchieve" + achieveDataList.Count.ToString();

            //构建存档数据
            NewAchieve(achieveName);
        }

        public void NewAchieve(string achieveName) {
            PlayerData playerData = new PlayerData();
            AchieveData achieveData = new AchieveData(achieveName, playerData, DateTime.Now, DateTime.Now);

            SaveBinaryData(achieveData, DatabasePath + "/" + achieveName);
        }

        /// <summary>
        /// 保存存档
        /// </summary>
        public void SaveAchieve(AchieveData achieveData) {
            SaveBinaryData(achieveData, DatabasePath + "/" + achieveData.achieveName, FileMode.Open);
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public void DeleteAchieve(string achieveName) {
            DeleteFile(DatabasePath + "/" + achieveName);
        }
        #endregion

        /// <summary>
        /// 序列化保存对象
        /// </summary>
        /// <typeparam name="T"> 可序列化的类名 </typeparam>
        /// <param name="dataClass"> 可序列化的对象 </param>
        /// <param name="path"> 保存的路径 </param>
        public void SaveBinaryData<T>(T dataClass, string path, FileMode fileMode = FileMode.Append) {
            try {
                IFormatter formatter = new BinaryFormatter();

                using Stream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, dataClass);

            } catch (Exception ex) {
                Debug.Log(ex.Message);
            }
        }

        /// <summary>
        /// 读取对象
        /// </summary>
        /// <typeparam name="T"> 类名 </typeparam>
        /// <param name="path"> 路径 </param>
        /// <returns> 读取到的对象 </returns>
        public T ReadBinaryData<T>(string path) {
            T data = default(T);
            try {
                IFormatter formatter = new BinaryFormatter();
                using Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

                if (stream.Length == 0) {
                    return data;

                } else {
                    data = (T)formatter.Deserialize(stream);
                }

            } catch (Exception ex) {
                Debug.Log(ex.Message);
                data = default(T);
            }
            return data;
        }

        public void DeleteFile(string path) {
            if (File.Exists(path)) {
                File.Delete(path);
            }
        }
    }


}
