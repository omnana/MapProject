using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TableManager<T, M> : ITableManager<T> where T : ITableModel 
{
    public static M Ins { get; set; }

    public abstract string TableName();

    public abstract void InitModel(T model, Dictionary<string, string> cellMap);

    public object TableData => _modelArray;

    private T[] _modelArray;

    private readonly Dictionary<object, int> _keyModelMap = new Dictionary<object, int>();

    internal TableManager()
    {
    }

    public virtual void Init()
    {
        //Reload();
    }

    public void Reload(Action callback)
    {
        ServiceContainer.Resolve<ResourceService>().StartCoroutine(TableParser.Parse<T>(TableName(), InitModel, data =>
        {
            if (data != null)
            {
                _keyModelMap.Clear();

                _modelArray = data;

                for (var i = 0; i < _modelArray.Length; ++i)
                {
                    if (_modelArray[i] != null)
                    {
                        var key = _modelArray[i].Key();
                        if (_keyModelMap.ContainsKey(key))
                        {
                            _keyModelMap[key] = i;
                        }
                        else
                        {
                            _keyModelMap.Add(key, i);
                        }
                    }
                }
            }

            callback?.Invoke();
        }));
    }

    /// <summary>
    /// 根据Id，获取数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetModelById(object key)
    {
        int index;
        if (_keyModelMap.TryGetValue(key, out index))
        {
            if (_modelArray != null)
            {
                return _modelArray[index];
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
        return _modelArray;
    }

    public List<T> GetAllModel(Func<T, bool> comp)
    {
        var list = new List<T>();

        if (_modelArray != null)
        {
            for (var i = 0; i < _modelArray.Length; ++i)
            {
                if (comp(_modelArray[i]))
                {
                    list.Add(_modelArray[i]);
                }
            }
        }

        return list;
    }
}
