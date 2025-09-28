using System;
using UnityEngine;
using UnityEngine.InputSystem; //AUDIO - new input system for audio

public class LampSwitch : MonoBehaviour, IInteractable
{
    public enum ESwitchState
    {
        Off,
        On
    }
    
    [SerializeField]
    private Animator m_animator;

    private ESwitchState m_currentState;
    public ESwitchState CurrentState => m_currentState;
    
    //AUDIO - adds a reference to the action in my script
    [SerializeField] private AudioClip m_turnOnLamp;
    [SerializeField] private AudioClip m_turnOffLamp;
    [SerializeField] private InputActionReference toggleAction;

    private bool m_pendingState = false;
    private bool m_isOn = false;
    private int m_switchOnHash = Animator.StringToHash("SwitchOn");
    private int m_switchOffHash = Animator.StringToHash("SwitchOff");
    
    public event Action<bool> OnSwitchStateChanged;

    //AUDIO - Subscribes to the Input Action in my class
    private void OnEnable()
    {
        // Subscribe safely
        if (toggleAction != null)
        {
            // Wait until the action is initialized
            toggleAction.action.Enable();
            toggleAction.action.performed += OnTogglePerformed; // use a named method
        }
    }

    private void OnDisable()
    {
        if (toggleAction != null && toggleAction.action != null)
        toggleAction.action.performed -= OnTogglePerformed;
    }
        private void OnTogglePerformed(InputAction.CallbackContext context)
    {
        Interact(); //calls my toggle logic once
    }

    private void Start()
    {
        m_pendingState = m_isOn;
        m_currentState = m_isOn ? ESwitchState.On : ESwitchState.Off;
    }

    public void Interact()
    {
        Toggle(!m_isOn);
    }

    public bool CanInteract()
    {
        return m_pendingState == m_isOn;
    }

    private void Toggle(bool value)
    {
        m_pendingState = value;
        m_animator.SetTrigger(value ? m_switchOnHash : m_switchOffHash);

        //AUDIO - Play click sound via SoundFXManager
        //Only do anything if the state is changing
        if (value == m_isOn)
            return;
        //Plays click sound via SoundFXManager
        if (value)
            SoundFXManager.instance.PlaySoundFXClip(m_turnOnLamp, transform, 1f);
        else
            SoundFXManager.instance.PlaySoundFXClip(m_turnOffLamp, transform, 1f);
            // Updates the actual state
            m_isOn = value;
    }
    private void Update()
    {
        if (m_pendingState != m_isOn)
        {
            AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("SwitchOn") || stateInfo.IsName("SwitchOff"))
            {
                if (stateInfo.normalizedTime >= 1f)
                {
                    SetOnState(m_pendingState);
                }
            }
        }
    }
        private void SetOnState(bool value)
    {
        return m_pendingState != true;
    }

    private void Toggle(bool value)
    {
        m_pendingState = value;
        m_animator.SetTrigger(value ? m_switchOnHash : m_switchOffHash);
    }
    
    private void SetOnState(bool value)
    {
        m_currentState = value ? ESwitchState.On : ESwitchState.Off;
        m_isOn = value;
        OnSwitchStateChanged?.Invoke(m_isOn);
    }
}
