using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using UnityEditor;

namespace Omnana
{

    public class TreeNodeDataCtrl
    {
        public static string RootName = "RedPointRoot";

        public static TreeNodeDataCtrl Inst { get; private set; } = new TreeNodeDataCtrl();

        private TreeNode root;

        public TreeNode Root => root;

        private HashSet<string> nodeNameHashset = new HashSet<string>();

        private RedPointConfigData configData;

        private List<int> idList = new List<int>();

        public int DataRow => idList.Count;

        public void LoadEditorTreeNodeData()
        {
            idList.Clear();

            nodeNameHashset.Clear();

            var str = FileHelper.ReadTxt(RedPointSystem.RedPointConfigOutputPath);

            root = new TreeNode(RootName);

            if (string.IsNullOrEmpty(str))
            {
                configData = new RedPointConfigData()
                {
                    ChildsDic = new Dictionary<int, List<int>>(),

                    ParentDic = new Dictionary<int, int>(),

                    CommentDic = new Dictionary<int, string>()
                };

                idList.Add(0);

                nodeNameHashset.Add(RootName);
            }
            else
            {
                //str = Util.ReStrEncryption(str, EncryptionKey);

                configData = JsonConvert.DeserializeObject<RedPointConfigData>(str);

                if (configData.ChildsDic == null)
                {
                    configData.ChildsDic = new Dictionary<int, List<int>>();
                }

                if (configData.ParentDic == null)
                {
                    configData.ParentDic = new Dictionary<int, int>();
                }

                if (configData.CommentDic == null)
                {
                    configData.CommentDic = new Dictionary<int, string>();
                }

                CreateAllTreeNode(0, root);
            }
        }

        /// <summary>
        /// 创建红点树
        /// </summary>
        /// <param name="id"></param>
        /// <param name="node"></param>
        private void CreateAllTreeNode(int id, TreeNode node)
        {
            node.RedPoint = (RedPoint)id;

            node.Id = id;

            if (node.Id == 0)
            {
                node.IsOpen = true;
            }

            idList.Add(node.Id);

            nodeNameHashset.Add(node.RedPoint.ToString());

            if (configData.ChildsDic.ContainsKey(node.Id))
            {
                var childs = configData.ChildsDic[node.Id];

                foreach (var c in childs)
                {
                    var redPoint = (RedPoint)c;

                    var curNode = new TreeNode(redPoint.ToString())
                    {
                        RedPoint = redPoint
                    };

                    node.AddChild(curNode);

                    CreateAllTreeNode(c, curNode);
                }
            }
        }

        /// <summary>
        /// 红点树转为字典
        /// </summary>
        /// <param name="node"></param>
        private void TreeNodeToDic(ITreeNode node)
        {
            if (!configData.ChildsDic.ContainsKey(node.Id) && node.Children != null)
            {
                configData.ChildsDic.Add(node.Id, new List<int>());
            }

            if (node.Children != null)
            {
                foreach (var c in node.Children)
                {
                    configData.ChildsDic[node.Id].Add(c.Id);

                    if (!configData.ParentDic.ContainsKey(c.Id))
                    {
                        configData.ParentDic.Add(c.Id, node.Id);
                    }

                    TreeNodeToDic(c);
                }
            }
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeName"></param>
        public void CreateChildNode(ITreeNode node, string nodeName)
        {
            if (ContianKey(nodeName))
            {
                return;
            }

            node.IsOpen = true;

            var n = new TreeNode(nodeName)
            {
                Id = FindEffectieveRedPointId(),

                Parent = node,
            };

            idList.Add(n.Id);

            nodeNameHashset.Add(n.DisplayString);

            node.AddChild(n);
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(ITreeNode node)
        {
            if (node.Parent != null)
            {
                node.Parent.Children.Remove(node);
            }

            ErgodicTreeNode(node, (childNode) =>
            {
                idList.Remove(childNode.Id);
                nodeNameHashset.Remove(childNode.DisplayString);
                RemoveRedPointComment(childNode.RedPoint);
            });

            node = null;
        }

        /// <summary>
        /// 遍历节点以下所有子节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name=""></param>
        /// <param name=""></param>
        private void ErgodicTreeNode(ITreeNode node, Action<ITreeNode> callback)
        {
            var stack = new Stack<ITreeNode>();

            stack.Push(node);

            while (stack.Count != 0)
            {
                var n = stack.Pop();

                callback?.Invoke(n);

                if (n.Children != null)
                {
                    var count = n.Children.Count;

                    for (var i = count - 1; i > -1; i--)
                    {
                        stack.Push(n.Children[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 重置节点名
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        public void ReNameNode(ITreeNode node, string name)
        {
            node.ReName(name);
        }

        /// <summary>
        /// 添加注释
        /// </summary>
        /// <param name="redPoint"></param>
        /// <param name="content"></param>
        public void AddRedPointComment(RedPoint redPoint, string content)
        {
            if (!configData.CommentDic.ContainsKey((int)redPoint))
            {
                configData.CommentDic.Add((int)redPoint, content);
            }

            configData.CommentDic[(int)redPoint] = content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redPoint"></param>
        /// <returns></returns>
        public string GetRedPointComment(RedPoint redPoint)
        {
            if (!configData.CommentDic.ContainsKey((int)redPoint))
                return string.Empty;

            return configData.CommentDic[(int)redPoint];
        }

        /// <summary>
        /// 删除注释
        /// </summary>
        /// <param name="redPoint"></param>
        /// <param name="content"></param>
        public void RemoveRedPointComment(RedPoint redPoint)
        {
            configData.CommentDic.Remove((int)redPoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public bool ContianKey(string keyName)
        {
            return nodeNameHashset.Contains(keyName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private int FindEffectieveRedPointId()
        {
            var maxIndex = 0;

            foreach (var i in idList)
            {
                if (maxIndex < i) maxIndex = i;
            }

            return maxIndex + 1;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveData()
        {
            configData.ChildsDic.Clear();

            configData.ParentDic.Clear();

            TreeNodeToDic(root);

            SaveRedPointScripts();

            //var str = Util.StrEncryption(JsonConvert.SerializeObject(configData), EncryptionKey);

            FileHelper.WriteTxt(RedPointSystem.RedPointConfigOutputPath, JsonConvert.SerializeObject(configData));
        }

        /// <summary>
        /// 生成红点枚举脚本
        /// </summary>
        private void SaveRedPointScripts()
        {
            var scriptFoldPath = RedPointSystem.RedPointEnumScriptOutputPath;

            if (!Directory.Exists(scriptFoldPath)) Directory.CreateDirectory(scriptFoldPath);

            string codeFullName = scriptFoldPath + "/RedPoint.cs";

            var sb = new StringBuilder();
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// 红点枚举");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("public enum RedPoint");
            sb.AppendLine("{");
            ErgodicTreeNode(root, (node) =>
            {
                sb.AppendLine("    /// <summary>");
                sb.AppendLine(string.Format("    /// {0}", GetRedPointComment(node.RedPoint)));
                sb.AppendLine("    /// </summary>");
                sb.AppendLine(string.Format("    {0} = {1},", node.DisplayString, node.Id));
            });
            sb.AppendLine("}");

            using (FileStream fs = new FileStream(codeFullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                sw.Write(sb.ToString());
                sw.Close();
                fs.Close();
            }
            Debug.Log("生成红点枚举成功:");
        }
    }

}