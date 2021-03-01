using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceContainerInit : MonoBehaviour
{
    public void Load()
    {
        // 基础服务
        ServiceContainer.AddService<RestService>(gameObject);
        ServiceContainer.AddService<ResourceService>(gameObject);

        // 业务服务
        ServiceContainer.AddService<TestServicer>(gameObject);
    }
}
