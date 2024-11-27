using System;
using UnityEngine;

namespace QG.Managers
{
    [CreateAssetMenu(fileName = "PopUpSettings", menuName = "Quarter Asset/Generic/PopUp Settings")]
    public class PopUpSettings : ScriptableObject
    {
        public event Action OnPopUpSettingsChanged;
        public PopUpData Data;
        private void OnValidate()
        {
            OnPopUpSettingsChanged?.Invoke();
        }
    }
}
