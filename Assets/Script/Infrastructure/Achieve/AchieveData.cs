//using GameOff2023.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.Utils.Achieve {

    /// <summary>
    /// 存档文件的数据结构，直接在其中添加可被序列化的字段，以持久化数据
    /// </summary>
    [Serializable]
    public class AchieveData {
        //存档名 即文件名 存档唯一标识
        public string achieveName;

        //存档对应的玩家数据
        public PlayerData playerData;

        // TODO : 确定更多要保存的数据


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