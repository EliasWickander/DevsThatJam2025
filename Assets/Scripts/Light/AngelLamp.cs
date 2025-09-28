using System;
using UnityEngine;

public class AngelLamp : LightSource
{
    private CollisionTrigger[] m_triggers;

    [SerializeField]
    private LayerMask m_smallMothLayerMask;
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
    
    private void OnLightEntered(Collider other)
    {
        if (((1 << other.gameObject.layer) & m_smallMothLayerMask.value) != 0)
        {
            var moth = other.GetComponentInParent<SmallMoth>();
            if(moth != null)
            {
                moth.OnEnterAngelLight(this);
            }
        }
    }
}
