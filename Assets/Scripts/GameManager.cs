using CustomToolkit.AdvancedTypes;
using CustomToolkit.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Scene]
    [SerializeField]
    private string m_levelScene;
    
    protected override void OnSingletonDestroy()
    {
        base.OnSingletonDestroy();
        
        GameContext.Clear();
    }
    
    public void GameOver()
    {
        Debug.Log("Game over!");
        SceneManager.LoadScene(m_levelScene);
    }
}
