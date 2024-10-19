using UnityEngine;
using UnityEngine.Advertisements;
using Unity.Services;
using Unity.Services.Core;
using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace QG.Managers.Ads.Legacy
{
    public class LegacyAdsManager : AdsManager, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        public UnityLegacyAdsSettings Settings;
        string gameId => Application.platform == RuntimePlatform.Android ? Settings.AndroidGameId : Settings.IosGameId;
        public bool isTestMode = false;
        private bool isReady = false;
        List<AddUnit> readyAdUnits;
        Dictionary<AddUnit, Action<AdStatusEvent>> CurrentlyShowedAds;
        public override void Init()
        {
            base.Init();
            readyAdUnits = new List<AddUnit>();
            CurrentlyShowedAds = new Dictionary<AddUnit, Action<AdStatusEvent>>();
            TryInitAds();
        }
        private async void TryInitAds()
        {

            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }
            Advertisement.Initialize(gameId, isTestMode, this);
        }
        private void OnInitialized(bool isSuccess)
        {
            isReady = true;
            if (isSuccess)
            {
                LoadAllAds();
            }
        }
        public override bool IsReady()
        {
            return isReady;
        }
        public void LoadAllAds()
        {
            if (Advertisement.isInitialized == false)
            {
                Debug.LogError("Unity Ads is not initialized.");
                return;
            }
            foreach (var addUnit in Application.platform == RuntimePlatform.Android ? Settings.AndroidAddUnits : Settings.IosAddUnits)
            {
                Advertisement.Load(addUnit.AddUnitId, this);
            }
        }
        public void ShowByID(string ID)
        {
            ShowAd(readyAdUnits.Find(x => x.AddUnitId == ID).AdsType);
        }
        public void ShowAd(AdsType type)
        {
            ShowAd(type, null);
        }
        public void ShowAd(AdsType type, Action<AdStatusEvent> OnFinishCallback = null)
        {
            if (readyAdUnits.Count == 0)
            {
                Debug.LogError("No ads are ready to show.");
                return;
            }
            var addUnit = readyAdUnits.Find(x => x.AdsType == type);
            if (addUnit == null)
            {
                Debug.LogError($"No {type} ads are ready to show.");
                return;
            }
            readyAdUnits.Remove(addUnit);
            CurrentlyShowedAds.Add(addUnit, OnFinishCallback);
            Advertisement.Show(addUnit.AddUnitId, this);
        }
        public void ConfigureBunner()
        {

        }

        #region Callbacks
        public void OnInitializationComplete()
        {
            OnInitialized(true);
            Debug.Log("Unity Ads initialized.");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            OnInitialized(false);
            Debug.LogError($"Unity Ads initialization failed: {error.ToString()} - {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log($"Ad loaded: {placementId}");
            readyAdUnits.Add(Application.platform == RuntimePlatform.Android 
                ? Settings.AndroidAddUnits.Find(x => x.AddUnitId == placementId)
                : Settings.IosAddUnits.Find(x => x.AddUnitId == placementId)
                );
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"Ad failed to load: {placementId} - {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.LogError($"Ad failed to show: {placementId} - {error.ToString()} - {message}");
            CurrentlyShowedAds[Settings.AndroidAddUnits.Find(x => x.AddUnitId == placementId)]?.Invoke(AdStatusEvent.OnAdFailedToShow);
            CurrentlyShowedAds.Remove(Settings.AndroidAddUnits.Find(x => x.AddUnitId == placementId));
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log($"Ad started: {placementId}");
            CurrentlyShowedAds[Settings.AndroidAddUnits.Find(x => x.AddUnitId == placementId)]?.Invoke(AdStatusEvent.OnAdStarted);
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log($"Ad clicked: {placementId}");
            CurrentlyShowedAds[Settings.AndroidAddUnits.Find(x => x.AddUnitId == placementId)]?.Invoke(AdStatusEvent.OnAdClicked);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log($"Ad finished: {placementId} - {showCompletionState.ToString()}");
            AdStatusEvent status = showCompletionState == UnityAdsShowCompletionState.COMPLETED ? AdStatusEvent.OnAdFinished : AdStatusEvent.OnAdSkiped;
            CurrentlyShowedAds[Settings.AndroidAddUnits.Find(x => x.AddUnitId == placementId)]?.Invoke(status);
            CurrentlyShowedAds.Remove(Settings.AndroidAddUnits.Find(x => x.AddUnitId == placementId));
        }
        #endregion
    }
}
