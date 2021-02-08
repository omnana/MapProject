using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace AssetBundles
{
    public class AssetBundleBuildMgr
    {
        public static AssetBundleBuildMgr Instance = new AssetBundleBuildMgr();

        private Dictionary<string, ABNode> abNodeDic = new Dictionary<string, ABNode>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AssetBundleBuild[] Analyze()
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

            var abBuilds = new List<AssetBundleBuild>();

            foreach (var n in abNodeDic.Values)
            {
                if (n.IsCombine) continue;

                abBuilds.Add(new AssetBundleBuild()
                {
                    assetBundleName = GetAbName(n),
                    assetNames = n.Combinelist.ToArray(),
                });
            }

            return abBuilds.ToArray();
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

        private string GetAbName(ABNode abNode)
        {
            if (abNode.IsRoot)
            {
                // root节点以所在路径为加载路径
                return abNode.Name.Replace("Assets/", "") + ".ab";
            }
            else
            {
                // 依赖节点以guid作为路径
                return "depends/" + AssetDatabase.AssetPathToGUID(abNode.Path) + ".ab";
            }
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
                    IsRoot = true,
                };

                abNode.Combinelist.Add(path);

                abNodeDic.Add(path, abNode);
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
                MergeWithBeDependNode(abNode);
            }

            foreach (var abNode in abNodeDic.Values)
            {
                MergeWithDependNode(abNode);
            }
        }

        /// <summary>
        /// 跟被依赖合并
        /// </summary>
        /// <param name="abNode"></param>
        private void MergeWithBeDependNode(ABNode abNode)
        {
            if (abNode.IsCombine) return;

            if (abNode.Path.ToLower().EndsWith(".shader")) return;

            if (abNode.Dependences.Count == 0)
            {
                // 向上合并
                if (abNode.BeDependences.Count == 1)
                {
                    var beDepend = abNode.BeDependences.Values.ToArray()[0];

                    abNode.IsCombine = true;

                    beDepend.Combinelist.AddRange(abNode.Combinelist);

                    abNode.BeDependences.Remove(beDepend.Path);

                    beDepend.Dependences.Remove(abNode.Path);
                }
            }
            else
            {
                var depends = abNode.Dependences.Values.ToArray();

                for (var i = 0; i < depends.Length; i++)
                {
                    MergeWithBeDependNode(depends[i]);
                }
            }
        }

        /// <summary>
        /// 所有依赖项合并
        /// </summary>
        /// <param name="abNode"></param>
        private void MergeWithDependNode(ABNode abNode)
        {
            if (abNode.IsCombine) return;

            var depends = abNode.Dependences.Values.ToArray();

            for (var i = 0; i < depends.Length; i++)
            {
                if (depends[i].IsCombine) continue;

                for (var j = i + 1; j < depends.Length; j++)
                {
                    if (depends[j].IsCombine) continue;

                    if (IsBeDependsEqual(depends[i], depends[j]))
                    {
                        depends[i].Combinelist.AddRange(depends[j].Combinelist);

                        foreach(var beDepend in depends[j].BeDependences.Values)
                        {
                            beDepend.Dependences.Remove(depends[j].Path);
                        }

                        depends[j].IsCombine = true;

                        depends[j].BeDependences.Clear();

                        //depends[j].BeDependences.Remove(abNode.Path);

                        abNode.Dependences.Remove(depends[j].Path);
                    }
                }
            }
        }

        private bool IsBeDependsEqual(ABNode a, ABNode b)
        {
            if (a.BeDependences.Count != b.BeDependences.Count) return false;

            if (a.BeDependences.Count == 0) return false;

            foreach (var beDepend in a.BeDependences.Values)
            {
                if (!b.BeDependences.ContainsKey(beDepend.Path)) return false;
            }

            return true;
        }

        public void Clear()
        {
            abNodeDic.Clear();
        }
    } 
}