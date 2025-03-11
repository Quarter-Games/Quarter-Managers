using UnityEngine;

namespace QG.Managers.Economy
{
    [CreateAssetMenu(fileName = "CurrencyIcon", menuName = "Scriptable Objects/CurrencyIcon")]
    public class CurrencyIcon : ScriptableObject
    {
        [SerializeField] private string _name;
        public string Name { get => _name; }
        [SerializeField] private Sprite _icon;
        public Sprite Icon { get => _icon; }
    }
}
