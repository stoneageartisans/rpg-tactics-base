//using System.Collections.Generic;

public static class Constants
{
    public const float TokenZone = 2.0f;

    public const int StartingPoints = 10;
    public const int StatDefaultValue = 10;
    public const int StatMaxValue = 20;
    public const int StatMinValue = 5;

    public const int Modify = 0;
    public const int View = 1;

    public enum GameState
    {
        MainMenu,
        WarningDialog,
        CharacterCreation,
        Settings,
        ExitDialog,
        PreRound,
        CharacterModification,
        InCombat,
        ViewCharacter,
        PlayerTurn,
        OpponentTurn,
        PostRound
    };

    public enum TacticalStance
    {
        Defensive,
        Balanced,
        Aggressive
    };

    public enum Weapon
    {
        Unarmed
    };
}
