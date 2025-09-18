using UnityEngine;

[CreateAssetMenu(fileName = "Meta Mediation Key", menuName = "TD/Meta/MetaMediationKey", order = 0)]
public class MetaMediationKey : ScriptableObject
{
    [SerializeField] MetaMediationOperator @operator = MetaMediationOperator.Add;
    public MetaMediationOperator Operator => @operator;
    [SerializeField] float defaultValue = 0f;
    public float DefaultValue => defaultValue;

}
