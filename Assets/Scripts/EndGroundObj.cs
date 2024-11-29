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
    public CreateRandomGround randomGround;

    private bool EnterPing = false;
    public bool GameEnd = false;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        changeRange = FindAnyObjectByType<ChangeRange>();
        randomGround = FindAnyObjectByType<CreateRandomGround>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EnterPing = true;
            PlayerController pressPlayer = collision.gameObject.GetComponent<PlayerController>();
            pressPlayer.CantMove = true;
            
            if (pressPlayer.isActive)
            {
                photonView.RPC("IncreaseEnterPlayerCount", RpcTarget.AllBuffered);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        EnterPing = false;
    }

    [PunRPC]
    private void IncreaseEnterPlayerCount()
    {
        if (EnterPing)
        {
            EnterPlayer++;
            Debug.Log("플레이어가 들어왔습니다: " + EnterPlayer);

            if (EnterPlayer == 2 && !hasActivatedPlayers)
            {
                GameEnd = true;
                changeRange.Range = 3;
                randomGround.GameStartbutton.gameObject.transform.localScale = Vector3.one;
                photonView.RPC("CharacterMove", RpcTarget.AllBuffered);
            }
        }
    }
    [PunRPC]
    private void CharacterMove()
    {
        foreach(var Players in GameObject.FindGameObjectsWithTag("Player"))
        {
            Debug.Log(Players);
            PlayerController playerController = Players.GetComponent<PlayerController>();
            playerController.CantMove = false;
            Debug.Log("playerController.CantMove");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(EnterPlayer);
            stream.SendNext(hasActivatedPlayers);
            stream.SendNext(changeRange.Range);
            stream.SendNext(EnterPing);
        }
        else
        {
            EnterPlayer = (int)stream.ReceiveNext();
            hasActivatedPlayers = (bool)stream.ReceiveNext();
            changeRange.Range = (int)stream.ReceiveNext();
            EnterPing = (bool)stream.ReceiveNext();
        }
    }
}
