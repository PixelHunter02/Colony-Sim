using System.Text;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public static class WebRequest
{
    private class WebRequestMonoBehaviour : MonoBehaviour {}

    private static WebRequestMonoBehaviour _webRequestMonoBehaviour;

    private static void Init() {
        if(_webRequestMonoBehaviour == null)
        {
            var gameObject = new GameObject("WebRequests");
            _webRequestMonoBehaviour = gameObject.AddComponent<WebRequestMonoBehaviour>();
        }

    }

    public static void GetText(string url, Action<string> onError, Action<string> onSuccess)
    {
        Init();
        _webRequestMonoBehaviour.StartCoroutine(GetCoroutine(url, onError, onSuccess));
    }

    private static IEnumerator GetCoroutine(string url, Action<string> onError, Action<string> onSuccess)
    {
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
        {
            Debug.Log(unityWebRequest.downloadProgress);  
            yield return unityWebRequest.SendWebRequest();
            if(unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                onError(unityWebRequest.error);
            }
            else
            {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }
    }

    public static void GetTexture(string url, Action<string> onError, Action<Texture2D> onSuccess)
    {
        Init();
        _webRequestMonoBehaviour.StartCoroutine(GetTextureCoroutine(url, onError, onSuccess));
    }

    private static IEnumerator GetTextureCoroutine(string url, Action<string> onError, Action<Texture2D> onSuccess)
    {
        using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return unityWebRequest.SendWebRequest();
            if(unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                onError(unityWebRequest.error);
            }
            else
            {
                DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                onSuccess(downloadHandlerTexture.texture);
            }
        }
    }

    public static void SendJsonData(string uri, string jsonData, Action<string> onError, Action<string> onSuccess)
    {
        Init();
        _webRequestMonoBehaviour.StartCoroutine(SendJsonDataCoroutine(uri, jsonData, onError, onSuccess));
    }

    private static IEnumerator SendJsonDataCoroutine(string uri, string jsonData, Action<string> onError, Action<string> onSuccess)
    {
        using(UnityWebRequest unityWebRequest = new UnityWebRequest (uri, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            
            yield return unityWebRequest.SendWebRequest();
            
            if(unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                onError(unityWebRequest.error);
            }
            else
            {
                Debug.Log("Uploaded");
            }
        }
    }
} 