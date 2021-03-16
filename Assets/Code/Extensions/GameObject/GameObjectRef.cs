using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omnana
{
    // 对象引用工具组件
    public class GameObjectRef : MonoBehaviour, IEnumerable<KeyValuePair<string, GameObject>>
    {
        public string[] Names;

        private Dictionary<string, GameObject> refMaps;

        public GameObject[] Refs;

        public GameObject this[string key]
        {
            get
            {
                if (refMaps == null)
                    Init();

                if (refMaps != null && refMaps.ContainsKey(key))
                    return refMaps[key];

                Debug.LogError(string.Format("{0}没有定义", key));
                return null;
            }

            set
            {
                if (refMaps == null)
                    refMaps = new Dictionary<string, GameObject>(Refs != null ? Refs.Length : 0);

                if (refMaps.ContainsKey(key))
                {
                    var idx = Array.IndexOf(Names, key);
                    if (idx != -1)
                    {
                        refMaps[key] = value;
                        //Names[idx] = key;
                        Refs[idx] = value;
                    }
                }
                else
                {
                    var lenNames = Names != null ? Names.Length : 0;
                    var lenRefs = Refs != null ? Refs.Length : 0;
                    var newNames = new string[lenNames + 1];
                    var newRefs = new GameObject[lenRefs + 1];
                    if (Names != null && Refs != null)
                    {
                        Array.Copy(Names, newNames, lenNames);
                        Array.Copy(Refs, newRefs, lenRefs);
                    }

                    newNames[lenNames] = key;
                    newRefs[lenRefs] = value;

                    refMaps.Add(key, value);
                    Names = newNames;
                    Refs = newRefs;
                }
            }
        }

        public IEnumerator<KeyValuePair<string, GameObject>> GetEnumerator()
        {
            if (refMaps == null)
                Init();

            return refMaps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Awake()
        {
            //Init();
        }

        public void Init()
        {
            var len = Refs != null ? Refs.Length : 0;
            refMaps = new Dictionary<string, GameObject>(len);
            for (var i = 0; i < len; i++)
                refMaps.Add(Names[i], Refs[i]);
        }

        public void Remove(string key)
        {
            if (refMaps == null || !refMaps.ContainsKey(key))
                return;

            if (Names == null || Names.Length <= 0 ||
                Refs == null || Refs.Length <= 0)
                return;

            var idx = Array.IndexOf(Names, key);
            if (idx == -1)
                return;

            var newNames = new string[Names.Length - 1];
            var newRefs = new GameObject[Refs.Length - 1];
            Array.Copy(Names, 0, newNames, 0, idx);
            Array.Copy(Refs, 0, newRefs, 0, idx);
            if (idx < Names.Length - 1)
            {
                Array.Copy(Names, idx + 1, newNames, idx, Names.Length - idx - 1);
                Array.Copy(Refs, idx + 1, newRefs, idx, Refs.Length - idx - 1);
            }

            refMaps.Remove(key);
            Names = newNames;
            Refs = newRefs;
        }

        public void ChangeName(string oldName, string newName)
        {
            if (refMaps == null || !refMaps.ContainsKey(oldName))
                return;

            if (Names == null || name.Length <= 0)
                return;

            var idx = Array.IndexOf(Names, oldName);
            if (idx == -1)
                return;

            var go = refMaps[oldName];
            refMaps.Remove(oldName);
            refMaps.Add(newName, go);
            Names[idx] = newName;
        }

        public void Add(string name, GameObject go)
        {
            var lenNames = Names != null ? Names.Length : 0;
            var lenRefs = Refs != null ? Refs.Length : 0;

            var newNames = new string[lenNames + 1];
            var newRefs = new GameObject[lenRefs + 1];

            if (Names != null && Refs != null)
            {
                Array.Copy(Names, newNames, lenNames);
                Array.Copy(Refs, newRefs, lenRefs);
            }

            newNames[lenNames] = name;
            newRefs[lenRefs] = go;

            //refMaps.Add(name, go);
            Names = newNames;
            Refs = newRefs;
        }

        public void Clear()
        {
            Refs = null;
            Names = null;
            refMaps = null;
        }
    }
}