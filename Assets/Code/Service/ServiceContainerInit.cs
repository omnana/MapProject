using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omnana
{
    public class ServiceHelper
    {
        public static void Load()
        {
            // 基础服务
            ServiceLocator.RegisterSingleton<ResourceService>();

            // 业务服务
            ServiceLocator.RegisterSingleton<TestServicer>();
        }
    }
}