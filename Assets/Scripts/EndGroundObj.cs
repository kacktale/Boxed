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
    public ChangeRange changeRange;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        changeRange = FindAnyObjectByType<ChangeRange>();
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
            photonView.RPC("ActivateAllPlayers", RpcTarget.All);
        }
    }

    [PunRPC]
    private void ActivateAllPlayers()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerController playerScript = player.GetComponent<PlayerController>();
            playerScript.transform.position = new Vector3(58.82f, -46.75f,0);
            playerScript.SetActiveState(true);
        }
        changeRange.Range = 3;
        hasActivatedPlayers = true;
        Debug.Log("모든 플레이어가 활성화되었습니다!");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(EnterPlayer);
            stream.SendNext(hasActivatedPlayers);
            stream.SendNext(changeRange.Range);
        }
        else
        {
            EnterPlayer = (int)stream.ReceiveNext();
            hasActivatedPlayers = (bool)stream.ReceiveNext();
            changeRange.Range = (int)stream.ReceiveNext();
        }
    }
}
