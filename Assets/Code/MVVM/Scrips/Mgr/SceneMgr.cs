using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;
using UnityEngine.SceneManagement;

public class SceneMgr : BaseCtrl
{
    private ResourceService resourceService;

    public SceneMgr()
    {
        resourceService = ServiceLocator.Resolve<ResourceService>();
    }

    public void LoadScene(string sceneName)
    {
    }
}
