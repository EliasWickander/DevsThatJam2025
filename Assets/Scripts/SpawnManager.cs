using Unity.Cinemachine;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController m_playerPrefab;

    [SerializeField]
    private PlayerCamera m_playerCameraPrefab;
    
    [SerializeField]
    private Transform[] m_playerSpawnPoints;
    
    private void Awake()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (m_playerPrefab != null && m_playerSpawnPoints.Length > 0)
        {
            int spawnIndex = Random.Range(0, m_playerSpawnPoints.Length);
            Transform spawnPoint = m_playerSpawnPoints[spawnIndex];

            PlayerController spawnedPlayer = Instantiate(m_playerPrefab, spawnPoint.position, spawnPoint.rotation);

            if (m_playerCameraPrefab != null)
            {
                PlayerCamera spawnedPlayerCamera = Instantiate(m_playerCameraPrefab);
                spawnedPlayerCamera.CinemachineCamera.Follow = spawnedPlayer.HeadTransform;
                spawnedPlayerCamera.CinemachinePanTilt.PanAxis.Value = spawnedPlayer.HeadTransform.eulerAngles.y;
            }
        }
    }
}
