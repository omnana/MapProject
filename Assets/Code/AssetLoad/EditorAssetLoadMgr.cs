using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class EditorAssetLoadMgr : BaseCtrl
{
    public Dictionary<string, string> resourceDic;

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
            name = name.Substring(0, name.LastIndexOf("."));
            name = name.Replace("\\", "/");

            var startIndex = name.LastIndexOf("/")  + 1;

            if(startIndex != -1 && name.Length > startIndex)
            {
                var assetName = name.Substring(startIndex, name.Length - startIndex);

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
        return false;
    }

    public Object LoadSync(string assetName)
    {
        if (resourceDic.ContainsKey(assetName))
        {
            return AssetDatabase.LoadAssetAtPath<Object>(resourceDic[assetName]);
        }

        return null;
    }


    public EditorAssetRequest LoadAsync(string assetName)
    {
        if (resourceDic.ContainsKey(assetName))
        {
            var request = new EditorAssetRequest();

            request.asset = AssetDatabase.LoadAssetAtPath<Object>(resourceDic[assetName]);

            return request;
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
