using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourcesLoadMgr : BaseCtrl
{
    private HashSet<string> resourcesList;

    public void Init()
    {
        resourcesList = new HashSet<string>();
#if UNITY_EDITOR
        ExportConfig();
#endif
        ReadConfig();
    }

#if UNITY_EDITOR
    private void ExportConfig()
    {
        string path = Application.dataPath + "/Resources/";
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        string txt = "";
        foreach (var file in files)
        {
            if (file.EndsWith(".meta")) continue;

            string name = file.Replace(path, "");
            name = name.Substring(0, name.LastIndexOf("."));
            name = name.Replace("\\", "/");
            name = name.ToLower();
            txt += name + "\n";
        }

        path = path + "FileList.bytes";

        if (File.Exists(path)) File.Delete(path);

        File.WriteAllText(path, txt);
    }
#endif

    private void ReadConfig()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("FileList");
        string txt = textAsset.text;
        txt = txt.Replace("\r\n", "\n");

        foreach (var line in txt.Split('\n'))
        {
            if (string.IsNullOrEmpty(line)) continue;

            if (!resourcesList.Contains(line))
                resourcesList.Add(line);
        }
    }

    public bool IsFileExist(string assetName)
    {
        return resourcesList.Contains(assetName);
    }

    public ResourceRequest LoadAsync(string assetName)
    {
        if (!resourcesList.Contains(assetName))
        {
            //Utils.LogError("EditorAssetLoadMgr No Find File " + assetName);
            return null;
        }

        return Resources.LoadAsync(assetName);
    }

    public UnityEngine.Object LoadSync(string assetName)
    {
        if (!resourcesList.Contains(assetName))
        {
            //Utils.LogError("EditorAssetLoadMgr No Find File " + assetName);
            return null;
        }

        UnityEngine.Object asset = Resources.Load(assetName);

        return asset;
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
