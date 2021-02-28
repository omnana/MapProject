using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationThermalUpdates : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        //DownloadMgr.Inst.DownloadAsync(
        //new DownloadUnit()
        //{
        //    DownUrl = "https://t7.baidu.com/it/u=1595072465,3644073269&fm=193&f=GIF",
        //    SavePath = Application.streamingAssetsPath + "/Image.png",
        //    Name = "Image"
        //});

        //DownloadMgr.Inst.DownloadAsync(
        //new DownloadUnit()
        //{
        //    DownUrl = "http://192.168.31.219:80/Version.txt",
        //    SavePath = Application.streamingAssetsPath + "/Test/Version.txt",
        //    Name = "Version"
        //});

        DownloadMgr.Inst.DownloadAsync(
        new DownloadUnit()
        {
            DownUrl = "http://192.168.31.219:80/Windows/Windows.manifest",
            SavePath = Application.streamingAssetsPath + "/Windows/Windows.manifest",
            Name = "Windows.manifest"
        });
    }
}
