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
    // �濡 �����ϸ� ȣ���
    public override void OnJoinedRoom()
    {
        Debug.Log("�濡 �����߽��ϴ�.");
        chatDisplay.text += "\n[�ý���] �濡 �����߽��ϴ�.";
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
