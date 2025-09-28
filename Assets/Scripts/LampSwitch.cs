using System;
using UnityEngine;

public class LampSwitch : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Animator m_animator;
    
    private bool m_pendingState = false;
    private bool m_isOn = false;
    private int m_switchOnHash = Animator.StringToHash("SwitchOn");
    private int m_switchOffHash = Animator.StringToHash("SwitchOff");

    private void Start()
    {
        m_pendingState = m_isOn;
    }

    public void Interact()
    {
        Toggle(!m_isOn);
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

    public bool CanInteract()
    {
        return m_pendingState == m_isOn;
    }

    private void Toggle(bool value)
    {
        m_pendingState = value;
        m_animator.SetTrigger(value ? m_switchOnHash : m_switchOffHash);
    }
    
    private void SetOnState(bool value)
    {
        m_isOn = value;
    }
}
