using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper 
{
    private static Transform uiRoot;

    public static Transform UIRoot
    {
        get
        {
            if(uiRoot == null)
            {
                var uiObj = GameObject.Find("UI");

                if (uiObj != null)
                    uiRoot = uiObj.transform;
            }

            return uiRoot;
        }
    }

    private static Transform mgrRoot;

    public static Transform MgrRoot
    {
        get
        {
            if (mgrRoot == null)
            {
                var managerObj = GameObject.Find("Manager");

                if(managerObj != null)
                    mgrRoot = managerObj.transform;
            }

            return mgrRoot;
        }
    }
}
