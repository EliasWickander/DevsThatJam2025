using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField]
    private GameObject m_lightGameObject;

    private bool m_isOn = false;

    private void Start()
    {
        m_lightGameObject.SetActive(m_isOn);
    }

    public void ToggleFlashlight()
    {
        m_isOn = !m_isOn;
        m_lightGameObject.SetActive(m_isOn);
    }
}
