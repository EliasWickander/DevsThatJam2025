using System;
using System.Collections.Generic;
using CustomToolkit.AdvancedTypes;
using CustomToolkit.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private AudioSource m_ambienceAudioSource;
    
    [Header("Ambience Clips")]
    [SerializeField]
    private AudioClip m_defaultAmbienceClip;
    
    [SerializeField]
    private AudioClip m_chaseAmbienceClip;

    [SerializeField]
    private float m_chaseVolume = 1.0f;
    
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

    private void Start()
    {
        SetAmbienceSound(m_defaultAmbienceClip);
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

    public void OnChaseStart()
    {
        SetAmbienceSound(m_chaseAmbienceClip, m_chaseVolume);
    }
    
    public void OnChaseEnd()
    {
        SetAmbienceSound(m_defaultAmbienceClip);
    }

    private void SetAmbienceSound(AudioClip clip, float volume = 1.0f)
    {
        m_ambienceAudioSource.clip = clip;
        
        if(m_ambienceAudioSource.clip != null)
            m_ambienceAudioSource.Play();
        else
            m_ambienceAudioSource.Stop();
    }
}
