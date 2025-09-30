using UnityEngine;

public static class GameContext
{
    public static PlayerController Player;
    public static BigMoth BigMoth;
    
    public static void Clear()
    {
        Player = null;
        BigMoth = null;
    }
}
