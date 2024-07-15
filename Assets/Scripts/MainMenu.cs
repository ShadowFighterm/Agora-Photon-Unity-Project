using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]private InputField createInput;
    [SerializeField]private InputField joinInput;
    [SerializeField]private GameObject lobbyInputError;
    public void CreateRoom()
    {
        if(createInput.text != "")
            PhotonNetwork.CreateRoom(createInput.text);
        else
            lobbyInputError.SetActive(true);
    }
    public void JoinRoom()
    {
        if(joinInput.text != "")
            PhotonNetwork.JoinRoom(joinInput.text);
        else
            lobbyInputError.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}
