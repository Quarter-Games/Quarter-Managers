using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace QG.Managers.Ads.Legacy
{
    [CreateAssetMenu(fileName = "UnityLegacyAdsSettings", menuName = "Quarter Asset/Ads/Unity Legacy Ads Settings")]
    public class UnityLegacyAdsSettings : ScriptableObject
    {
        public string AndroidGameId;
        public string IosGameId;
        public List<AddUnit> AndroidAddUnits;
        public List<AddUnit> IosAddUnits;
    }
    [System.Serializable]
    public class  AddUnit
    {
        public string AddUnitId;
        public AdsType AdsType;
    }
    [System.Serializable]
    public enum AdStatusEvent
    {
        OnAdFailedToShow,
        OnAdStarted,
        OnAdClicked,
        OnAdFinished,
        OnAdSkiped
    }
}
