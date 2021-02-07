using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundles
{
    public class ABNode
    {
        public string Name;

        public string Path;

        public bool IsRoot;

        public bool IsCombine;

        public List<string> Combinelist = new List<string>();

        public Dictionary<string, ABNode> Dependences = new Dictionary<string, ABNode>(); // 依赖项

        public Dictionary<string, ABNode> BeDependences = new Dictionary<string, ABNode>(); // 被依赖项
    }
}