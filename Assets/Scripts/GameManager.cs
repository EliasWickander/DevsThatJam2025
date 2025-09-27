using CustomToolkit.AdvancedTypes;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected override void OnSingletonDestroy()
    {
        base.OnSingletonDestroy();
        
        GameContext.Clear();
    }
}
