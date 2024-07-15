using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Player : MonoBehaviour
{
    [SerializeField]private float moveSpeed;
    PhotonView view;
    private bool joined;

    void Start()
    {
        view = GetComponent<PhotonView>();
        joined = false;
    }
    void Update()
    {
        if(view.IsMine)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            Vector3 moveLoc = new Vector3(moveX, moveY, 0);
            transform.position += moveLoc*moveSpeed*Time.deltaTime;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !joined)
        {
            Debug.Log(joined);
            AgoraUnityVideo.Instance.JoinAgora();
            joined = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && joined)
        {
            AgoraUnityVideo.Instance.LeaveAgora();
            joined = false;
        }
    }
    
}
