using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using QuarterAsset.SaveSystem;
using Unity.VisualScripting;

namespace QuarterAsset
{
    public class GameManager : MonoBehaviour
    {
        public event Action<float> LoadingProgress;
        public static GameManager Instance;
        [SerializeField] string mainMenuSceneName;
        [SerializeField] List<SingletonManager> managersToLoad;
        [SerializeField] float MinLoadingTime = 1f;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        void Start()
        {
            StartCoroutine(LoadMainMenu(mainMenuSceneName));
        }
        public IEnumerator LoadMainMenu(string sceneName)
        {
            var sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName);
            sceneLoadingOperation.allowSceneActivation = false;
            yield return new WaitUntil(() => sceneLoadingOperation.progress >= 0.8);
            LoadingProgress?.Invoke(0.5f);
            yield return new WaitUntil(() => Time.time >= MinLoadingTime);
            int readyManagers = 0;
            while (!managersToLoad.All(x => x.IsReady()))
            {
                readyManagers = managersToLoad.Count(x => x.IsReady());
                LoadingProgress?.Invoke(0.5f + (readyManagers * 0.5f) / managersToLoad.Count);
                Debug.Log("Progress: " + (0.5f + (readyManagers * 0.5f) / managersToLoad.Count));
                Debug.Log("Ready Managers: " + readyManagers);
                yield return new WaitUntil(() => readyManagers != managersToLoad.Count(x => x.IsReady()));
            }
            sceneLoadingOperation.allowSceneActivation = true;
        }
    }
}