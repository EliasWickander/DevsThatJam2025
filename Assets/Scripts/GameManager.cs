using System;
using CustomToolkit.AdvancedTypes;
using CustomToolkit.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Scene]
    [SerializeField]
    private string m_levelScene;

    [SerializeField]
    private Transform[] m_patrolPoints;
    public Transform[] PatrolPoints => m_patrolPoints;

    [SerializeField]
    private SpawnManager m_spawnManager;
    
    private int m_mothsAscended = 0;

    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();

        m_spawnManager = GetComponent<SpawnManager>();
    }

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

    public void OnMothAscended()
    {
        m_mothsAscended++;
        
        if(m_mothsAscended >= m_spawnManager.SmallMothAmount)
            OnAllMothsAscended();
    }

    private void OnAllMothsAscended()
    {
        Debug.Log("You win");
        SceneManager.LoadScene(m_levelScene);
    }
}
