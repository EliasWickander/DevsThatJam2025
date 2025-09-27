using CustomToolkit.AdvancedTypes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraManager : Singleton<CameraManager>
{
    private Camera m_currentCamera;

    public Camera CurrentCamera => m_currentCamera;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_currentCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
