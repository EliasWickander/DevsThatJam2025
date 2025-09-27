using System;
using System.Collections.Generic;
using CustomToolkit.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public enum ESmallMothState
{
    State_Idle,
    State_MoveTowardsLight,
}

public class SmallMoth : MonoBehaviour
{
    [SerializeField]
    private float m_turnRate = 5.0f;
    public float TurnRate => m_turnRate;
    
    [SerializeField]
    private float m_lightDetectionRadius = 10f;
    public float LightDetectionRadius => m_lightDetectionRadius;
    
    [SerializeField]
    private float m_lightFollowDistanceThreshold = 1.0f;
    public float LightFollowDistanceThreshold => m_lightFollowDistanceThreshold;
    
    [SerializeField]
    private NavMeshAgent m_navmeshAgent;
    public NavMeshAgent NavmeshAgent => m_navmeshAgent;
    
    private StateMachine m_stateMachine;
    public StateMachine StateMachine => m_stateMachine;

    private Light m_currentLightTarget;
    public Light CurrentLightTarget => m_currentLightTarget;
    
    private void Awake()
    {
        m_navmeshAgent.updateRotation = false;
        
        Dictionary<Enum, State> states = new Dictionary<Enum, State>()
        {
            {ESmallMothState.State_Idle, new State_Idle(this)},
            {ESmallMothState.State_MoveTowardsLight, new State_MoveTowardsLight(this)},
        };
        
        m_stateMachine = new StateMachine(states);
    }

    private void Update()
    {
        UpdateLightTarget();
        
        if(m_stateMachine != null)
            m_stateMachine.Update();
    }
    
    private void UpdateLightTarget()
    {
        Light targetLight = null;
        float bestScore = 0f;
        
        foreach (Light activeLight in LightManager.Instance.ActiveLights)
        {
            if (!activeLight.enabled || activeLight.intensity <= 0f)
                continue;

            Vector3 dirToLightXZ = activeLight.transform.position - transform.position;
            dirToLightXZ.y = 0;
            if (dirToLightXZ.sqrMagnitude > m_lightDetectionRadius * m_lightDetectionRadius)
                continue;
            
            float distanceToLight = dirToLightXZ.magnitude;
            float score = activeLight.intensity / (distanceToLight + 1f);

            if (score > bestScore)
            {
                bestScore = score;
                targetLight = activeLight;
            }
        }
        
        m_currentLightTarget = targetLight;
    }
    
    public void RotateTowards(Vector3 targetPoint)
    {
        Vector3 dirToTargetXZ = targetPoint - transform.position;
        dirToTargetXZ.y = 0;
        
        if (dirToTargetXZ.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dirToTargetXZ);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_turnRate * Time.deltaTime);
        }
    }
}
