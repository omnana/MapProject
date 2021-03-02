using UnityEngine;

public class ControllerInit : MonoBehaviour
{
    public void Load()
    {
        ServiceLocator.RegisterSingleton<TestCtrl>();
    }
}
