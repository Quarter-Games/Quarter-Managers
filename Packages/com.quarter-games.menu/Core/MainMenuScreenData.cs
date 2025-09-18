using UnityEngine;
[CreateAssetMenu(fileName = "MainMenuScreenData", menuName = "QG/Menu/MainMenuScreenData", order = 1)]
public class MainMenuScreenData : ScriptableObject
{
    public string SceneName;
    public Sprite Icon;
    public string Title;
    public FeatureData RequiredFeature;
}