using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NpcInteract : MonoBehaviour
{
    public GameObject InteractText;
    public Image[] CardImg;
    public TextMeshProUGUI[] CardText;
    public Outline[] CardOutline;
    public Transform pannelTransform;
    public int Selection = 0;

    private bool IsTouched = false;
    private bool IsShowed = false;
    private bool BuffGived = false;
    private Vector3 targetPos;
    [SerializeField]
    private GameObject CollsionPlayer;
    private PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        targetPos = pannelTransform.position;
        pannelTransform.position -= new Vector3(0, 10000, 0);

        for (int i = 0; i < CardOutline.Length; i++)
        {
            if (i == Selection)
            {
                CardOutline[i].enabled = true;
            }
            else
            {
                CardOutline[i].enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CardPannelAppear();
        ChooseCard();
    }

    void ChooseCard()
    {
        if (IsShowed)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                if(Selection == 0)
                {
                    Selection++;
                    for (int i = 0; i < CardOutline.Length; i++)
                    {
                        if (i == Selection)
                        {
                            CardOutline[i].enabled = true;
                        }
                        else
                        {
                            CardOutline[i].enabled = false;
                        }
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                if(Selection == 1)
                {
                    Selection--;
                    for (int i = 0; i < CardOutline.Length; i++)
                    {
                        if (i == Selection)
                        {
                            CardOutline[i].enabled = true;
                        }
                        else
                        {
                            CardOutline[i].enabled = false;
                        }
                    }
                }
            }
        }
    }

    void CardPannelAppear()
    {
        if (IsTouched)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!BuffGived)
                {
                    PlayerController playerController = CollsionPlayer.GetComponent<PlayerController>();
                    if (!IsShowed)
                    {
                        pannelTransform.position = targetPos;
                        IsShowed = true;
                        playerController.CantMove = true;
                    }
                    else
                    {
                        switch (Selection)
                        {
                            case 0:
                                playerController.doubleJump = true; break;
                            case 1:
                                photonView.RPC("DisableDoubleJumpForOthers", RpcTarget.Others); break;
                        }
                        pannelTransform.position -= new Vector3(0, 10000, 0);
                        IsShowed = false;
                        playerController.CantMove = false;
                        BuffGived = true;
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    [PunRPC]
    private void DisableDoubleJumpForOthers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var playerObj in players)
        {
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null && playerController.photonView.IsMine)
            {
                playerController.doublePress = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CollsionPlayer = collision.gameObject;
            InteractText.SetActive(true);
            IsTouched = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            InteractText.SetActive(false);
            IsTouched = false;
        }
    }
}
