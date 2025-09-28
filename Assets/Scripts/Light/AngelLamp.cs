using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class AngelLamp : LightSource
{
    private CollisionTrigger[] m_triggers;

    [SerializeField]
    private LayerMask m_smallMothLayerMask;

    [SerializeField]
    private UnityEvent m_onTurnOff;

    private SmallMoth m_pulledSmallMoth;

    [SerializeField]
    private float m_deathDelay = 0.5f;
    
    private float m_pendingDeathTimer = 0.0f;
    private bool m_pendingDeath = false;
    
    private void Awake()
    {
        m_triggers = GetComponentsInChildren<CollisionTrigger>(true);
    }

    private void OnEnable()
    {
        if (m_triggers != null)
        {
            foreach(var trigger in m_triggers)
            {
                trigger.OnTriggerEnterEvent += OnLightEntered;
            }   
        }
    }

    private void OnDisable()
    {
        if (m_triggers!= null)
        {
            foreach(var trigger in m_triggers)
            {
                trigger.OnTriggerEnterEvent -= OnLightEntered;
            }   
        }
    }

    private void Update()
    {
        if(m_pendingDeath)
        {
            m_pendingDeathTimer += Time.deltaTime;
            if(m_pendingDeathTimer >= m_deathDelay)
            {
                Toggle(false);
                m_pendingDeath = false;
                m_pendingDeathTimer = 0.0f;
            }
        }
    }

    private void OnLightEntered(Collider other)
    {
        if (((1 << other.gameObject.layer) & m_smallMothLayerMask.value) != 0)
        {
            var moth = other.GetComponentInParent<SmallMoth>();
            if(moth != null)
            {
                m_pulledSmallMoth = moth;
                m_pulledSmallMoth.OnAscendComplete += OnSmallMothAscendComplete;
                moth.OnEnterAngelLight(this);
            }
        }
    }

    protected override void OnToggle(bool isOn)
    {
        base.OnToggle(isOn);

        if (!isOn)
        {
            m_onTurnOff?.Invoke();
        }
    }
    
    private void OnSmallMothAscendComplete()
    {
        m_pulledSmallMoth.OnAscendComplete -= OnSmallMothAscendComplete;
        m_pendingDeath = true;
        m_pendingDeathTimer = 0.0f;
    }
}
