using QG.Managers;
using QG.Managers.Economy;
using System;

public class ExperienceManager : SingletonManager<ExperienceManager>
{
    public static event Action<LevelDefinition> OnLevelUp;
    public ExperienceSystemData ExperienceSystemData;
    public override bool IsReady() => true;
    public override void Init()
    {
        base.Init();
        RecursiveLevelUp();
        ExperienceSystemData.ExpCurrency.OnCurrencyChanged += GiveExp;
    }
    private void OnDestroy()
    {
        ExperienceSystemData.ExpCurrency.OnCurrencyChanged -= GiveExp;
    }

    private void GiveExp(Currency currency, float arg2)
    {
        if (arg2 > 0) RecursiveLevelUp();
    }

    public static LevelDefinition GetCurrentLevelData()
    {
        if (Instance == null)
        {
            LoadFallBackManager();
        }
        return Instance.ExperienceSystemData.GetLevelData();
    }
    public static LevelDefinition GetNextLevelData()
    {
        if (Instance == null)
        {
            LoadFallBackManager();
        }
        return Instance.ExperienceSystemData.GetNextLevelData();
    }
    public static int GetPlayerExp()
    {
        if (Instance == null)
        {
            LoadFallBackManager();
        }
        return Instance.ExperienceSystemData.ExpCurrency.GetAmount();
    }
    public bool CheckLevelUp()
    {
        var currentLevel = ExperienceSystemData.GetLevelData();
        return currentLevel.LevelUpTransaction.IsPossible(null);
    }
    public void RecursiveLevelUp()
    {
        if (CheckLevelUp())
        {
            var currentLevel = ExperienceSystemData.GetLevelData();
            currentLevel.LevelUp();
            OnLevelUp?.Invoke(ExperienceSystemData.GetLevelData());
            RecursiveLevelUp();
        }

    }
}
