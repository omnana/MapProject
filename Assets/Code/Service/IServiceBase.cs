using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServiceBase
{
    //同步
    void Setup();

    //异步
    IEnumerator SetupAsync();

    //加载完成
    void Loaded();

    //销毁
    void Dispose();
}
