public enum UIScreen
{
    Home,
    Level,
    Gameplay,
    Pause,
    Win,
    Lose,
    UnlockNewLevel,
    Battle
}
public enum PlayerDataKey
{
    UnlockedLevel,
    HighScore,
}
public enum SoundEffect
{
    BG,
    Click,
    Win,
    Lose,
    Buy,
    Arrow,
    BG_2,
    UnlockNewChar,
    Merge
}
public enum UIAnimationState
{
    ShowMainUI,
    HideMainUI,
    ShowOverlayUI,
    HideOverlayUI,
    ButtonClick
}
public enum VisualEffect
{
    None,
    VFX_Confetti
}
public enum GameLevel
{
    Level_1,
    Level_2,
    Level_3,
    Level_4,
    Level_5,
    Level_6,
    Level_7,
    Level_8,
    Level_9,
    Level_11,
    Level_12,
    Level_13,
    Level_14,
    Level_15,
    Level_16,
    Level_17,
    Level_18,
    Level_19,
    Level_20,
}

// Create New Enum Here

public enum EntityLevel
{
    Level_1,
    Level_2,
    Level_3,
    Level_4,
    Level_5,
    Level_6,
    Level_7,
    Level_8,
    Level_9,
    Level_10
}
public enum EntityType
{
    Character,
    Pet,
    Enemy
}
public enum AnimationType
{
    Idle,
    Moving,
    Attaking
}
public enum AttackType
{
    Melee,
    Ranged
}
public class GameDataManager : BaseSingleton<GameDataManager>
{

    public string FormatPrice(long value)
    {
        if (value >= 99_900_000_000)
        {
            return "99.9B";
        }
        else if (value >= 1_000_000_000)
        {
            return (value / 1_000_000_000f).ToString("0.#") + "B";
        }
        else if (value >= 1_000_000)
        {
            return (value / 1_000_000f).ToString("0.#") + "M";
        }
        else if (value >= 10_000)
        {
            return (value / 1_000f).ToString("0.#") + "K";
        }
        return value.ToString();
    }

}
