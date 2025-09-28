using System;
using System.Collections.Generic;
using CustomToolkit.AdvancedTypes;
using UnityEngine;

public class LightManager : Singleton<LightManager>
{
    private List<LightSource> m_lightSources = new List<LightSource>();
    
    private List<Light> m_activeLights = new List<Light>();
    public List<Light> ActiveLights => m_activeLights;
    
    private void OnLightToggled(Light lightSource, bool isOn)
    {
        if(isOn)
        {
            if(!m_activeLights.Contains(lightSource))
                m_activeLights.Add(lightSource);
        }
        else
        {
            if(m_activeLights.Contains(lightSource))
                m_activeLights.Remove(lightSource);
        }
    }
    
    public float GetLightIntensityAtPosition(Vector3 position)
    {
        float totalIntensity = 0; 
        
        foreach (Light activeLight in m_activeLights)
        {
            float distance = Vector3.Distance(position, activeLight.transform.position);
            if (distance < activeLight.range)
            {
                float intensity = activeLight.intensity / (1.0f + distance * distance);
                totalIntensity += intensity;
            }
        }

        return totalIntensity;
    }

    public void AddLightSource(LightSource lightSource)
    {
        lightSource.OnToggled += OnLightToggled;
        m_lightSources.Add(lightSource);
    }

    public void RemoveLightSource(LightSource lightSource)
    {
        lightSource.OnToggled -= OnLightToggled;
        m_lightSources.Remove(lightSource);
    }
}
