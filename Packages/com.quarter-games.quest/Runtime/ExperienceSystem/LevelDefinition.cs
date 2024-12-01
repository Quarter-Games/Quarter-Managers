
using QG.Managers.Economy.Transactions;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Quarter Asset/Experience/Level")]
public class LevelDefinition : ScriptableObject
{
    public List<UnlockableData> Unlocks;
    public string BigTitle;
    public string Description;
    public int Level;
    public CurrencyToCurrencyTransaction LevelUpTransaction;
    public Sprite Icon;
    private void OnValidate()
    {
        foreach (UnlockableData lockedData in Unlocks)
        {
            if (lockedData == null) continue;
            lockedData.RequiredLevel = Level;
        }
        if (LevelUpTransaction == null) LevelUpTransaction = new();
    }
    virtual public void LevelUp()
    {
        LevelUpTransaction.Execute(null, null);
        foreach (UnlockableData lockedData in Unlocks)
        {
            if (lockedData == null) continue;
            lockedData.TryUnlock(Level);
        }
    }
}
