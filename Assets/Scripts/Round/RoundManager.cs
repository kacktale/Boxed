using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour, IPunObservable
{
    public int redWin = 0;
    public int blueWin = 0;
    public Image[] RedImg;
    public Image[] BlueImg;
    public Image MiddleImg;
    public PhotonView PhotonView;

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(blueWin == 4 || redWin == 4)
        {
            return;
        }
        switch(redWin)
        {
            case 1: RedImg[0].color = Color.red; break;
            case 2: RedImg[1].color = Color.red; break;
            case 3: RedImg[2].color = Color.red; break;
            case 4: MiddleImg.color = Color.red; break;
        }
        switch(blueWin)
        {
            case 1: BlueImg[0].color = Color.blue; break;
            case 2: BlueImg[1].color = Color.blue; break;
            case 3: BlueImg[2].color = Color.blue; break;
            case 4: MiddleImg.color = Color.blue; break;
        }

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(redWin);
            stream.SendNext(blueWin);
        }
        else
        {
            redWin = (int)stream.ReceiveNext();
            blueWin = (int)stream.ReceiveNext();
        }
    }
}
