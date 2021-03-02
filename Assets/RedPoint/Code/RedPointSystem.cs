using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System;

public class RedPointSystem
{
    public static string RedPointEnumScriptOutputPath = Application.dataPath + "/RedPoint/Code";

    public static string RedPointConfigOutputPath = Application.dataPath + "/RedPoint/Plugins/RedPointConfig.txt";

    private static RedPointSystem inst;

    public static RedPointSystem Inst
    {
        get
        {
            if(inst == null)
            {
                inst = new RedPointSystem();

                inst.LoadConfigData();
            }

            return inst;
        }
    }

    /// <summary>
    /// 子节点
    /// </summary>
    public static Dictionary<int, List<int>> childsDic = new Dictionary<int, List<int>>();

    /// <summary>
    /// 父节点
    /// </summary>
    public static Dictionary<int, int> parentDic = new Dictionary<int, int>();

    /// <summary>
    /// 
    /// </summary>
    private Dictionary<int, Action<bool>> redPointEventDic = new Dictionary<int, Action<bool>>();

    /// <summary>
    /// 
    /// </summary>
    private Dictionary<int, bool> redPointStatusDic = new Dictionary<int, bool>();

    /// <summary>
    /// 加载配置
    /// </summary>
    public void LoadConfigData()
    {
        var configDataStr = FileUtil.ReadTxt(RedPointConfigOutputPath);

        if (!string.IsNullOrEmpty(configDataStr))
        {
            var configData = JsonConvert.DeserializeObject<RedPointConfigData>(configDataStr);

            childsDic = configData.ChildsDic;

            parentDic = configData.ParentDic;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="redPoint"></param>
    /// <param name="callback"></param>
    public void AddObserver(RedPoint redPoint, Action<bool> callback)
    {
        CheckRedPointContains((int)redPoint);

        var redPointValue = (int)redPoint;

        if (redPointEventDic.ContainsKey(redPointValue)) return;

        redPointEventDic.Add(redPointValue, callback);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="redPoint"></param>
    public void RemoveObserver(RedPoint redPoint)
    {
        var redPointValue = (int)redPoint;

        redPointEventDic.Remove(redPointValue);

        redPointStatusDic.Remove(redPointValue);
    }

    /// <summary>
    /// 重置红点状态
    /// </summary>
    /// <param name="redPoint"></param>
    public void SetActive(RedPoint redPoint, bool active)
    {
        var curPoint = (int)redPoint;

        CheckRedPointContains(curPoint);

        while (curPoint != 0)
        {
            var childs = GetChilds(curPoint);

            var findRedChild = false; // 有没有儿子有红点

            if (childs != null)
            {
                for (var i = 0; i < childs.Count; i++)
                {
                    var childRedPointValue = childs[i];

                    CheckRedPointContains(childRedPointValue);

                    if (redPointStatusDic[childRedPointValue])
                    {
                        findRedChild = true;
                        break;
                    }
                }
            }

            var lastActive = redPointStatusDic[curPoint];

            if (findRedChild)
            {
                redPointStatusDic[curPoint] = true;
            }
            else
            {
                redPointStatusDic[curPoint] = active;
            }

            var refresh = lastActive != redPointStatusDic[curPoint];

            if (refresh && redPointEventDic.ContainsKey(curPoint)) // 状态改变，刷新
            {
                redPointEventDic[curPoint](redPointStatusDic[curPoint]);
            }

            curPoint = GetParent(curPoint);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="redPoint"></param>
    private void CheckRedPointContains(int redPoint)
    {
        if (!redPointStatusDic.ContainsKey(redPoint))
            redPointStatusDic.Add(redPoint, false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="redPoint"></param>
    /// <returns></returns>
    public bool GetRedPointStatus(RedPoint redPoint)
    {
        CheckRedPointContains((int)redPoint);

        var redPointValue = (int)redPoint;

        return redPointStatusDic[redPointValue];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private int GetParent(int redPoint)
    {
        return !parentDic.ContainsKey(redPoint) ? 0 : parentDic[redPoint];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private List<int> GetChilds(int redPoint)
    {
        return !childsDic.ContainsKey(redPoint) ? new List<int>() : childsDic[redPoint];
    }
}
