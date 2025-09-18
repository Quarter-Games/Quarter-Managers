using System.Collections.Generic;
using UnityEngine;

public interface IMetaMediator
{
    protected static List<IMetaMediator> Instances = new();
    public static float GetValue(MetaMediationKey mediationKey)
    {
        var value = mediationKey.DefaultValue;
        foreach (var instance in Instances)
        {
            var instanceValue = instance.GetInstanceValue(mediationKey);
            switch (mediationKey.Operator)
            {
                case MetaMediationOperator.Add:
                    value += instanceValue;
                    break;
                case MetaMediationOperator.Multiply:
                    value *= instanceValue;
                    break;
            }
        }
        return value;
    }
    public float GetInstanceValue(MetaMediationKey mediationKey);
    public void EnableInstance();
    public void DisableInstance();
}
public enum MetaMediationOperator
{
    Add, Multiply
}