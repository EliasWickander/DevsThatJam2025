using System;
using CustomToolkit.AdvancedTypes;
using CustomToolkit.Attributes;
using UnityEngine;
using UnityEngine.Events;
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

    [SerializeField]
    private PlayerAngelLamp m_playerAngelLampPrefab;

    public event Action OnBeginPlayerAscension;

    [SerializeField]
    private LayerMask m_roofLayerMask;

    public UnityEvent OnWon;
    public UnityEvent OnLost;

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
        Debug.Log("You lost");
        Time.timeScale = 0;
        OnLost?.Invoke();
    }

    public void OnMothAscended()
    {
        m_mothsAscended++;
        OnAllMothsAscended();
        if(m_mothsAscended >= m_spawnManager.SmallMothAmount)
            OnAllMothsAscended();
    }

    private void OnAllMothsAscended()
    {
        if (Physics.Raycast(GameContext.Player.transform.position, Vector3.up * 100, out RaycastHit hitInfo, Mathf.Infinity, m_roofLayerMask))
        {
            Instantiate(m_playerAngelLampPrefab, hitInfo.point, Quaternion.Euler(90, 0, 0));   
            OnBeginPlayerAscension?.Invoke();
        }
    }

    public void Win()
    {
        Debug.Log("You win");
        Time.timeScale = 0;
        OnWon?.Invoke();
    }
}
