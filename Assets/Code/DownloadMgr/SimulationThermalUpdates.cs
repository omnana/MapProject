using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationThermalUpdates : MonoBehaviour
{

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        DownloadMgr.Inst.DownloadAsync(
        new DownloadUnit()
        {
            DownUrl = ServerHelper.DownloadUrl + "Windows.manifest",
            SavePath = Application.streamingAssetsPath + "/Windows/Windows.manifest",
            Name = "Windows.manifest",
            ErrorFun = DonwloadError,
            ProgressFun = DonwloadProcess,
            CompleteFun = DonwloadComplete,
        });
    }

    private void DonwloadProcess(DownloadUnit unit, int curSize, int allize)
    {
        Debug.Log("DonwloadProcess -> " + curSize + ", all = " + allize);
    }

    private void DonwloadComplete(DownloadUnit unit)
    {
        Debug.Log("DonwloadComplete -> " + unit.Name);
    }

    private void DonwloadError(DownloadUnit unit, string msg)
    {
    }
}
