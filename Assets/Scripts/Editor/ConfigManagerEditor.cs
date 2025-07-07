using GoogleSheetsToUnity;
using System;
using System.Net;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static ConfigManager;

[CustomEditor(typeof(ConfigManager))]

public class ConfigManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ConfigManager configManager = (ConfigManager)target;
        if (GUILayout.Button("Parse All Tables"))
        {
            configManager.ParseAllTables();
        }

    }
}

public class ConfigManagerWindow : EditorWindow
{
    public string Password = "";
    bool toggle = false;
    public Action<string> _onComplete;
    static ConfigManager _manager;
    static ConfigManager configManager
    {
        get
        {
            if (_manager == null)
            {
                _manager = AssetDatabase.LoadAssetAtPath<ConfigManager>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:ConfigManager")[0]));
            }
            return _manager;
        }
        set
        {
            _manager = value;
        }
    }
    [MenuItem("QG/ConfigManager")]
    public static void ShowWindow()
    {
        if (configManager == null)
        {
            configManager = AssetDatabase.LoadAssetAtPath<ConfigManager>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:ConfigManager")[0]));
        }
        var window = GetWindow(typeof(ConfigManagerWindow)) as ConfigManagerWindow;
        var json = FileLoader.LoadTextFile("ConfigManager.txt");
        if (json != null)
        {
            var save = JsonUtility.FromJson<SaveFile>(json);
            window.Password = save.Password;
            ConfigManager.AUTH_TOKEN = save.AUTH_TOKEN;
        }
    }
    private void OnGUI()
    {
        toggle = EditorGUILayout.Toggle("Show Password", toggle);
        if (toggle)
        {
            Password = EditorGUILayout.TextField("Password", Password);
        }
        else
        {
            Password = EditorGUILayout.PasswordField("Password", Password);
        }
        GUILayout.Label("API Key: " + ConfigManager.AUTH_TOKEN);

        if (GUILayout.Button("Build Connection"))
        {
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/LoadAll.wav");
            PlayClip(clip);
            //GetSecretsFromPassword();
            //ConfigManager.AccessApiKey(JsonUtility.FromJson<SaveFile>(FileLoader.LoadTextFile("ConfigManager.txt")));
        }
        GUILayout.BeginArea(new Rect(15f, 100f, 200f, 200f));
        GUILayout.Label("Tables to Parse");
        if (GUILayout.Button("Load All"))
        {
            configManager.ParseAllTables();
        }
        foreach (var item in configManager.TablesToParse)
        {
            if (GUILayout.Button("Parse " + item.TableName))
            {
                item.ParseTable(configManager._tableID);
            }

        }
        GUILayout.EndArea();
    }
    public static void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

        MethodInfo method = audioUtilClass.GetMethod(
            "PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] { typeof(AudioClip), typeof(Int32), typeof(Boolean) },
            null
        );

        if (method != null)
        {
            method.Invoke(null, new object[] { clip, 0, false });
        }
        else
        {
            Debug.LogError("Method not found");
            foreach (var meth in audioUtilClass.GetMethods())
            {
                Debug.Log(meth.Name);
            }
        }

    }

    private void GetSecretsFromPassword()
    {
        {
            UnityWebRequest requestToAPIKey = UnityWebRequest.Get("https://gq-google-to-unity.fly.dev/secrets");
            requestToAPIKey.SetRequestHeader("Authorization", "Bearer " + Password);
            requestToAPIKey.SendWebRequest().completed += (AsyncOperation op) =>
            {
                if (requestToAPIKey.result == UnityWebRequest.Result.Success)
                {
                    string json = requestToAPIKey.downloadHandler.text;
                    var save = JsonUtility.FromJson<APIResponce>(json);

                    SaveFile saveFile = new SaveFile
                    {
                        Password = Password,
                        CLIENT_ID = save.googleClientID,
                        CLIENT_SECRET = save.googleClientSecret
                    };
                    json = JsonUtility.ToJson(saveFile);
                    FileLoader.SaveTextFile("ConfigManager.txt", json);
                    Debug.Log(json);
                }
                else
                {
                    Debug.LogError("Error: " + requestToAPIKey.error);
                }
                requestToAPIKey.Dispose();
            };
        }
    }
}