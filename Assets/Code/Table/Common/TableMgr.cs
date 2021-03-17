using System.Collections.Generic;
using UnityEngine;
using System;

namespace Omnana
{
    public abstract class TableManager<T> where T : ITableModel
    {
        public abstract string TableName();

        public abstract void InitModel(T model, Dictionary<string, string> cellMap);

        public object TableData => modelArray;

        private T[] modelArray;

        private readonly Dictionary<object, int> keyModelMap = new Dictionary<object, int>();

        internal TableManager()
        {
            Reload();
        }

        public void Reload()
        {
            TableParser.Parse<T>(TableName(), InitModel, data =>
            {
                if (data != null)
                {
                    keyModelMap.Clear();

                    modelArray = data;

                    for (var i = 0; i < modelArray.Length; ++i)
                    {
                        if (modelArray[i] != null)
                        {
                            var key = modelArray[i].Key();
                            if (keyModelMap.ContainsKey(key))
                            {
                                keyModelMap[key] = i;
                            }
                            else
                            {
                                keyModelMap.Add(key, i);
                            }
                        }
                    }
                }

                MessageAggregator<object>.Instance.Publish(MessageType.TableMgrLoadFinish, this, null);
            });
        }

        /// <summary>
        /// 根据Id，获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetTableById(object key)
        {
            if (keyModelMap.ContainsKey(key))
            {
                if (modelArray != null)
                {
                    return modelArray[keyModelMap[key]];
                }
            }
            else
            {
                Debug.LogWarning("本地" + TableName() + "表， 不存在Id为" + key + "的数据！！");
            }

            return default;
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public T[] GetAllTable()
        {
            return modelArray;
        }

        public List<T> GetTables(Func<T, bool> comp)
        {
            var list = new List<T>();

            if (modelArray != null)
            {
                for (var i = 0; i < modelArray.Length; ++i)
                {
                    if (comp(modelArray[i]))
                    {
                        list.Add(modelArray[i]);
                    }
                }
            }

            return list;
        }
    }
}
