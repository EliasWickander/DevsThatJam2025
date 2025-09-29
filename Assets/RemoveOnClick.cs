using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RemoveOnClick : MonoBehaviour
{
    public InputActionReference m_ref;

    public UnityEvent m_event;
    
    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            m_event?.Invoke();
            Destroy(gameObject);
        }
    }
}
