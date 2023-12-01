
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


        #region �浵����
        /// <summary>
        /// ���ݴ浵���ƻ�ȡ��ǰ�浵
        /// </summary>
        public AchieveData LoadAchieve(string achieveName) {
            List<AchieveData> achieveDataList = LoadAllAchieve();
            return achieveDataList.Find(achieve => achieve.achieveName == achieveName);
        }

        public List<AchieveData> LoadAllAchieve() {
            List<AchieveData> achieveDataList = new List<AchieveData>();
            DirectoryInfo achieveDirectory = Directory.CreateDirectory(DatabasePath);
            if (!achieveDirectory.Exists) {
                Debug.LogError("�浵�ļ��в�����");
                return achieveDataList;
            }

            //��ȡ���д浵�ļ� ת��Ϊ�浵����
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
        /// �½��浵
        /// </summary>
        public void NewAchieve() {
            List<AchieveData> achieveDataList = LoadAllAchieve();
            string achieveName = "newAchieve" + achieveDataList.Count.ToString();

            //�����浵����
            NewAchieve(achieveName);
        }

        public void NewAchieve(string achieveName) {
            PlayerData playerData = new PlayerData();
            AchieveData achieveData = new AchieveData(achieveName, playerData, DateTime.Now, DateTime.Now);

            SaveBinaryData(achieveData, DatabasePath + "/" + achieveName);
        }

        /// <summary>
        /// ����浵
        /// </summary>
        public void SaveAchieve(AchieveData achieveData) {
            SaveBinaryData(achieveData, DatabasePath + "/" + achieveData.achieveName, FileMode.Open);
        }

        /// <summary>
        /// ɾ���浵
        /// </summary>
        public void DeleteAchieve(string achieveName) {
            DeleteFile(DatabasePath + "/" + achieveName);
        }
        #endregion

        /// <summary>
        /// ���л��������
        /// </summary>
        /// <typeparam name="T"> �����л������� </typeparam>
        /// <param name="dataClass"> �����л��Ķ��� </param>
        /// <param name="path"> �����·�� </param>
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
        /// ��ȡ����
        /// </summary>
        /// <typeparam name="T"> ���� </typeparam>
        /// <param name="path"> ·�� </param>
        /// <returns> ��ȡ���Ķ��� </returns>
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
