using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace AssetBundles
{
    public class AssetBundleBuildMgr
    {
        public static AssetBundleBuildMgr Instance = new AssetBundleBuildMgr();

        private List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();

        private Dictionary<string, ABNode> abNodeDic = new Dictionary<string, ABNode>();

        public void Analyze()
        {
            Clear();

            List<string> targetFiles = new List<string>();

            string[] files = Directory.GetFiles(Utility.AssetBuildPath, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                if (file.EndsWith(".meta")) continue;

                targetFiles.Add(file);
            }

            AnalyzeComplex(targetFiles);
        }

        private bool IsFileVaild(string filePath)
        {
            filePath = filePath.ToLower();

            if (filePath.EndsWith(".meta")) return false;
            if (filePath.EndsWith(".cs")) return false;
            if (filePath.EndsWith(".dll")) return false;
            if (filePath.Contains("arteditor")) return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        private void AnalyzeComplex(List<string> files)
        {
            // 建立依赖节点，生成一张有向无环图
            for (int i = 0; i < files.Count; ++i)
            {
                //提取在unity资源Assets目录下路径
                string unityPath = files[i].Substring(files[i].IndexOf("Assets/"));

                unityPath = unityPath.Replace("\\", "/");

                if (!IsFileVaild(unityPath)) continue;

                var abNode = CreateNode(unityPath);

                AnalyzeNode(abNode);
            }

            // 剪枝
            CropAbNodeMap();

            // 删除多余节点
            DeleteRedundantNode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private ABNode CreateNode(string path)
        {
            if (!abNodeDic.ContainsKey(path))
            {
                var abNode = new ABNode()
                {
                    Name = Path.GetFileNameWithoutExtension(path),
                    Path = path,
                    IsRoot = false,
                };

                abNode.Combinelist.Add(path);

                abNodeDic.Add(path, abNode);
            }
            else
            {
                abNodeDic[path].IsRoot = true;
            }

            return abNodeDic[path];
        }

        /// <summary>
        /// 加载依赖项
        /// </summary>
        /// <param name="abNode"></param>
        private void AnalyzeNode(ABNode abNode)
        {
            //提取依赖项
            string[] dependPaths = AssetDatabase.GetDependencies(abNode.Path);

            foreach (var tempDependPath in dependPaths)
            {
                string path = tempDependPath.Replace("\\", "/");

                if (path.Equals(abNode.Path)) continue; //依赖自己

                if (!IsFileVaild(path)) continue; //判读文件合法性。具体看整合代码

                var abDependNode = CreateNode(path);

                abNode.Dependences.Add(path, abDependNode); //添加依赖项

                abDependNode.BeDependences.Add(abNode.Path, abNode); //添加被依赖项

                if (!abNodeDic.ContainsKey(path))
                {
                    abNodeDic.Add(path, abDependNode);

                    AnalyzeNode(abDependNode); //递归分析，保证所有节点被创建
                }
            }
        }

        /// <summary>
        /// 裁剪
        /// </summary>
        /// <param name="root"></param>
        private void CropAbNodeMap()
        {
            foreach(var abNode in abNodeDic.Values)
            {
                if (abNode.BeDependences.Count < 2) continue;

                var cropList = new List<string>();

                // 被依赖项
                foreach (var beDepend in abNode.BeDependences.Values)
                {
                    var cropNodePath = string.Empty;

                    // 被依赖项的依赖项
                    foreach (var depend in beDepend.Dependences.Values)
                    {
                        //  被依赖项的依赖项的依赖项已经包含abNode，裁剪掉
                        if (depend.Dependences.ContainsKey(abNode.Path))
                        {
                            cropNodePath = abNode.Path;

                            cropList.Add(beDepend.Path);
                        }
                    }

                    if (!string.IsNullOrEmpty(cropNodePath))
                    {
                        beDepend.Dependences.Remove(cropNodePath);
                    }
                }

                foreach(var c in cropList)
                {
                    abNode.BeDependences.Remove(c);
                }
            }
        }

        /// <summary>
        /// 减去多余的资源包
        /// </summary>
        private void DeleteRedundantNode()
        {
            foreach (var abNode in abNodeDic.Values)
            {
                var removeList = new List<string>();

                if (abNode.Dependences.Count == 1)
                {
                   var depend = abNode.Dependences.GetEnumerator().Current.Value;

                    // 当该依赖项的被依赖项只有一个，向上合并
                    if (depend.BeDependences.Count == 1)
                    {

                    }
                }
                else
                {

                }

                foreach (var depend in abNode.Dependences.Values)
                {
                    // 当依赖项没有依赖项
                    if(depend.Dependences.Count == 0)
                    {
                        // 当被依赖项只有一个
                        if (depend.BeDependences.Count == 1)
                        {
                            //（1）向上合并
                            if(abNode.Dependences.Count < 2)
                            {
                                abNode.IsCombine = true;

                                abNode.Combinelist.Add(depend.Path);

                                depend.BeDependences.Remove(abNode.Path);
                            }
                            //（2）同级合并
                            else
                            {

                            }
                        }
                    }
                }

                foreach (var r in removeList)
                {
                    abNode.Dependences.Remove(r);
                }
            }
        }

        public void Clear()
        {
            assetBundleBuilds.Clear();

            abNodeDic.Clear();
        }
    } 
}