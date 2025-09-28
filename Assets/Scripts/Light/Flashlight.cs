using UnityEngine;

public class Flashlight : LightSource
{
    [SerializeField]
    private AudioSource m_audioSource;
    
    [SerializeField] 
    private AudioClip m_turnOnLamp;
    
    [SerializeField] 
    private AudioClip m_turnOffLamp;

    protected override void OnToggle(bool isOn)
    {
        base.OnToggle(isOn);
        
        SoundManager.Instance.PlaySoundFX(isOn ? m_turnOnLamp : m_turnOffLamp, transform, 1.0f);
    }
}
