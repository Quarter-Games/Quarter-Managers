using NUnit.Framework;
using QG.Managers.Economy.Transactions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "TestingManagers", menuName = "Testing/TestingManagers")]
public class TestingManagers : MonoBehaviour
{
    [SerializeReference]
    public List<Transaction> upgrades;

    [SerializeReference]
    public Transaction upgrade;

}