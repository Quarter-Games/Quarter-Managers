using NUnit.Framework;
using QG.Managers.Economy.Transactions;
using QG.Managers.SaveSystem.Basic;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "TestingManagers", menuName = "Testing/TestingManagers")]
public class TestingManagers : MonoBehaviour
{
    private void Awake()
    {
        BasicSaveLoadManager.SetData("test", "test",null);
    }

}