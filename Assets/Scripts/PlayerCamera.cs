using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera m_cinemachineCamera;
    public CinemachineCamera CinemachineCamera => m_cinemachineCamera;
    
    [SerializeField]
    private CinemachinePanTilt m_cinemachinePanTilt;
    public CinemachinePanTilt CinemachinePanTilt => m_cinemachinePanTilt;
}
