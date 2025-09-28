using System;
using UnityEngine;

public class MothAnimationEventListener : MonoBehaviour
{
    public event Action OnStepEvent;
    
    public void OnStep()
    {
        OnStepEvent?.Invoke();
    }
}
