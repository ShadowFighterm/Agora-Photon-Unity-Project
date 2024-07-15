using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnPlayer : MonoBehaviour
{
    [SerializeField]private GameObject playerPrefab;
    [SerializeField]private float minX;
    [SerializeField]private float maxX;
    [SerializeField]private float minY;
    [SerializeField]private float maxY;
    void Start()
    {
        Vector2 randomPos = new Vector2(Random.Range(minX,maxX),Random.Range(minY,maxY));
        PhotonNetwork.Instantiate(playerPrefab.name,randomPos,Quaternion.identity);
    }
}
