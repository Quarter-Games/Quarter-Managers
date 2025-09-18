using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace QG.Managers.Analytics.UnityAnalytics
{
    [AddComponentMenu("QG/Managers/Unity Analytics Manager")]
    internal class UnityAnalyticsManager : AnalyticsManager
    {
        [SerializeField] private bool _isReady = false;
        override public bool IsReady() => _isReady;
        public override void Init()
        {
            base.Init();
            AwaitForConsent();
        }

        private async void AwaitForConsent()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }
            if (await GetPlayerConsent() == PlayerConsentStatus.NotAsked) AskToConsent();
            else _isReady = true;
        }
        public virtual void AskToConsent()
        {

        }
        public void Consent(bool consent)
        {
            if (consent) UpdatePlayerConsent(PlayerConsentStatus.Granted);
            else UpdatePlayerConsent(PlayerConsentStatus.Denied);
            _isReady = true;
        }
        override public void LogNewEvent<T>(string eventName, Dictionary<string, T> eventData)
        {
            var ev = new CustomEvent(eventName);
            foreach (var item in eventData)
            {
                ev.Add(item.Key, item.Value);
            }
            AnalyticsService.Instance.RecordEvent(ev);
        }
        public override void StartCollecting()
        {
            AnalyticsService.Instance.StartDataCollection();
        }
        public override void StopCollecting()
        {
            AnalyticsService.Instance.StopDataCollection();
        }
        public override void DeleteData()
        {
            AnalyticsService.Instance.RequestDataDeletion();
        }
        public override void SendAllEventsImediate()
        {
            AnalyticsService.Instance.Flush();
        }
    }
}