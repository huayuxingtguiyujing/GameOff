using GameOff2023.GamePlay.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameOff2023.EditorPart.Map {
    [CustomEditor(typeof(MapController))]
    public class MapCtrlEditor : Editor {
        

        public override void OnInspectorGUI() {
            
            base.OnInspectorGUI();
            MapController mapController = (MapController)target;

            GUILayout.Space(10);

            GUILayout.Label("编辑器功能：");

            if (GUILayout.Button("绘制战斗地图")) {
                mapController.CreateMapGrids();
            }
        }

    }
}