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
    public class GameManager : SingletonManager<GameManager>
    {
        public event Action<float> LoadingProgress;
        [SerializeField] string mainMenuSceneName;
        [SerializeField] List<SingletonManager> managersToLoad;
        [SerializeField] float MinLoadingTime = 1f;
        void Start()
        {
            Init();
        }
        internal override void Init()
        {
            base.Init();
            StartCoroutine(LoadMainMenu(mainMenuSceneName));

        }
        public IEnumerator LoadMainMenu(string sceneName)
        {
            var sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName);
            sceneLoadingOperation.allowSceneActivation = false;
            int readyManagers = 0;
            for (int i = 0; i<managersToLoad.Count; i++)
            {
                var manager = managersToLoad[i];
                manager.Init();
                yield return new WaitUntil(() => manager.IsReady());
                if (manager.IsReady())
                {
                    readyManagers++;
                }
                LoadingProgress?.Invoke(0.5f + (readyManagers * 0.5f) / managersToLoad.Count);
                Debug.Log("Progress: " + (0.5f + (readyManagers * 0.5f) / managersToLoad.Count));
                Debug.Log(manager.name +" is ready!");
            }
            yield return new WaitUntil(() => sceneLoadingOperation.progress >= 0.8);
            LoadingProgress?.Invoke(0.5f);
            yield return new WaitUntil(() => Time.time >= MinLoadingTime);
            sceneLoadingOperation.allowSceneActivation = true;
        }

        public override bool IsReady() => managersToLoad.All(x => x.IsReady());
    }
}