using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController m_playerPrefab;

    [SerializeField]
    private BigMoth m_bigMothPrefab;
    
    [SerializeField]
    private SmallMoth m_smallMothPrefab;
    
    [SerializeField]
    private PlayerCamera m_playerCameraPrefab;

    [SerializeField]
    private int m_smallMothAmount = 3;

    public int SmallMothAmount => m_smallMothAmount;
    
    [SerializeField]
    private Transform[] m_spawnPoints;
    
    private List<Transform> m_activeSpawnPoints = new List<Transform>();

    [SerializeField]
    private bool m_peacefulMode = false;
    
    private void Awake()
    {
        m_activeSpawnPoints = m_spawnPoints.ToList();
    }

    private void Start()
    {
        SpawnPlayer();
        
        if(!m_peacefulMode)
            SpawnBigMoth();
        
        SpawnSmallMoths();
    }

    private void SpawnPlayer()
    {
        if (m_playerPrefab != null)
        {
            GameObject spawnedPlayer = SpawnAtRandomActiveSpawnPoint(m_playerPrefab.gameObject);

            if (spawnedPlayer != null)
            {
                PlayerController playerController = spawnedPlayer.GetComponent<PlayerController>();
                
                if (m_playerCameraPrefab != null)
                {
                    PlayerCamera spawnedPlayerCamera = Instantiate(m_playerCameraPrefab);
                    spawnedPlayerCamera.CinemachineCamera.Follow = playerController.HeadTransform;
                    spawnedPlayerCamera.CinemachinePanTilt.PanAxis.Value = playerController.HeadTransform.eulerAngles.y;
                }
            }
        }
    }

    private void SpawnBigMoth()
    {
        if(m_bigMothPrefab != null)
        {
            GameObject spawnedBigMothObject = SpawnAtRandomActiveSpawnPoint(m_bigMothPrefab.gameObject);
            BigMoth bigMoth = spawnedBigMothObject.GetComponent<BigMoth>();
            bigMoth.SetPatrolPoints(GameManager.Instance.PatrolPoints);
        }
    }
    
    private void SpawnSmallMoths()
    {
        if (m_smallMothPrefab != null)
        {
            for(int i = 0; i < m_smallMothAmount; i++)
            {
                SpawnAtRandomActiveSpawnPoint(m_smallMothPrefab.gameObject);
            }
        }
    }

    private GameObject SpawnAtRandomActiveSpawnPoint(GameObject prefab)
    {
        if (m_activeSpawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, m_activeSpawnPoints.Count);
            Transform spawnPoint = m_activeSpawnPoints[spawnIndex];
            m_activeSpawnPoints.Remove(spawnPoint);
            
            return Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
        
        return null;
    }
}
