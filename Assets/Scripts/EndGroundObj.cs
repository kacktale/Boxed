using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGroundObj : MonoBehaviour, IPunObservable
{
    public int EnterPlayer = 0;
    private bool hasActivatedPlayers = false;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController pressPlayer = collision.gameObject.GetComponent<PlayerController>();

            if (pressPlayer.isActive)
            {
                pressPlayer.SetActiveState(false);

                photonView.RPC("IncreaseEnterPlayerCount", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    private void IncreaseEnterPlayerCount()
    {
        EnterPlayer++;
        Debug.Log("플레이어가 들어왔습니다: " + EnterPlayer);

        if (EnterPlayer == 2 && !hasActivatedPlayers)
        {
            ActivateAllPlayers();
        }
    }

    private void ActivateAllPlayers()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerController playerScript = player.GetComponent<PlayerController>();
            playerScript.SetActiveState(true);
        }

        hasActivatedPlayers = true;
        Debug.Log("모든 플레이어가 활성화되었습니다!");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(EnterPlayer);
            stream.SendNext(hasActivatedPlayers);
        }
        else
        {
            EnterPlayer = (int)stream.ReceiveNext();
            hasActivatedPlayers = (bool)stream.ReceiveNext();
        }
    }
}
