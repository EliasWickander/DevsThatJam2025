using UnityEngine;
using UnityEngine.Serialization;

public class FlickeringLamp : LightSource
{
    [SerializeField] 
    private float m_minIntensity = 0.8f;
    
    [SerializeField] 
    private float m_maxIntensity = 1.2f;

    [SerializeField] 
    private float m_flickerSpeed = 0.1f;

    [SerializeField] 
    private float m_offChance = 0.1f;

    [SerializeField]
    private float m_minOffDuration = 0.05f;
    
    [SerializeField] 
    private float m_maxOffDuration = 0.3f;
    
    private float m_targetIntensity;
    private float m_timeUntilNextFlicker;
    private float m_offTimer;
    private float m_offDuration;

    protected override void Start()
    {
        base.Start();
        
        m_targetIntensity = m_lightObject.intensity;
        m_timeUntilNextFlicker = m_flickerSpeed;
    }
    
    void Update()
    {
        if(m_isOn)
            HandleFlicker();
        else
        {
            m_offTimer += Time.deltaTime;
            if(m_offTimer >= m_offDuration)
            {
                Toggle(true);
            }
        }
    }

    protected override void OnToggle(bool isOn)
    {
        base.OnToggle(isOn);

        m_offTimer = 0.0f;
    }

    private void HandleFlicker()
    {
        m_timeUntilNextFlicker -= Time.deltaTime;

        if (Random.value < m_offChance * Time.deltaTime)
        {
            m_offDuration = Random.Range(m_minOffDuration, m_maxOffDuration);
            Toggle(false);
        }
        else
        {
            if (m_timeUntilNextFlicker <= 0f)
            {
                m_targetIntensity = Random.Range(m_minIntensity, m_maxIntensity);
                m_timeUntilNextFlicker = m_flickerSpeed;
            }

            m_lightObject.intensity = Mathf.Lerp(m_lightObject.intensity, m_targetIntensity, Time.deltaTime / m_flickerSpeed);   
        }
    }
}
