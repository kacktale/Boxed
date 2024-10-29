using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
    void Start()
    {
        ErrorMessage.text = "";
        HidPosY = CurPosY;
        HidPosX = CurPosX;
    }

    // Update is called once per frame
    void Update()
    {
        //if(PhotonNetwork.PlayerList.Length == 2)
        //{
        //    if(!CreateGround)
        //    {

        //        CreateGround = true;
        //    }
        //}
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        int CurPlayer = Players.Length;
        ColorBlock colorBlock = GameStartbutton.colors;
        if (CurPlayer == 2)
        {
            colorBlock.normalColor = Color.green;
            colorBlock.highlightedColor = Color.green;
            colorBlock.pressedColor = new Color(0, 0.6666666f, 0.1437909f);
            colorBlock.selectedColor = Color.green;
            colorBlock.disabledColor = Color.green;
        }
        else
        {
            colorBlock.normalColor = Color.gray;
            colorBlock.highlightedColor = Color.gray;
            colorBlock.pressedColor = new Color(0, 0.6f, 0.7f);
            colorBlock.selectedColor = Color.gray;
            colorBlock.disabledColor = Color.gray;
        }
        GameStartbutton.colors = colorBlock;
    }
    [PunRPC]
    public void StartGame()
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        int CurPlayers = Players.Length;
        if(CurPlayers == 2)
        {
            Players[0].transform.position = new Vector3(-8.27f, -2.78f);
            Players[1].transform.position = new Vector3(23.53f,-2.78f);
            MakeNomalGround();
            ChangeRange.Range = 1;
            ChangeRange.ChangedRange();
        }
        else
        {
            ErrorTextAppear();
        }
    }
    private void ErrorTextAppear()
    {
        ErrorMessage.text = "플레이어 수가 부족하거나 너무 많습니다.";
        Invoke("ErrorTextHide",2f);
    }
    private void ErrorTextHide()
    {
        ErrorMessage.text = "";
    }
    [PunRPC]
    public void MakeNomalGround()
    {
        if (!CreateGround)
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
                PhotonNetwork.Instantiate("Ground", pos, Quaternion.identity);
            }
            MakeEndGround();
        }
    }
    [PunRPC]
    public void MakeEndGround()
    {
        float posX = Random.Range(-2, 2);
        float posY = Random.Range(3, 5);
        Vector2 pos = new Vector2(posX, posY + CurPosY);
        PhotonNetwork.Instantiate("EndGround", pos, Quaternion.identity);
        CurPosY = HidPosY;
        CurPosX = HidPosX;
        CreateGround = true;
        GameStartbutton.gameObject.SetActive(false);
    }
}
