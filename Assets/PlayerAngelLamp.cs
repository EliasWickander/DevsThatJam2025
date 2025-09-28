using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerAngelLamp : MonoBehaviour
{
    [SerializeField]
    private UnityEvent m_onTurnOff;

    [SerializeField]
    private float m_deathDelay = 0.5f;
    
    private float m_pendingDeathTimer = 0.0f;
    private bool m_pendingDeath = false;
    
    private float m_ascendTimer = 0.0f;

    private float m_timeToAscend = 3.0f;
    private float AscendProgress => Mathf.Clamp01(m_ascendTimer / m_timeToAscend);
    
    private bool m_hasReachedTarget = false;

    private Vector3 m_playerStartPos;
    private Vector3 m_targetPos;
    private void Start()
    {
        m_playerStartPos = GameContext.Player.transform.position;
        m_targetPos = transform.position - new Vector3(0, 3.5f, 0);
        m_ascendTimer = 0.0f;
    }

    private void Update()
    {
        if(m_pendingDeath)
        {
            m_pendingDeathTimer += Time.deltaTime;
            if(m_pendingDeathTimer >= m_deathDelay)
            {
                ToggleOff();
                m_pendingDeath = false;
                m_pendingDeathTimer = 0.0f;
            }
        }
        else
        {
            HandleAscension(m_targetPos);
        }
    }
    
    private void HandleAscension(Vector3 targetPos)
    {
        if(AscendProgress >= 1.0f)
        {
            m_hasReachedTarget = true;
            OnPlayerAscendComplete();
            return;
        }
        
        GameContext.Player.transform.position = Vector3.Lerp(m_playerStartPos, targetPos, AscendProgress);
        m_ascendTimer += Time.deltaTime;
        
        //RotateHeadTowards(targetPos);
    }
    private void ToggleOff()
    {
        m_onTurnOff?.Invoke();
    }
    
    private void OnPlayerAscendComplete()
    {
        m_pendingDeath = true;
        m_pendingDeathTimer = 0.0f;
        GameManager.Instance.Win();
    }
}
