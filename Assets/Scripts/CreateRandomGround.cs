using Photon.Pun;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRandomGround : MonoBehaviour
{
    public GameObject ground;
    public GameObject EndGround;

    public float CurPosY = -2.68f;
    private float HidPosY;
    public float CurPosX = 8.67f;
    private float HidPosX;

    public int MaxGroundSpawn;
    public bool CreateGround = false;

    public TextMeshProUGUI ErrorMessage;
    public Button GameStartbutton;

    public ChangeRange ChangeRange;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        ErrorMessage.text = "";
        HidPosY = CurPosY;
        HidPosX = CurPosX;
    }

    void Update()
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        int CurPlayer = Players.Length;
        ColorBlock colorBlock = GameStartbutton.colors;

        colorBlock.normalColor = (CurPlayer == 2) ? Color.green : Color.gray;
        colorBlock.highlightedColor = colorBlock.normalColor;
        colorBlock.pressedColor = (CurPlayer == 2) ? new Color(0, 0.6666666f, 0.1437909f) : new Color(0, 0.6f, 0.7f);
        colorBlock.selectedColor = colorBlock.normalColor;
        colorBlock.disabledColor = colorBlock.normalColor;

        GameStartbutton.colors = colorBlock;

        if (Input.GetKeyDown(KeyCode.K))
        {
            CreateGround = false;
            photonView.RPC("TeleportPlayers", RpcTarget.All);
            photonView.RPC("MakeNomalGround", RpcTarget.All);
            photonView.RPC("FindRange", RpcTarget.All);
            GameStartbutton.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            photonView.RPC("DeleteGround", RpcTarget.All);
            CreateGround = false;
            photonView.RPC("TeleportPlayers", RpcTarget.All);
            photonView.RPC("MakeNomalGround", RpcTarget.All);
            photonView.RPC("FindRange", RpcTarget.All);
            GameStartbutton.gameObject.transform.localScale = Vector3.zero;
        }
        else
        {
            ErrorTextAppear();
        }
    }

    [PunRPC]
    private void DeleteGround()
    {
        foreach (var Ground in GameObject.FindGameObjectsWithTag("Ground"))
        {
            Destroy(Ground);
        }

        GameObject EndGround = GameObject.FindGameObjectWithTag("EndGround");
        Destroy(EndGround);
    }

    private void ErrorTextAppear()
    {
        ErrorMessage.text = "플레이어 수가 부족하거나 너무 많습니다.";
        Invoke("ErrorTextHide", 2f);
    }

    private void ErrorTextHide()
    {
        ErrorMessage.text = "";
    }
    [PunRPC]
    void FindRange()
    {
        ChangeRange.Range = 2;
    }

    [PunRPC]
    private void TeleportPlayers()
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        if (Players.Length == 2)
        {
            Players[0].transform.position = new Vector3(-8.27f, -2.78f);
            Players[1].transform.position = new Vector3(23.53f, -2.78f);
        }
    }

    [PunRPC]
    public void MakeNomalGround()
    {
        if (!CreateGround && PhotonNetwork.IsMasterClient)
        {
            Vector2 FirstPos = new Vector2(HidPosX, HidPosY);
            PhotonNetwork.Instantiate(ground.name, FirstPos, Quaternion.identity);

            for (int i = 0; i < MaxGroundSpawn; i++)
            {
                float posX = Random.Range(-5, 5);
                float posY = Random.Range(3, 5);
                Vector2 pos = new Vector2(posX + CurPosX, posY + CurPosY);
                CurPosY += posY;
                CurPosX += posX;
                PhotonNetwork.Instantiate(ground.name, pos, Quaternion.identity);
            }
            photonView.RPC("MakeEndGround", RpcTarget.All);
        }
    }

    [PunRPC]
    public void MakeEndGround()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            float posX = Random.Range(-2, 2);
            float posY = Random.Range(3, 5);
            Vector2 pos = new Vector2(posX + CurPosX, posY + CurPosY);
            PhotonNetwork.Instantiate(EndGround.name, pos, Quaternion.identity);

            CurPosY = HidPosY;
            CurPosX = HidPosX;
            CreateGround = true;
            GameStartbutton.gameObject.SetActive(false);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ChangeRange.Range);
        }
        else
        {
            ChangeRange.Range = (int)stream.ReceiveNext();
        }
    }
}
