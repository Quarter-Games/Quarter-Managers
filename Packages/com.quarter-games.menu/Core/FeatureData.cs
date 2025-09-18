using QG.Managers.SaveSystem.Basic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature Data", menuName = "QG/Feature/Feature Data")]
public class FeatureData : ScriptableObject
{
    public static event Action<FeatureData> OnFeatureUnlocked;
    public const string FEATURE_UNLOCKED_KEY = "FEATURE_UNLOCKED_KEY";
    [SerializeField] string featureName = "New Feature";
    public bool IsUnlocked
    {
        get => BasicSaveLoadManager.GetData(FEATURE_UNLOCKED_KEY + featureName, false);
        set
        {
            BasicSaveLoadManager.SetData(FEATURE_UNLOCKED_KEY + featureName, value);
            BasicSaveLoadManager.SaveAllClusters();
            OnFeatureUnlocked?.Invoke(this);
        }
    }
}
