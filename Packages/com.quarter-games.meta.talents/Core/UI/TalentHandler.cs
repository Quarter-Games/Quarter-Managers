using QG.Managers.Economy;
using BigInteger = System.Numerics.BigInteger;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using QG.Managers.Economy.Transactions;
using MoreMountains.Feedbacks;
using System.Collections;

public class TalentHandler : MonoBehaviour
{
    public static event Action<TalentHandler, BigInteger> OnTalentSelected;
    [SerializeField] Image Icon;
    [SerializeField] Talent _talentData;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] GameObject UnlockedIcon;
    [SerializeField] GameObject BuyableIcon;
    [SerializeField] MMF_Player unlockFeedback;
    [SerializeField] Sprite normalState;
    [SerializeField] Sprite lockedState;
    [SerializeField] Sprite LockIcon;
    [SerializeField] Image parent;
    [ColorUsage(true)][SerializeField] Color buyedColor;
    [SerializeField] ParticleSystem OnBecameBuyable;
    public Talent TalentData => _talentData;
    public RectTransform RectTransform => rectTransform;
    public SerializedBigInteger Price;
    [SerializeField] Button button;
    Transaction _transaction;
    public bool IsBuyable => (_transaction?.IsPossible(null)).GetValueOrDefault();
    public void Init(Talent talent, BigInteger price)
    {
        Icon.sprite = talent.Icon;
        _talentData = talent;
        if (talent.Complexity == TalentComplexity.Big)
        {
            transform.localScale = Vector3.one * 1.3f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
        Price = price;
        if (talent.IsUnlocked)
        {
            SetUpUnclockedState();
        }
        else if (talent.DependsOn.All(x => x.IsUnlocked))
        {
            SetUpBuyableState();
        }
        else SetUpNonBuyableState();
    }
    private void SetUpUnclockedState()
    {
        StartCoroutine(waitToFeedback());
        IEnumerator waitToFeedback()
        {
            bool ShouldPlayFeedback = _talentData != null && BuyableIcon.activeInHierarchy && _talentData.IsUnlocked;
            BuyableIcon.SetActive(false);
            if (ShouldPlayFeedback)
                yield return unlockFeedback.PlayFeedbacksCoroutine(Vector3.one);

            button.interactable = false;
            UnlockedIcon.SetActive(true);
            parent.sprite = normalState;
            Icon.color = buyedColor;
            parent.color = buyedColor;
        }

    }
    private void SetUpBuyableState()
    {
        _transaction = new CurrencyToGameActionTransaction()
        {
            Cost = Price,
            ReducedCurrency = TalentData.CurrencyCost,
        };
        if (!button.interactable && OnBecameBuyable)
        {
            OnBecameBuyable.Play();
        }
        button.interactable = true;
        BuyableIcon.SetActive(_transaction.IsPossible(null));
        parent.sprite = normalState;
        Icon.color = Color.white;
        parent.color = Color.white;
    }

    private void SetUpNonBuyableState()
    {
        button.interactable = false;
        BuyableIcon.SetActive(false);
        parent.sprite = lockedState;
        Icon.sprite = LockIcon;
        Icon.color = Color.white;
        parent.color = Color.white;
    }

    public void OnClick()
    {
        OnTalentSelected?.Invoke(this, Price);
    }
}

