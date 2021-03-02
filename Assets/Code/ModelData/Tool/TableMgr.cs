using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TableManager<T, M> : ITableManager<T> where T : ITableModel 
{
    public static M Ins { get; set; }

    public abstract string TableName();

    public abstract void InitModel(T model, Dictionary<string, string> cellMap);

    public object TableData => modelArray;

    private T[] modelArray;

    private readonly Dictionary<object, int> keyModelMap = new Dictionary<object, int>();

    internal TableManager()
    {
    }

    public virtual void Init()
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
        });
    }

    /// <summary>
    /// 根据Id，获取数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetModelById(object key)
    {
        int index;
        if (keyModelMap.TryGetValue(key, out index))
        {
            if (modelArray != null)
            {
                return modelArray[index];
            }
        }
        else
        {
            Debug.LogWarning("本地" + TableName() + "表， 不存在Id为" + index + "的数据！！");
        }

        return default(T);
    }

    /// <summary>
    /// 获取所有数据
    /// </summary>
    /// <returns></returns>
    public T[] GetAllModel()
    {
        return modelArray;
    }

    public List<T> GetAllModel(Func<T, bool> comp)
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
