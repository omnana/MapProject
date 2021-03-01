using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Collections;
using Newtonsoft.Json;
public class TableParser
{

    private static readonly List<string> TableHeaders = new List<string>();

    /// <summary>
    /// 第一行为表头
    /// 第二行为字段类型
    /// 第三行为注释
    /// 第四行起为配置内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName"></param>
    /// <param name="initModel"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IEnumerator Parse<T>(string tableName, Action<T, Dictionary<string, string>> initModel, Action<T[]> callback)
    {
        T[] list = null;

       ServiceContainer.Resolve<ResourceService>().LoadTxtAsync(tableName, txt =>
        {
            var content = txt.Replace("\r\n", "*");

            var datas = content.Split('*');

            var count = datas.Length - 3;

            list = new T[count];

            var headers = datas[0].Split(',');

            for (var i = 3; i < count; i++)
            {
                var d = datas[i].Split(',');

                var m = Activator.CreateInstance<T>();

                var dic = new Dictionary<string, string>();

                for (var j = 0; j < headers.Length; i++)
                {
                    dic.Add(headers[j], d[j]);
                }

                initModel.Invoke(m, dic);

                list[i] = m;
            }


            callback?.Invoke(list);
        });

        yield return 0;
    }

    #region 给编辑器用

    /// <summary>
    /// 第一行为表头
    /// 第二行为字段类型
    /// 第三行为注释
    /// 第四行起为配置内容
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static List<string>[] GetTableHeaders(string path)
    {
        var headers = new List<string>[3];

        path += ".csv";

        Debug.Log(path);

        if (File.Exists(path))
        {
            Debug.Log(path);

            var content = File.ReadAllLines(path);

            for (var i = 0; i < 3; i++)
            {
                var d = content[i].Split(',');

                headers[i] = new List<string>();

                for (var j = 0; j < d.Length; j++)
                {
                    headers[i].Add(d[j]);
                }
            }
        }

        return headers;
    }

    #endregion
}