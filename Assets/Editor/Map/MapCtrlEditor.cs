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

            GUILayout.Label("�༭�����ܣ�");

            if (GUILayout.Button("����ս����ͼ")) {
                mapController.CreateMapGrids();
            }
        }

    }
}