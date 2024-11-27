using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using QG.Managers;

public class GenericPopUp : MonoBehaviour
{
    [SerializeField] TMP_Text Description;
    [SerializeField] TMP_Text Title;
    [SerializeField] Image Image;
    [SerializeField] ButtonSetUpData LeftButton;
    [SerializeField] ButtonSetUpData RightButton;
    [SerializeField] Button CloseButton;
    [SerializeField] Canvas PopUpCanvas;
    [SerializeField] Image BackGround;
    [SerializeField] Sprite PositiveButtonSprite;
    [ColorUsage(true)][SerializeField] Color PositiveTextColor;
    [SerializeField] Sprite NegativeButtonSprite;
    [ColorUsage(true)][SerializeField] Color NegativeTextColor;
    [SerializeField] Sprite NeutralButtonSprite;
    [ColorUsage(true)][SerializeField] Color NeutralTextColor;
    [SerializeField] Sprite NonInteractableSprite;
    [ColorUsage(true)][SerializeField] Color NonInteractableTextColor;
    private PopUpSettings _data;
    virtual public void Init(PopUpSettings data)
    {
        data.OnPopUpSettingsChanged += () => UpdateData(data);
        UpdateData(data);

    }
    public void UpdateData(PopUpSettings data)
    {
        if (data == null)
        {
            Close();
            return;
        }
        else gameObject.SetActive(true);
        _data = data;
        Title.text = data.Data.Title;
        Description.text = data.Data.Description;
        Image.sprite = data.Data.Image;
        Image.gameObject.SetActive(data.Data.Image != null);
        PopUpCanvas.sortingOrder = data.Data.Priority;
        BackGround.color = new Color(BackGround.color.r, BackGround.color.g, BackGround.color.b, data.Data.BackGroundAlpha);

        CloseButton.gameObject.SetActive(data.Data.isClosable);

        SetUpButton(LeftButton, data.Data.LeftButton);
        SetUpButton(RightButton, data.Data.RightButton);
    }
    virtual protected void SetUpButton(ButtonSetUpData buttonData, PopUpButton settings)
    {
        if (settings == null)
        {
            buttonData.button.gameObject.SetActive(false);
            return;
        }
        if (settings.IsInteractable != null && !settings.IsInteractable.Invoke())
        {
            buttonData.button.image.sprite = NonInteractableSprite;
            buttonData.text.color = NonInteractableTextColor;
        }
        buttonData.button.gameObject.SetActive(true);

        buttonData.text.text = settings.Text;
        buttonData.text.gameObject.SetActive(!string.IsNullOrEmpty(settings.Text));

        buttonData.Icon.sprite = settings.Icon;
        buttonData.Icon.gameObject.SetActive(settings.Icon != null);

        buttonData.LayoutGroup.reverseArrangement = settings.IsIconOnLeft;

        (buttonData.button.image.sprite, buttonData.text.color) = settings.Type switch
        {
            PopUpButtonType.Positive => (PositiveButtonSprite, PositiveTextColor),
            PopUpButtonType.Negative => (NegativeButtonSprite, NegativeTextColor),
            PopUpButtonType.Neutral => (NeutralButtonSprite, NeutralTextColor),
            _ => (PositiveButtonSprite, PositiveTextColor)
        };
    }
    virtual public void LeftButtonClick()
    {
        _data.Data.LeftButton.OnPress?.Invoke();
        Close();
    }
    virtual public void RightButtonClick()
    {
        _data.Data.RightButton.OnPress?.Invoke();
        Close();
    }
    virtual public void CloseButtonClick()
    {
        _data.Data.OnCloseClick?.Invoke();
        Close();
    }
    virtual public void ClickOnBackground()
    {
        if (_data.Data.isBackgroundClosable)
        {
            _data.Data.OnCloseClick?.Invoke();
            Close();
        }
    }
    virtual public void Close()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        _data.OnPopUpSettingsChanged -= () => UpdateData(_data);
    }
    [Serializable]
    public class ButtonSetUpData
    {
        public Button button;
        public TMP_Text text;
        public Image Icon;
        public HorizontalOrVerticalLayoutGroup LayoutGroup;
    }
}
[Serializable]
public class PopUpData
{
    public string Title;
    [Multiline] public string Description;
    public Sprite Image;
    public bool isClosable;
    public bool isBackgroundClosable;
    public Action OnCloseClick;
    public PopUpButton LeftButton;
    public PopUpButton RightButton;
    public int Priority;
    [Range(0, 1f)] public float BackGroundAlpha = 0.66f;
}
[Serializable]
public class PopUpButton
{
    public PopUpButtonType Type;
    public string Text;
    public Action OnPress;
    public Func<bool> IsInteractable;
    public Sprite Icon;
    public bool IsIconOnLeft;
    private PopUpButton() { }
    public PopUpButton(
        PopUpButtonType type,
        string text,
        Action onPress,
        Sprite icon = default,
        bool isIconOnLeft = false,
        Func<bool> isInteractable = default)
    {
        Type = type;
        Text = text;
        OnPress = onPress;
        if (isInteractable == default)
        {
            IsInteractable = () => true;
        }
        else IsInteractable = isInteractable;
        Icon = icon;
        IsIconOnLeft = isIconOnLeft;
    }
}
public enum PopUpButtonType
{
    Positive,
    Negative,
    Neutral
}
