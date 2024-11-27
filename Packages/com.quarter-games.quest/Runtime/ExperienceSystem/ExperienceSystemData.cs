using QG.Managers.Economy;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExperienceSystemData", menuName = "Quarter Asset/Experience/System Data", order = 1)]
public class ExperienceSystemData : ScriptableObject
{
    public List<LevelDefinition> Levels;
    public Currency ExpCurrency;
    public Currency LevelCurrency;
    public LevelDefinition GetLevelData()
    {
        return Levels.Find(x => x.Level == LevelCurrency.GetAmount());
    }
    public LevelDefinition GetNextLevelData()
    {
        return Levels.Find(x => x.Level == LevelCurrency.GetAmount() + 1);
    }
}
