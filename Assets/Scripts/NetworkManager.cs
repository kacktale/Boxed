using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject LoadingPannel;
    public GameObject CreateNameScene;
    public GameObject OnlyMaster;
    public InputField inputField;
    public TextMeshProUGUI MaxPlayerList;
    public CreateRandomGround createRandomGround;
    void Start()
    {
        MaxPlayerList.text = "";
        Screen.SetResolution(1920,1080,true);
        PhotonNetwork.ConnectUsingSettings();  // Photon 서버에 연결
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("Room1", new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        LoadingPannel.SetActive(false);
        Debug.Log("채팅 방에 입장했습니다.");
        MaxPlayerList.text = "u r " + PhotonNetwork.PlayerList.Length + "st place";
    }
    public void SetNick()
    {
        PhotonNetwork.NickName = inputField.text;
        CreateNameScene.SetActive(false);
        //CamStart.enabled = false;

        PhotonNetwork.Instantiate("Player", new Vector3(-57.23f, -37.31f,0), Quaternion.identity);

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        int CurPlayer = Players.Length;
        if(CurPlayer == 1)
        {
            SpriteRenderer color = Players[CurPlayer-1].GetComponent<SpriteRenderer>();
            color.color = Color.red;
            OnlyMaster.SetActive(true);
            PlayerController controller = Players[CurPlayer-1].GetComponent<PlayerController>();
            controller.Master = true;
        }
        else if(CurPlayer == 2)
        {
            SpriteRenderer color = Players[CurPlayer - 1].GetComponent<SpriteRenderer>();
            color.color = Color.blue;
        }
    }
}
