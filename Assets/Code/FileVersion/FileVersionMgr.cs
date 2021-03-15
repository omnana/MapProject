using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class FileVersionMgr : MonoBehaviour
{
    public int DownloadSize
    {
        get
        {
            var size = 0;

            foreach(var d in fileList)
            {
                size += d.Value.Size;
            }

            return size;
        }
    }

    private Dictionary<string, FileVersionData> fileList;

    public void Init()
    {
        fileList = new Dictionary<string, FileVersionData>();
    }

    public void Clear()
    {
        fileList.Clear();
    }

    public void InitVersionFile(string path)
    {
        fileList.Clear();

        if (!File.Exists(path)) return;

        var lines = File.ReadAllLines(path);

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue;

            var fileVersionData = new FileVersionData();

            fileVersionData.InitData(line);

            fileList.Add(fileVersionData.Name, fileVersionData);
        }

    }

    public void SaveVersionFile(string path)
    {
        if (File.Exists(path)) File.Delete(path);

        var sb = new StringBuilder();

        foreach (var fileData in fileList.Values)
        {
            sb.Append(fileData.ToString() + "\n");
        }

        File.WriteAllText(path, sb.ToString());
    }

    public FileVersionData GetFileVersionData(string Name)
    {
        if (fileList.ContainsKey(Name))
            return fileList[Name];

        return null;
    }

    // 提取大于当前版本文件
    public List<FileVersionData> FindUpdateFiles(int version)
    {
        List<FileVersionData> updateList = new List<FileVersionData>();

        foreach (var fileData in fileList.Values)
        {
            //大包初始版本，不提取包内资源
            if (fileData.InitPackage) continue;

            if (fileData.Version >= version)
                updateList.Add(fileData);
        }

        return updateList;
    }

    // 根据版本获取路径
    public string GetFilePath(string Name)
    {
        if (fileList.ContainsKey(Name) && fileList[Name].InitPackage)
        {
            return GPath.StreamingAssetsPath + Name;
        }

        return GPath.StreamingAssetsPath + Name;
    }

    // 根据文件是否存在，返回路径，没有返回null
    public string GetFilePathByExist(string Name)
    {
        if (fileList.ContainsKey(Name) && fileList[Name].InitPackage)
        {
            return GPath.StreamingAssetsPath + Name;
        }

        string path = GPath.StreamingAssetsPath + Name;
        if (File.Exists(path)) return path;

        path = GPath.StreamingAssetsPath + Name;
        if (File.Exists(path)) return path;

        return null;
    }


    // 新文件或有更改，替换
    public bool ReplaceFileVersionData(FileVersionData newData)
    {
        if (!fileList.ContainsKey(newData.Name))
        {
            fileList.Add(newData.Name, newData);
            return true;
        }

        var oldData = fileList[newData.Name];

        if (newData.Size != oldData.Size || newData.Md5 != oldData.Md5)
        {
            fileList[newData.Name] = newData;
            return true;
        }
        return false;
    }

    public void DeleteFileVersionData(string name)
    {
        if (fileList.ContainsKey(name))
            fileList.Remove(name);
    }
}
