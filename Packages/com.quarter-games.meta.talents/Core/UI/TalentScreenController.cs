using QG.Managers.Economy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class TalentScreenController : MonoBehaviour, IMenuScreenManager
{
    [SerializeField] Transform talentVerticalParent;
    [SerializeField] GameObject talentHorizontalPrefab;
    [SerializeField] TalentHandler talentHandlerPrefab;
    [SerializeField] List<TalentHandler> talentHandlers = new List<TalentHandler>();
    [SerializeField] List<GameObject> horizontalRows = new List<GameObject>();
    [SerializeField] UILineConnector lineConnector;
    [SerializeField] TalentPopUp talentPopUp;

    public bool IsNotification => talentHandlers.Any(x => x.IsBuyable);

    private void OnEnable()
    {
        TalentHandler.OnTalentSelected += OnTalentClick;
        var currencies = TalentsManager.GetTalentLibrary.Select(x => x.CurrencyCost).Distinct().ToList();
        currencies.ForEach(x => x.OnCurrencyChanged += UpdateVisuals);
        Populate(TalentsManager.GetTalentLibrary);
    }


    private void OnDisable()
    {
        var currencies = TalentsManager.GetTalentLibrary.Select(x => x.CurrencyCost).Distinct().ToList();
        currencies.ForEach(x => x.OnCurrencyChanged -= UpdateVisuals);
        TalentHandler.OnTalentSelected -= OnTalentClick;
    }
    void IMenuScreenManager.PerformStateUpdate()
    {
        UpdateVisuals(null, 0);
    }
    private void UpdateVisuals(Currency currency, BigInteger integer)
    {
        foreach (var handler in talentHandlers)
        {
            handler.Init(handler.TalentData, handler.Price);
        }
        (this as IMenuScreenManager).NotifyStateChanged();
    }

    private void OnTalentClick(TalentHandler handler, BigInteger integer)
    {
        talentPopUp.Init(handler, integer);
    }

    private void Populate(TalentLibrary talents)
    {
        int currentIndex = 0;
        foreach (var talent in talents)
        {
            if (!talent) continue;
            currentIndex = talent.CalculateIndex();
            if (horizontalRows.Count < currentIndex + 1)
            {
                var newRow = Instantiate(talentHorizontalPrefab, talentVerticalParent);
                horizontalRows.Add(newRow);
            }
            var talentHandler = Instantiate(talentHandlerPrefab, horizontalRows[currentIndex].transform);
            if (talent.Complexity == TalentComplexity.Simple)
            {
                talentHandler.transform.SetAsFirstSibling();
            }
            else
            {
                talentHandler.transform.SetAsLastSibling();
            }
            var price = CalculatePrice(talent, talents, currentIndex);
            talentHandler.Init(talent, price);
            talentHandlers.Add(talentHandler);
            BuildDependencyVisuals(talent, talentHandler);
        }
    }

    private void BuildDependencyVisuals(Talent talent, TalentHandler talentHandler)
    {
        foreach (var dependency in talent.DependsOn)
        {
            if (dependency == null) continue;
            var dependencyHandler = talentHandlers.FirstOrDefault(th => th.TalentData == dependency);
            if (dependencyHandler != null)
            {
                var currentTransforms = lineConnector.transforms.ToList();
                currentTransforms.Add(dependencyHandler.RectTransform);
                currentTransforms.Add(talentHandler.RectTransform);
                lineConnector.transforms = currentTransforms.ToArray();
            }
        }
    }

    private BigInteger CalculatePrice(Talent talent, TalentLibrary lib, int index)
    {
        BigInteger prevPrice = new(lib.StartPrice * Mathf.Pow(lib.regularPriceMultiplier, index - 1));
        if (talent.Complexity == TalentComplexity.Big)
        {
            return RoundToClosestTen(((float)prevPrice) * lib.bigPriceMultiplier);
        }

        return RoundToClosestTen(((float)prevPrice) * lib.regularPriceMultiplier);
    }
    static private BigInteger RoundToClosestTen(float value)
    {
        return new BigInteger(Math.Round(value / 10) * 10);
    }

}
