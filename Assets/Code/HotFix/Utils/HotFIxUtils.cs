using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omnana
{
    public class HotFixUtils
    {
        public static readonly string HotFixProjectName = "HotFix";

        public static readonly string HotFixGuiNameFormat = "HotFix.{0}";

        public static readonly string HotFixDllPath = Application.streamingAssetsPath + "/HotFix.dll";

        public static readonly string HotFixPdbPath = Application.streamingAssetsPath + "/HotFix.pdb";
    }
}