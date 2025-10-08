using UnityEngine;
using TMPro;
using UnityEngine.UI;
using QG.Managers.Economy;
using QG.Managers.Economy.Transactions;
using UnityEngine.InputSystem;
using BigInteger = System.Numerics.BigInteger;
public class TalentPopUp : MonoBehaviour
{
    [SerializeField] TalentHandler TalentHandlerRef;
    [SerializeField] Talent TalentData;
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Description;
    [SerializeField] Button BuyButton;
    [SerializeField] TMP_Text Cost;
    [SerializeField] Transaction currentTransaction;
    [SerializeField] RectTransform arrow;
    [SerializeField] InputActionReference Pointer;
    [SerializeField] InputActionReference ClickAction;
    [SerializeField] Camera mainCamera;
    private void OnEnable()
    {
        ClickAction.action.performed += OnClickOutside;
        if (mainCamera == null) mainCamera = Camera.main;

    }
    private void OnDisable()
    {
        ClickAction.action.performed -= OnClickOutside;
    }
    private void OnClickOutside(InputAction.CallbackContext context)
    {
        var position = Pointer.action.ReadValue<Vector2>();
        if (!RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, position, mainCamera))
        {
            gameObject.SetActive(false);
        }
    }
    public void OnClick()
    {
        if (currentTransaction != null && currentTransaction.IsPossible(null))
        {
            TalentData.Unlock();
            currentTransaction.Execute(null, null);
            BuyButton.interactable = false;
        }
        gameObject.SetActive(false);
    }
    public void Init(TalentHandler talent, BigInteger cost)
    {
        gameObject.SetActive(true);
        TalentHandlerRef = talent;
        TalentData = talent.TalentData;
        Title.text = TalentData.TalentName;
        currentTransaction = new CurrencyToGameActionTransaction()
        {
            Cost = cost,
            ReducedCurrency = TalentData.CurrencyCost,
        };
        BuyButton.interactable = currentTransaction.IsPossible(null);
        Cost.text = new CurrencyValue(cost).GetStringValue();
        var pivot = TalentData.Complexity == TalentComplexity.Big ? new Vector2(1, 0.5f) : new Vector2(0, 0.5f);
        (transform as RectTransform).pivot = pivot;
        Vector3 offset = (TalentData.Complexity == TalentComplexity.Big ? Vector3.left : Vector3.right) * (TalentHandlerRef.RectTransform.rect.width / 2);
        transform.SetParent(TalentHandlerRef.transform);
        transform.localScale = new Vector3(1 / talent.transform.localScale.x, 1 / talent.transform.localScale.y);
        Description.text = TalentData.Description;
        transform.localPosition = offset;
        arrow.position = transform.position;
        arrow.rotation = talent.transform.position.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }

}
