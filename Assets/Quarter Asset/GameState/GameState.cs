
[System.Serializable]
public class GameState
{
    virtual public float GameTimeScale { get; } = 1;
    virtual public float UnityTimeScale { get; } = 1;
}