using UnityEngine;

public class Flashlight : LightSource
{
    [SerializeField] private AudioClip m_turnOnLamp;
    [SerializeField] private AudioClip m_turnOffLamp;

    protected override void OnToggle(bool isOn)
    {
        base.OnToggle(isOn);

        // if (isOn)
        //     SoundFXManager.instance.PlaySoundFXClip(m_turnOnLamp, transform, 1f);
        // else
        //     SoundFXManager.instance.PlaySoundFXClip(m_turnOffLamp, transform, 1f);
    }
}
