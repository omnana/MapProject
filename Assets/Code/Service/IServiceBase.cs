using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omnana
{
    public interface IServiceBase
    {
        //同步
        void Setup();

        //加载完成
        void Loaded();

        //销毁
        void Dispose();
    }
}