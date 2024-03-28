using Photon.Pun;
using UnityEngine;

public class CharacterRespawner : MonoBehaviourPunCallbacks
{
    
    public Transform[] spawnPoints;
    public static CharacterRespawner Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public void RespawnPlayer()
    {
        // 랜덤한 위치 선택
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[spawnIndex].position;
        Quaternion spawnRotation = spawnPoints[spawnIndex].rotation;

        
        PhotonNetwork.Instantiate("Character", spawnPosition, spawnRotation);
    }
}