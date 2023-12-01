using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.GamePlay.Map {
    public interface IRoundInterface {

        public void MapStart();

        // 一整个回合轮开始
        public void RoundStart();

        // 一整个回合轮结束
        public void RoundOver();

        public void MapOver();

    }
}