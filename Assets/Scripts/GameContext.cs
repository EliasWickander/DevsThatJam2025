using UnityEngine;

public static class GameContext
{
    public static PlayerController Player;
    
    public static void Clear()
    {
        Player = null;
    }
}
