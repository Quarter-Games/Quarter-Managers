using UnityEngine;

namespace QG.Managers.Ads
{
    abstract public class AdsManager : SingletonManager<AdsManager>
    {
    }
    public enum AdsType
    {
        Banner,
        Interstitial,
        Rewarded,
        Native
    }
}
