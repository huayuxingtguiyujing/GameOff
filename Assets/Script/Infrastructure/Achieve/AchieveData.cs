//using GameOff2023.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.Utils.Achieve {

    /// <summary>
    /// �浵�ļ������ݽṹ��ֱ����������ӿɱ����л����ֶΣ��Գ־û�����
    /// </summary>
    [Serializable]
    public class AchieveData {
        //�浵�� ���ļ��� �浵Ψһ��ʶ
        public string achieveName;

        //�浵��Ӧ���������
        public PlayerData playerData;

        // TODO : ȷ������Ҫ���������


        public readonly DateTime createTime;

        public DateTime lastUpdateTime;

        public AchieveData(string achieveName, PlayerData playerData, DateTime createTime, DateTime lastUpdateTime) {
            this.achieveName = achieveName;
            this.playerData = playerData;
            this.createTime = createTime;
            this.lastUpdateTime = lastUpdateTime;
        }
    }
}