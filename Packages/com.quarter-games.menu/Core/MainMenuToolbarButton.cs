using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuToolbarButton : MonoBehaviour
{
    public static event Action<MainMenuToolbarButton> OnButtonClicked;
    public static event Action<MainMenuToolbarButton, Canvas> OnFeatureUnlockedEvent;
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] TMPro.TMP_Text buttonText;
    [SerializeField] Image Icon;
    [SerializeField] Sprite LockIcon;
    [SerializeField] Button button;
    [SerializeField] Canvas OverrideSortingCanvas;
    [SerializeField] GameObject Notification;
    MainMenuScreenData _mainMenuScreenData;
    public MainMenuScreenData MainMenuScreen=> _mainMenuScreenData;
    IMenuScreenManager _buttonManager;
    private void OnEnable()
    {
        FeatureData.OnFeatureUnlocked += OnFeatureUnlocked;
        IMenuScreenManager.OnNotificationRecived += IMenuScreenManager_OnNotificationRecived;
    }

    private void IMenuScreenManager_OnNotificationRecived(IMenuScreenManager obj)
    {
        if (_buttonManager == obj)
        {
            SetNotificationState();
        }
    }

    private void OnDisable()
    {
        FeatureData.OnFeatureUnlocked -= OnFeatureUnlocked;
        IMenuScreenManager.OnNotificationRecived -= IMenuScreenManager_OnNotificationRecived;
    }
    private void OnFeatureUnlocked(FeatureData data)
    {
        if (!_mainMenuScreenData || _mainMenuScreenData.RequiredFeature != data) return;
        button.interactable = true;
        Icon.sprite = _mainMenuScreenData.Icon;
        OnFeatureUnlockedEvent?.Invoke(this, OverrideSortingCanvas);
        SetNotificationState(true);
    }

    public void Init(bool isSelected, MainMenuScreenData Data = null, IMenuScreenManager manager = default)
    {
        if (Data)
        {
            Icon.sprite = Data.Icon;
            buttonText.text = Data.Title;
            if (Data.RequiredFeature && !Data.RequiredFeature.IsUnlocked)
            {
                button.interactable = false;
                Icon.sprite = LockIcon;
            }
            _mainMenuScreenData = Data;
        }
        if (isSelected)
        {
            InitForSelected();
        }
        else
        {
            InitForUnselected();
        }
        if (manager != default)
        {
            _buttonManager = manager;
            _buttonManager.PerformStateUpdate();
            SetNotificationState();
        }
    }
    private void SetNotificationState(bool forseTrue = false)
    {
        if (forseTrue)
        {
            Notification.SetActive(true);
            return;
        }
        var isManagerNull = _buttonManager == null;
        var isFeatureNull = _mainMenuScreenData == null || _mainMenuScreenData.RequiredFeature == null;
        var isFeatureUnlocked = !isFeatureNull && _mainMenuScreenData.RequiredFeature.IsUnlocked;
        var isNotification = !isManagerNull && isFeatureUnlocked && _buttonManager.IsNotification;
        Notification.SetActive(isNotification);
    }
    private void InitForUnselected()
    {
        layoutElement.layoutPriority = -1;
        buttonText.gameObject.SetActive(false);
    }

    private void InitForSelected()
    {
        layoutElement.layoutPriority = 2;
        buttonText.gameObject.SetActive(true);
    }

    public void OnClick()
    {
        OnButtonClicked?.Invoke(this);
        SetNotificationState();
    }

    public void AfterMoveCallback()
    {
        _buttonManager.PerformStateUpdate();
    }
}
