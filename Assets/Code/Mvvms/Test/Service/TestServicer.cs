using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TestServicer : ServiceBase
{
    public RestService RestService;

    public override void Loaded()
    {
        base.Loaded();

        RestService = ServiceContainer.Resolve<RestService>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<string> Test()
    {
        var data = await RestService.HttpGet<object>("www.baidu.com");

        return "Test";
    }
}
