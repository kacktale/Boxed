using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ChatManager : MonoBehaviourPunCallbacks
{ 
    public InputField chatInput;
    public Text chatDisplay;

    private bool IsChatAppear = true;
    // 방에 접속하면 호출됨
    public override void OnJoinedRoom()
    {
        Debug.Log("방에 접속했습니다.");
        chatDisplay.text += "\n[시스템] 방에 접속했습니다.";
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(!IsChatAppear)
            {
                chatInput.gameObject.SetActive(true);
                IsChatAppear=true;
            }
            else
            {
                string message = chatInput.text.Trim();
                if (chatInput.text != "" || !string.IsNullOrEmpty(message))
                {
                    Debug.Log("Message Sent: " + chatInput.text);

                    photonView.RPC("ReceiveMessage", RpcTarget.All, PhotonNetwork.NickName, chatInput.text);
                    chatInput.text = "";
                }
                chatInput.gameObject.SetActive(false);
                IsChatAppear = false;
            }
        }
    }

    [PunRPC]
    void ReceiveMessage(string playerName, string message)
    {
        chatDisplay.text += $"\n<color=yellow>[{playerName}]</color> {message}";
    }
}
