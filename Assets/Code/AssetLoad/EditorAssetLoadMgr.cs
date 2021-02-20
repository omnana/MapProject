using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class EditorAssetLoadMgr : BaseCtrl
{
    public Dictionary<string, string> resourceDic;

    private const string AssetPath = "Assets/Art/{0}";

    public void Init()
    {
        resourceDic = new Dictionary<string, string>();

        ReadConfig();
    }

    private void ReadConfig()
    {
        string path = Application.dataPath + "/Art/";

        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        string txt = "";
        foreach (var file in files)
        {
            if (file.EndsWith(".meta")) continue;

            string name = file.Replace(path, "");
            //name = name.Substring(0, name.LastIndexOf("."));
            name = name.ToLower();
            name = name.Replace("\\", "/");

            var startIndex = name.LastIndexOf("/")  + 1;

            if(startIndex != -1 && name.Length > startIndex)
            {
                var cutName = name.Substring(0, name.LastIndexOf("."));

                var assetName = cutName.Substring(startIndex, cutName.Length - startIndex);

                if (!resourceDic.ContainsKey(assetName))
                {
                    resourceDic.Add(assetName, name);
                }
            }

            txt += name + "\n";
        }
    }

    public bool IsFileExist(string assetName)
    {
        return resourceDic.ContainsKey(assetName);
    }

    private string GetAssetPath(string assetName)
    {
        return string.Format(AssetPath, assetName);
    }

    public Object LoadSync(string assetName)
    {
        if (resourceDic.ContainsKey(assetName))
        {
            var assetPath = GetAssetPath(resourceDic[assetName]);

            return AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        }

        return null;
    }


    public void Unload(Object asset)
    {
        if (asset is GameObject)
        {
            return;
        }

        Resources.UnloadAsset(asset);

        asset = null;
    }
}
