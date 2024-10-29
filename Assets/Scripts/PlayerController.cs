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
    public bool isActive = true;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody2D>();

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
        if (photonView.IsMine)
        {
            //rigid.velocity = new Vector2 (h * Speed, 0);
            transform.position += new Vector3(h * Speed, 0);
        }
    }
    void Update()
    {
        RaycastHit2D MidgroundHit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, layerMask);
        RaycastHit2D EndgroundHit = Physics2D.Raycast(transform.position + Vector3.right * 0.5f, Vector2.down, 0.6f, layerMask);
        RaycastHit2D AnotherEndgroundHit = Physics2D.Raycast(transform.position + Vector3.left * 0.5f, Vector2.down, 0.6f, layerMask);
        Debug.DrawRay(transform.position, Vector3.down * 0.6f, Color.blue);
        Debug.DrawRay(transform.position + Vector3.right * 0.5f, Vector3.down * 0.6f, Color.blue);
        Debug.DrawRay(transform.position + Vector3.left * 0.5f, Vector3.down * 0.6f, Color.blue);
        isGround = MidgroundHit.collider || EndgroundHit.collider || AnotherEndgroundHit.collider != null ? true : false;
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            photonView.RPC("Jump", RpcTarget.All);
        }
    }

    [PunRPC]
    private void Jump()
    {
        if (isGround)
        {
            rigid.AddForce(Vector3.up * JumpPower, ForceMode2D.Impulse);
            isGround = false;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 상태 동기화
        if (stream.IsWriting)
        {
            stream.SendNext(isActive);
        }
        else
        {
            isActive = (bool)stream.ReceiveNext();
            gameObject.SetActive(isActive);
        }
    }
}
