using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public TextMeshPro PlayerTxt;
    float Speed = 0.1f;
    public float JumpPower = 0.0f;
    PhotonView photonView;
    Rigidbody2D rigid;
    public LayerMask layerMask;
    public bool isGround = false;
    public bool Chatting = false;
    public bool isActive = true;
    public bool CantMove = false;
    private bool JumpKey = false;
    private bool EndGroundEnter = false;
    public bool NPCInteract = false;
    [Header("Buff")]
    public bool doubleJump = false;
    public int doblueJumpCount = 2;
    [Header("Nerf")]
    public bool doublePress = false;
    public int doublePressCount = 2;
    [Header("Input")]
    public ChangeRange changeRange;
    public RoundManager roundManager;
    [Header("BangJang")]
    public bool Master = false;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody2D>();
        changeRange = FindAnyObjectByType<ChangeRange>();
        roundManager = FindAnyObjectByType<RoundManager>();

        if (photonView.IsMine)
        {
            var CM = GameObject.Find("CMCam").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
            PlayerTxt.color = Color.green;
            PlayerTxt.text = PhotonNetwork.NickName;
        }
        else
        {
            PlayerTxt.text = photonView.Owner.NickName;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if(!CantMove && !Chatting)
        {
            if (photonView.IsMine)
            {
                //rigid.velocity = new Vector2 (h * Speed, 0);
                transform.position += new Vector3(h * Speed, 0);
            }
        }
    }
    void Update()
    {
        if (!CantMove)
        {
            RaycastHit2D MidgroundHit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, layerMask);
            RaycastHit2D EndgroundHit = Physics2D.Raycast(transform.position + Vector3.right * 0.4f, Vector2.down, 0.6f, layerMask);
            RaycastHit2D AnotherEndgroundHit = Physics2D.Raycast(transform.position + Vector3.left * 0.4f, Vector2.down, 0.6f, layerMask);
            Debug.DrawRay(transform.position, Vector3.down * 0.6f, Color.blue);
            Debug.DrawRay(transform.position + Vector3.right * 0.4f, Vector3.down * 0.6f, Color.blue);
            Debug.DrawRay(transform.position + Vector3.left * 0.4f, Vector3.down * 0.6f, Color.blue);
            if (MidgroundHit.collider || EndgroundHit.collider || AnotherEndgroundHit.collider)
            {
                if (!JumpKey)
                {
                    isGround = true;
                    doblueJumpCount = 2;
                }
            }
            else
            {
                if (doubleJump)
                {
                    if (doblueJumpCount <= 0)
                    {
                        isGround = false;
                    }
                    if (doblueJumpCount == 2)
                    {
                        doublePressCount = 1;
                    }
                }
                else
                {
                    isGround = false;
                }
            }
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Space) && isGround && !Chatting)
            {
                photonView.RPC("Jump", RpcTarget.All);
                JumpKey = true;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                JumpKey = false;
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (Chatting)
                {
                    Chatting = false;
                }
                else
                {
                    Chatting = true;
                }
            }
        }
    }

    [PunRPC]
    private void Jump()
    {
        if (isGround)
        {
            if (doublePress)
            {
                if(doublePressCount <= 1)
                {
                    if (doubleJump)
                    {
                        if (doblueJumpCount > 0)
                        {
                            rigid.AddForce(Vector3.up * JumpPower, ForceMode2D.Impulse);
                            doblueJumpCount--;
                        }
                        else
                        {
                            isGround = false;
                        }
                    }
                    else
                    {
                        rigid.AddForce(Vector3.up * JumpPower, ForceMode2D.Impulse);
                        isGround = false;
                    }
                    doublePressCount = 2;
                }
                else
                {
                    doublePressCount--;
                }
            }
            else
            {
                if (doubleJump)
                {
                    if(doblueJumpCount > 0)
                    {
                        rigid.AddForce(Vector3.up * JumpPower, ForceMode2D.Impulse);
                        doblueJumpCount--;
                    }
                    else
                    {
                        isGround = false;
                    }
                }
                else
                {
                    rigid.AddForce(Vector3.up * JumpPower, ForceMode2D.Impulse);
                    isGround = false;
                }
            }
        }
    }

    private void OnDisable()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetActiveState", RpcTarget.All, false);
        }
    }

    [PunRPC]
    public void SetActiveState(bool state)
    {
        isActive = state;
        gameObject.SetActive(state);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EndGround"))
        {
            EndGroundObj GoundSetting = collision.gameObject.GetComponent<EndGroundObj>();
            Debug.Log(GoundSetting.gameObject);
            if (GoundSetting.EnterPlayer == 1)
            {
                if (Master)
                {
                    roundManager.redWin++;
                }
                else if (!Master)
                {
                    roundManager.blueWin++;
                }
            }
            transform.position = new Vector3(58.82f, -46.75f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            NPCInteract = true;
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EndGround"))
        {
            EndGroundEnter = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            NPCInteract = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 상태 동기화
        if (stream.IsWriting)
        {
            stream.SendNext(isActive);
            stream.SendNext(doubleJump);
            stream.SendNext(doublePress);
            stream.SendNext(roundManager.redWin);
            stream.SendNext(roundManager.blueWin);
        }
        else
        {
            isActive = (bool)stream.ReceiveNext();
            doubleJump = (bool)stream.ReceiveNext();
            doublePress = (bool)stream.ReceiveNext();
            roundManager.redWin = (int)stream.ReceiveNext();
            roundManager.blueWin = (int)stream.ReceiveNext();
            gameObject.SetActive(isActive);
        }
    }
}
