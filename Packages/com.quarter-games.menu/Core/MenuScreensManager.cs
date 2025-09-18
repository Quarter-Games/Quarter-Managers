using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreensManager : MonoBehaviour
{
    public static MenuScreensManager Instance;
    [SerializeField] List<MainMenuScreenData> SceneNames = new();
    [SerializeField] List<RectTransform> menuScreens = new();
    [SerializeField] List<MainMenuToolbarButton> toolBarButtons = new();
    public CanvasScaler MainMenuCanvas;
    [SerializeField] Transform ToolbarParent;
    [SerializeField] MainMenuToolbarButton MainMenuToolbarButtonPrefab;
    [SerializeField] RectTransform ScreensParent;
    [SerializeField] Canvas TutorialOverlayForFeatureUnlock;
    [SerializeField] RectTransform TutorialArrow;
    int currentScreenIndex;
    bool isInTutorial = false;
    private void OnEnable()
    {
        MainMenuToolbarButton.OnButtonClicked += OnToolbarButtonClicked;
        MainMenuToolbarButton.OnFeatureUnlockedEvent += OnFeatureUnlockedEvent;
    }
    private void OnDisable()
    {
        MainMenuToolbarButton.OnButtonClicked -= OnToolbarButtonClicked;
        MainMenuToolbarButton.OnFeatureUnlockedEvent -= OnFeatureUnlockedEvent;
    }

    private void OnFeatureUnlockedEvent(MainMenuToolbarButton button, Canvas canvas)
    {
        TutorialOverlayForFeatureUnlock.gameObject.SetActive(true);
        canvas.overrideSorting = true;
        canvas.sortingOrder = TutorialOverlayForFeatureUnlock.sortingOrder + 1;
        canvas.sortingLayerName = TutorialOverlayForFeatureUnlock.sortingLayerName;
        isInTutorial = true;
        TutorialArrow.position = button.transform.position;
        StartCoroutine(WaitForPlayerToClick());
        IEnumerator WaitForPlayerToClick()
        {
            yield return new WaitUntil(() => !isInTutorial);
            canvas.overrideSorting = false;
            TutorialOverlayForFeatureUnlock.gameObject.SetActive(false);
        }
    }

    public IEnumerator LoadScreens()
    {
        Instance = this;
        int amount = SceneNames.Count;
        int centerIndex = amount / 2;
        for (int i = 0; i < SceneNames.Count; i++)
        {
            var asyncHandler = SceneManager.LoadSceneAsync(SceneNames[i].SceneName, LoadSceneMode.Additive);
            yield return new WaitUntil(() => asyncHandler.isDone);
            var screen = SceneManager.GetSceneByName(SceneNames[i].SceneName).GetRootGameObjects()[0].transform.GetChild(0).transform as RectTransform;
            var manager = screen.GetComponent<IMenuScreenManager>();
            SceneManager.MergeScenes(screen.gameObject.scene, gameObject.scene);
            var prevParent = screen.parent;
            screen.SetParent(ScreensParent, false);
            Destroy(prevParent.gameObject);
            menuScreens.Add(screen);
            var deltaFromCenter = i - centerIndex;
            screen.anchoredPosition = new Vector2(deltaFromCenter * MainMenuCanvas.referenceResolution.x, 0);
            var button = Instantiate(MainMenuToolbarButtonPrefab, ToolbarParent);
            button.Init(i == centerIndex, SceneNames[i], manager);
            toolBarButtons.Add(button);

        }
        currentScreenIndex = menuScreens.Count / 2;
        UpdateButtons(currentScreenIndex);
    }

    private void OnToolbarButtonClicked(MainMenuToolbarButton button)
    {
        int newIndex = toolBarButtons.IndexOf(button);
        UpdateButtons(newIndex);
        SetUpScreens(newIndex);
        isInTutorial = false;
    }

    private void SetUpScreens(int selectedIndex)
    {
        var deltaPosition = menuScreens[selectedIndex].anchoredPosition.x;
        foreach (RectTransform screen in menuScreens)
        {
            screen.DOKill();
            var tween = screen.DOLocalMoveX(screen.anchoredPosition.x - deltaPosition, 0.5f).SetEase(Ease.OutCubic);
            tween.OnComplete(() =>
            {
                toolBarButtons[selectedIndex].AfterMoveCallback();
            });
        }
    }

    private void UpdateButtons(int selectedIndex)
    {
        for (int i = 0; i < toolBarButtons.Count; i++)
        {
            bool isSelected = i == selectedIndex;
            toolBarButtons[i].Init(isSelected);
        }
    }

}
