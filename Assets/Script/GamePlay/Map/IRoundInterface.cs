using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOff2023.GamePlay.Map {
    public interface IRoundInterface {

        public void MapStart();

        // һ�����غ��ֿ�ʼ
        public void RoundStart();

        // һ�����غ��ֽ���
        public void RoundOver();

        public void MapOver();

    }
}