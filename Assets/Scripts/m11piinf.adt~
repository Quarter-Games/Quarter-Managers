using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "ConfigManager", menuName = "QG/ConfigManager", order = 1)]
public class ConfigManager : ScriptableObject
{
    public static string AUTH_TOKEN;
    public string _tableID;
    public List<TableHandler> TablesToParse;
    public static ConfigManager Instance;
    static Action OnKeyGot = default;
    static HttpListener _httpListener;
    public void ParseAllTables()
    {
        OnKeyGot = default;
        if (!CheckKey())
        {
            OnKeyGot += ParseAllTables;
            return;
        }
        for (int i = 0; i < TablesToParse.Count; i++)
        {
            TablesToParse[i].ParseTable(_tableID);
        }
    }
    public static void RequestTable(string TableID, string TableName, List<Type> types, Action<GoogleTable> CallBack)
    {
        OnKeyGot = default;
        if (!CheckKey())
        {
            OnKeyGot += () =>
            {
                RequestTable(TableID, TableName, types, CallBack);
            };
            return;
        }
        var url = $"https://sheets.googleapis.com/v4/spreadsheets/{TableID}/values/{TableName}";
        var request = new UnityWebRequest(url, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + AUTH_TOKEN);
        request.downloadHandler = new DownloadHandlerBuffer();
        var responce = request.SendWebRequest();
        responce.completed += (async) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Request to {url} completed successfully.");
                string json = request.downloadHandler.text;
                CallBack(new GoogleTable(json, types));
            }
            else
            {
                Debug.LogError(request.uri);
                Debug.LogError($"Request to {url} failed.");
                Debug.LogError($"Error: {request.error}");
                CallBack(null);
            }
        };

    }

    private static bool CheckKey()
    {

        var saveFile = JsonUtility.FromJson<SaveFile>(FileLoader.LoadTextFile("ConfigManager.txt"));
        if (DateTime.FromBinary(saveFile.Until) < DateTime.Now)
        {
            Debug.Log(saveFile.Until);
            Debug.Log(DateTime.Now);
            Debug.Log("Key expired, requesting new key.");
            AccessApiKey(saveFile);
            return false;
        }
        else
        {
            AUTH_TOKEN = saveFile.AUTH_TOKEN;
            Debug.Log("Key is valid.");
            return true;
        }
    }
    public static void AccessApiKey(SaveFile saveFile)
    {
        string serverUrl = "http://localhost:8080";


        if (_httpListener != null)
        {
            try
            {
                _httpListener.Stop();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            try
            {
                _httpListener.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

        }

        if (!(string.IsNullOrEmpty(saveFile.CLIENT_ID) && string.IsNullOrEmpty(saveFile.CLIENT_SECRET)) && !string.IsNullOrEmpty(saveFile.AUTH_CODE))
        {
            EditorApplication.delayCall += () => TokenCall(saveFile.AUTH_CODE);
            return;
        }
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(serverUrl + "/");
        _httpListener.Start();
        _httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), _httpListener);

        var stringRequest = "https://accounts.google.com/o/oauth2/v2/auth" +
            "?" + $"client_id={saveFile.CLIENT_ID}&" +
            $"redirect_uri={serverUrl}&" +
            $"response_type=code&" +
            $"scope=https://www.googleapis.com/auth/spreadsheets&" +
            $"access_type=offline&";
        Application.OpenURL(stringRequest);
        void TokenCall(string obj)
        {
            WWWForm form = new WWWForm();
            form.AddField("code", obj);
            form.AddField("client_id", saveFile.CLIENT_ID);
            form.AddField("client_secret", saveFile.CLIENT_SECRET);
            form.AddField("redirect_uri", serverUrl);
            form.AddField("grant_type", "authorization_code");

            var request = UnityWebRequest.Post("https://oauth2.googleapis.com/token", form);

            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            var req = request.SendWebRequest();
            req.completed += (x) =>
            {

                Debug.Log(x.Equals(req));
                Debug.Log(req.webRequest.responseCode);
                Debug.Log(req.webRequest.downloadHandler.text);
                var json = req.webRequest.downloadHandler.text;
                var output = JsonUtility.FromJson<OauthResponce>(json);
                Debug.Log(output.access_token);
                ConfigManager.AUTH_TOKEN = output.access_token;
                var save = new SaveFile();
                save.AUTH_TOKEN = output.access_token;
                save.Password = saveFile.Password;
                save.CLIENT_SECRET = saveFile.CLIENT_SECRET;
                save.CLIENT_ID = saveFile.CLIENT_ID;
                save.AUTH_CODE = obj;
                var dateTime = DateTime.Now;
                Debug.Log(DateTime.Now);

                save.Until = dateTime.AddSeconds(output.expires_in).ToBinary();
                var jsonSave = JsonUtility.ToJson(save);
                FileLoader.SaveTextFile("ConfigManager.txt", jsonSave);
                OnKeyGot?.Invoke();
            };

        };
        void GetAuthComplete(string obj)
        {
            Debug.Log(obj);
            try
            {
                EditorApplication.delayCall += () => TokenCall(obj);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }



        void ListenerCallback(IAsyncResult ar)
        {
            Debug.Log("ListenerCallback");
            HttpListener listener = (HttpListener)ar.AsyncState;
            HttpListenerContext context = listener.EndGetContext(ar);
            if (context.Response.StatusCode == 200)
            {
                Debug.Log("Response Status Code: " + context.Response.StatusCode);
                string authToken = context.Request.QueryString["code"];
                context.Response.StatusCode = 200;
                GetAuthComplete(authToken);
            }
            else
            {
                Debug.LogError("Error: " + context.Response.StatusCode);
            }
            context.Response.Close();
            listener.Stop();
            _httpListener.Close();
            _httpListener.Abort();
            _httpListener.Stop();
        }
    }

    class OauthResponce
    {
        public string access_token;
        public int expires_in;
        public string scope;
        public string token_type;
    }
    public class APIResponce
    {
        public string googleClientID;
        public string googleClientSecret;
    }
    public struct SaveFile
    {
        public string Password;
        public string CLIENT_ID;
        public string CLIENT_SECRET;
        public string AUTH_CODE;
        public string AUTH_TOKEN;
        public long Until;

    }
}
public static class FileLoader
{
    public static string LoadTextFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError($"File not found at path: {filePath}");
            return null;
        }
    }
    public static void SaveTextFile(string fileName, string content)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(filePath, content);
        Debug.Log($"File saved at path: {filePath}");
    }
}
