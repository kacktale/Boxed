using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeRange : MonoBehaviour, IPunObservable
{
    public CinemachineConfiner confiner;
    private PhotonView photonView;
    public PolygonCollider2D Start, Main, Card;
    public int Range;

    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner>();
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (Range == 1)
        {
            confiner.m_BoundingShape2D = Start;
        }
        else if (Range == 2)
        {
            confiner.m_BoundingShape2D = Main;
        }
        else if (Range == 3)
        {
            confiner.m_BoundingShape2D = Card;
        }
    }

    //public void FindRange()
    //{
    //    photonView.RPC("ChangedRange", RpcTarget.All);
    //}

    //[PunRPC]
    //private void ChangedRange()
    //{
    //    if (Range == 1)
    //    {
    //        confiner.m_BoundingShape2D = Start;
    //    }
    //    else if (Range == 2)
    //    {
    //        confiner.m_BoundingShape2D = Main;
    //    }
    //    else if (Range == 3)
    //    {
    //        confiner.m_BoundingShape2D = Card;
    //    }
    //}
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Range);
        }
        else
        {
            Range = (int)stream.ReceiveNext();
        }
    }
}
