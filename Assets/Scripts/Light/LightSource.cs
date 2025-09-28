using System;
using UnityEngine;

public abstract class LightSource : MonoBehaviour
{
    [SerializeField]
    protected Light m_lightObject;
    
    protected bool m_isOn = false;
    public bool IsOn => m_isOn;
    public event Action<Light, bool> OnToggled;
    
    protected virtual void Start()
    {
        LightManager.Instance.AddLightSource(this);
        Toggle(m_lightObject.gameObject.activeSelf);
    }

    public void Toggle(bool isOn)
    {
        m_isOn = isOn;
            
        OnToggle(m_isOn);
        OnToggled?.Invoke(m_lightObject, m_isOn);
    }
    
    public void Toggle()
    {
        m_isOn = !m_isOn;
        
        OnToggle(m_isOn);
        OnToggled?.Invoke(m_lightObject, m_isOn);
    }
    
    private void Kill()
    {
        Toggle(false);
        LightManager.Instance.RemoveLightSource(this);
    }
    
    protected virtual void OnToggle(bool isOn)
    {
        if (m_lightObject != null && m_lightObject.gameObject.activeSelf != isOn)
            m_lightObject.gameObject.SetActive(isOn);   
    }
}
