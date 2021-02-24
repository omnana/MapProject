using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Collections;
using Newtonsoft.Json;
public class TableParser
{

    private static readonly List<string> TableHeaders = new List<string>();

    public static IEnumerator Parse<T>(string tableName, Action<T, Dictionary<string, object>> initModel, Action<T[]> callback)
    {
        T[] list = null;

       ServiceContainer.Resolve<ResourceService>().LoadTxtAsync(tableName, txt =>
        {
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(txt);

            list = new T[jsonData.Length];

            for (var i = 0; i < jsonData.Length; ++i)
            {
                var m = Activator.CreateInstance<T>();

                initModel.Invoke(m, jsonData[i]);

                list[i] = m;
            }

            callback?.Invoke(list);
        });

        yield return 0;
    }

    #region 给编辑器用

    public static List<string> GetTableHeaders(string path)
    {
        var headers = new List<string>();

        path += ".json";

        Debug.Log(path);

        if (File.Exists(path))
        {
            Debug.Log(path);

            var txt = File.ReadAllText(path);

            Debug.Log(txt);

            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(txt);

            foreach (var d in jsonData[0])
            {
                headers.Add(d.Key);
            }
        }

        return headers;
    }


    private static bool IsInt(double a)
    {
        const double eps = 1e-10;  // 精度范围

        if (a - (double)((int)a) < eps)
            return true;
        return false;
    }

    #endregion
}