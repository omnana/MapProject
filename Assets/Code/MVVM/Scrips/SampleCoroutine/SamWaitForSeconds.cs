using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

static class CTaskExtend
{
    static public IEnumerator WaitForSeconds(float second)
    {
        DateTime init_dt = DateTime.Now;

        TimeSpan time;

        while (true)
        {
            time = DateTime.Now - init_dt;

            if (time.TotalSeconds <= second)
            {
                yield return null;
            }

            break;
        }
    }
}
