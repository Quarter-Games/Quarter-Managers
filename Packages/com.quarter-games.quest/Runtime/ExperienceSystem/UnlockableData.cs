using QG.Managers.Economy.Transactions;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Unlockable", menuName = "Quarter Asset/Experience/Unlockable")]
public class UnlockableData : ScriptableObject
{
    public string UniqueID;
    public CurrencyToGameActionTransaction ToUnlock;
    [Tooltip("Updated Automaticly")] public int RequiredLevel;
    public Action UnlockRequested;
    public RewardTransaction Reward;
    public bool TryUnlock(int currentLevel, bool autoPay = false)
    {
        if (RequiredLevel > currentLevel) return false;
        if (ToUnlock.Cost.ActualValue == 0)
        {
            Unlock();
            return true;
        }
        if (!autoPay) return false;
        if (ToUnlock.IsPossible(null))
        {
            ToUnlock.Execute(null, null);
            return true;
        }
        return false;
    }
    public LockStatus GetStatus(int currentLevel)
    {
        if (RequiredLevel > currentLevel) return LockStatus.Locked;
        if (ToUnlock.Cost.ActualValue == 0) return LockStatus.Unlocked;
        return QG.Managers.SaveSystem.Basic.BasicSaveLoadManager.GetData(UniqueID, LockStatus.Locked);
    }
    public void Unlock()
    {
        if (Reward.Currency != null) Reward.Execute(null, null);
        UnlockRequested?.Invoke();
        QG.Managers.SaveSystem.Basic.BasicSaveLoadManager.SetData(UniqueID, LockStatus.Unlocked);
    }
    public bool IsLevelAvaliable(int currentLevel) => currentLevel >= RequiredLevel;
#if UNITY_EDITOR
    private void OnValidate()
    {
        var path = AssetDatabase.GetAssetPath(this);
        UniqueID = path;
    }
#endif
    [Serializable]
    public enum LockStatus
    {
        Locked,
        Unlocked
    }
}
