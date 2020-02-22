using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NameGenerator
{
    [CustomEditor(typeof(NameData))]
    public class NameDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            NameData data = (NameData)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate Distribution"))
                data.LoadData();
            if (GUILayout.Button("Generate Name"))
                Debug.Log(data.GenerateName());
        }
    }
}
