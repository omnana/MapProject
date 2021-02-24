using System.Collections.Generic;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using System.Text;
using System;

public class RestService : ServiceBase
{
    public async Task<RestResult<T>> HttpGet<T>(string url,
            Dictionary<string, string> getParams = null,
            bool message = true,
            bool cache = false)
            where T : new()
    {
        var tcs = new TaskCompletionSource<RestResult<T>>();

        var newUrl = new StringBuilder(url);

        var firstParam = true;

        if (getParams != null && getParams.Count > 0)
        {
            newUrl.Append("?");
            foreach (var param in getParams)
            {
                if (!firstParam) newUrl.Append("&");

                firstParam = false;

                newUrl.Append(param.Key + "=" + param.Value);
            }
        }

        StartCoroutine(Action(newUrl.ToString(), "Get", null, r =>
        {
            if (string.IsNullOrEmpty(r))
            {
                tcs.SetResult(null);
                return;
            }
            try
            {
                var restResult = JsonConvert.DeserializeObject<RestResult<T>>(r);
                tcs.SetResult(restResult);
            }
            catch
            {
                tcs.SetCanceled();
                Debug.LogError("接收数据为空: " + r + "  url:" + newUrl.ToString());
            }
        }));

        return await tcs.Task;
    }

    private IEnumerator Action(string url, string httpVerb, string bodyData, Action<string> callback)
    {
        var fullUrl = url;
        //Debugger.Log($"请求 {httpVerb} Url = {fullUrl}, BodyData = {bodyData}, 开始时间 = {Time.time}秒 ");
        long mRightCode = 0;
        UploadHandlerRaw uploadHandler = null;
        if (!string.IsNullOrEmpty(bodyData))
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData));
        }
 
        using (var www = new UnityWebRequest(fullUrl, httpVerb, new DownloadHandlerBuffer(), uploadHandler))
        {
            Debug.Log($"发送 {httpVerb} Url = {fullUrl}, BodyData = {bodyData},内容 = {www.downloadHandler.text}");
            float startTime = Time.time;
            www.SetRequestHeader("Content-Type", "application/json");
        
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogWarning($"{httpVerb} Url = {fullUrl}, Error : {www.error}, \nHttpCode : {www.responseCode}");
                callback?.Invoke(null);
            }
            else
            {
                mRightCode = www.responseCode;

                Debug.Log($"接收 {httpVerb} Url = {fullUrl}, BodyData = {bodyData}, 花费时间 = {Time.time - startTime}秒, 内容 = {www.downloadHandler.text}");

                callback?.Invoke(www.downloadHandler.text);

                //if (www.responseCode == (int)rightCode)
                //{
                //    Debug.Log($"接收 {httpVerb} Url = {fullUrl}, BodyData = {bodyData}, 花费时间 = {Time.time - startTime}秒, 内容 = {www.downloadHandler.text}");
                //}
                //else
                //{
                //    callback?.Invoke(null);
                //    Debug.LogError($"{httpVerb} Url = {fullUrl}, Error : {www.error}, \nHttpCode : {www.responseCode}");
                //}
            }
        }
    }

}
