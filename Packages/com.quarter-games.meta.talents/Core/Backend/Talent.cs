using QG.Managers.Economy;
using QG.Managers.SaveSystem.Basic;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Talent", menuName = "TD/Meta/" + "New Talent")]
public class Talent : ScriptableObject
{
    public const string SAVE_KEY = "Talent_";
    public const string SO_NAME_PREFIX = "TD/Meta/Talents/";
    [SerializeField] string talentName = "New Talent";
    [SerializeField] TalentComplexity complexity = TalentComplexity.Simple;
    [TextArea, SerializeField] string description = "Talent Description";
    [SerializeField] float value = 1f;
    [SerializeField] Sprite icon = null;
    public List<Talent> DependsOn = new();
    [SerializeField] Currency Currency;
    [SerializeField] MetaMediationKey metaMediationKey = null;

    public string TalentName { get => talentName; set => talentName = value; }
    public TalentComplexity Complexity { get => complexity; set => complexity = value; }
    public string Description { get => description; set => description = value; }
    public float Value { get => value; set => this.value = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public Currency CurrencyCost { get => Currency; set => Currency = value; }
    public MetaMediationKey MetaMediatorKey { get => metaMediationKey; set => metaMediationKey = value; }

    public int CalculateIndex()
    {
        if (DependsOn.Count == 0) return 0;
        return DependsOn[0].CalculateIndex() + 1;
    }
    public void Unlock()
    {
        BasicSaveLoadManager.SetData(SAVE_KEY + talentName + CalculateIndex(), true);
        BasicSaveLoadManager.SaveAllClusters();
    }
    public bool IsUnlocked => BasicSaveLoadManager.GetData(SAVE_KEY + talentName + CalculateIndex(), false);
}
public enum TalentComplexity
{
    Simple,
    Big
}
