using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Omnana
{
    [CustomEditor(typeof(GameObjectRef))]
    public class GameObjectRefEditor : Editor
    {
        public SerializedProperty RefsProp;
        public SerializedProperty NamesProp;

        private string[] names;
        private GameObject[] refs;

        void OnEnable()
        {
            RefsProp = serializedObject.FindProperty("Refs");
            NamesProp = serializedObject.FindProperty("Names");

            names = (serializedObject.targetObject as GameObjectRef).Names;
            names = names != null ? names : new string[0];

            refs = (serializedObject.targetObject as GameObjectRef).Refs;
            refs = refs != null ? refs : new GameObject[0];
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(RefsProp, new GUIContent("Refs"), true);

            var gof = serializedObject.targetObject as GameObjectRef;

            if (gof.Names == null || gof.Names.Length != RefsProp.arraySize)
            {
                var newNames = new string[RefsProp.arraySize];
                Array.Copy(names, newNames, Mathf.Min(names.Length, RefsProp.arraySize));
                gof.Names = newNames;
                for (int i = names.Length; i < RefsProp.arraySize; i++)
                    gof.Names[i] = RefsProp.GetArrayElementAtIndex(i).objectReferenceValue.name;
            }

            names = gof.Names;
            EditorGUILayout.PropertyField(NamesProp, new GUIContent("Names"), true);

            // 刷新按钮
            if (GUILayout.Button("Refresh", GUILayout.Width(120)))
            {
                ClearAllNullRefs();
            }

            serializedObject.ApplyModifiedProperties();
        }

        // 过滤掉子对象不存在的项
        private void ClearAllNullRefs()
        {
            var gof = serializedObject.targetObject as GameObjectRef;
            List<string> newNames = new List<string>();
            List<GameObject> newRefs = new List<GameObject>();

            for (int i = 0; i < names.Length; i++)
            {
                if (refs[i] == null)
                    continue;

                newNames.Add(names[i]);
                newRefs.Add(refs[i]);
            }

            gof.Names = newNames.ToArray();
            gof.Refs = newRefs.ToArray();
            names = gof.Names;
            refs = gof.Refs;

            serializedObject.Update();
        }
    }

}