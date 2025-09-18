using QG.Managers;
using UnityEngine;

public class TalentsManager : SingletonManager<TalentsManager>, IMetaMediator
{
    [SerializeField] TalentLibrary talentLibrary = null;
    public override bool IsReady()
    {
        return true;
    }

    public float GetInstanceValue(MetaMediationKey mediationKey)
    {
        float value = mediationKey.Operator == MetaMediationOperator.Add ? 0 : 1;
        foreach (var talent in talentLibrary)
        {
            if (talent.IsUnlocked && talent.MetaMediatorKey == mediationKey)
            {
                switch (mediationKey.Operator)
                {
                    case MetaMediationOperator.Add:
                        value += talent.Value;
                        break;
                    case MetaMediationOperator.Multiply:
                        value = (value == 0f) ? talent.Value : value * talent.Value;
                        break;
                }
            }
        }
        return value;
    }
    public override void Init()
    {
        base.Init();
        EnableInstance();
    }
    private void OnDestroy()
    {
        DisableInstance();
    }
    public void EnableInstance()
    {
        IMetaMediator.Instances.Add(this);
    }

    public void DisableInstance()
    {
        IMetaMediator.Instances.Remove(this);
    }

    public static TalentLibrary GetTalentLibrary => Instance.talentLibrary;
}
