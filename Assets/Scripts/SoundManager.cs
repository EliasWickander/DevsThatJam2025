using CustomToolkit.AdvancedTypes;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] 
    private AudioSource soundFXObject;
    
    public void PlaySoundFX(AudioClip audioClip, Transform spawnTransform, float volume, bool is3D = true, float maxDistance = 5.0f)
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;
        
        if(is3D)
        {
            audioSource.spatialBlend = 1.0f;
            audioSource.maxDistance = maxDistance;
        }
        else
        {
            audioSource.spatialBlend = 0.0f;
        }

        //play sound
        audioSource.Play();

        //get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
