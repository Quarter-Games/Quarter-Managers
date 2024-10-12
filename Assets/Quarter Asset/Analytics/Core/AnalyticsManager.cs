using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

abstract public class AnalyticsManager : SingletonManager<AnalyticsManager>
{
    public const string PlayerConsentKey = "PlayerConsent";
    public static PlayerConsentStatus PlayerConsent { get; protected set; } = PlayerConsentStatus.NotAsked;
    #region Static Methods
    async public static Task<PlayerConsentStatus> GetPlayerConsent()
    {
        var status = await BasicSaveLoadManager.GetData(PlayerConsentKey, PlayerConsent);
        PlayerConsent = status;
        return status;
    }
    public static void UpdatePlayerConsent(PlayerConsentStatus status)
    {
        PlayerConsent = status;
        BasicSaveLoadManager.SetData(PlayerConsentKey, status);
        if (status == PlayerConsentStatus.Granted) StartCollectingData();
        else StopCollectingData();
    }
    public static void LogEvent<T>(string eventName, Dictionary<string, T> eventData)
    {
        if (Instance == null) return;
        if (PlayerConsent != PlayerConsentStatus.Granted) return;
        Instance.StartCoroutine(WaitForAnalyticsToStart(() => Instance.LogNewEvent(eventName, eventData)));
        LogEvent(eventName, eventData);
    }
    public static void StartCollectingData()
    {
        if (Instance == null) return;
        if (PlayerConsent != PlayerConsentStatus.Granted) return;
        Instance.StartCoroutine(WaitForAnalyticsToStart(() => Instance.StartCollecting()));
    }
    public static void StopCollectingData()
    {
        if (Instance == null) return;
        Instance.StopCollecting();
    }
    public static void RequestToDeleteData()
    {
        if (Instance == null) return;
        Instance.StartCoroutine(WaitForAnalyticsToStart(() =>
        {
            Instance.StopCollecting();
            Instance.DeleteData();
        }
        ));
    }
    public static void SendImediate()
    {
        if (Instance == null) return;
        Instance.StartCoroutine(WaitForAnalyticsToStart(() => Instance.SendAllEventsImediate()));
    }
    #endregion
    static private IEnumerator WaitForAnalyticsToStart(Action continueWith)
    {
        yield return new WaitUntil(() => Instance.IsReady());
        continueWith();
    }
    #region Abstract Methods
    abstract public void LogNewEvent<T>(string eventName, Dictionary<string, T> eventData);
    abstract public void StartCollecting();
    abstract public void StopCollecting();
    abstract public void DeleteData();
    abstract public void SendAllEventsImediate();
    #endregion
    public enum PlayerConsentStatus
    {
        NotAsked,
        Denied,
        Granted
    }
}