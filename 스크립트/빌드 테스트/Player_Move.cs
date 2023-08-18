using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_PLAYER_ANIMATION { NULL, MOVE_FORWARD, MOVE_BACK, THROWING }

public class Player_Move : MonoBehaviour
{
    Player_Total m_Player_Total;

    [SerializeField] float m_fSpeed;

    public Rigidbody m_Rigidbody;

    [SerializeField] Transform m_Transform_Spine;
    [SerializeField] Transform m_Transform_RightArm;
    [SerializeField] Transform m_Transform_RightForeArm;
    [SerializeField] Transform m_Transform_LeftArm;
    [SerializeField] Transform m_Transform_LeftForeArm;

    [SerializeField] Vector3 m_Vector_Spine_Init;
    [SerializeField] Vector3 m_Vector_RightArm_Init;
    [SerializeField] Vector3 m_Vector_LeftArm_Init;

    [SerializeField] Vector3 m_Vector_RightArm_Throwing_Start;

    [SerializeField] Vector3 m_Vector_Spine_Move_Start;
    [SerializeField] Vector3 m_Vector_RightArm_Move_Start;
    [SerializeField] Vector3 m_Vector_LeftArm_Move_Start;

    [SerializeField] bool m_bMove;
    [SerializeField] bool m_bMove_Current;
    [SerializeField] E_PLAYER_ANIMATION m_ePlayer_Animation;
    private void Awake()
    {
        m_Player_Total = this.gameObject.GetComponent<Player_Total>();

        m_Rigidbody = this.gameObject.GetComponent<Rigidbody>();

        m_Vector_Spine_Init = m_Transform_Spine.localRotation.eulerAngles;
        m_Vector_RightArm_Init = new Vector3(-1.806f, 38.224f, 77.056f);
        m_Vector_LeftArm_Init = new Vector3(-4.362f, -40.325f, -82.112f);

        m_Vector_RightArm_Throwing_Start = new Vector3(-1.985f, -125.378f, 102.918f);
        
        m_Vector_Spine_Move_Start = new Vector3(40f, 0, 0);
        m_Vector_RightArm_Move_Start = new Vector3(-0.957f, 18.755f, 82.836f);
        m_Vector_LeftArm_Move_Start = new Vector3(-9.744f, -20.714f, -75.909f);

        m_bMove = false;
        m_ePlayer_Animation = E_PLAYER_ANIMATION.NULL;
    }

    [PunRPC]
    void RPC_Move(int vectordir)
    {
        switch (vectordir)
        {
            case +1:
                {
                    PhotonNetwork.GetPhotonView(1001).gameObject.GetComponent<Player_Total>().m_Player_Move.Behaviour_Move_Forward();
                } break;
            case -1:
                {
                    PhotonNetwork.GetPhotonView(1001).gameObject.GetComponent<Player_Total>().m_Player_Move.Behaviour_Move_Back();
                }
                break;
        }

        //Debug.Log(PhotonNetwork.GetPhotonView(1001).gameObject.name);
        //Debug.Log(PhotonNetwork.GetPhotonView(1001).gameObject.transform.position);
    }

    public void Update()
    {
        m_bMove_Current = false;
        if (m_Player_Total.m_PhotonView.IsMine == true && PhotonNetwork.IsConnected == true && PhotonNetwork.InRoom == true)
        {
            m_bMove = false;

            if (m_Player_Total.m_ePlayer_Move_State != E_PLAYER_MOVE_STATE.IMPOSSIBLE && m_ePlayer_Animation != E_PLAYER_ANIMATION.THROWING)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    if (m_Player_Total.m_ePlayer_Move_State == E_PLAYER_MOVE_STATE.POSSIBLE || m_Player_Total.m_ePlayer_Move_State == E_PLAYER_MOVE_STATE.POSSIBLE_FOWARD)
                    {
                        this.transform.Translate(Vector3.forward * m_fSpeed * Time.deltaTime);
                        Behaviour_Move_Forward();

                        m_Player_Total.m_PhotonView.RPC("RPC_Move", RpcTarget.Others, +1);
                    }
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    if (m_Player_Total.m_ePlayer_Move_State == E_PLAYER_MOVE_STATE.POSSIBLE || m_Player_Total.m_ePlayer_Move_State == E_PLAYER_MOVE_STATE.POSSIBLE_BACK)
                    {
                        this.transform.Translate(Vector3.back * m_fSpeed * Time.deltaTime);
                        Behaviour_Move_Back();

                        m_Player_Total.m_PhotonView.RPC("RPC_Move", RpcTarget.Others, -1);
                    }
                }
            }

            if (m_bMove == false)
            {
                if (m_ePlayer_Animation == E_PLAYER_ANIMATION.MOVE_FORWARD || m_ePlayer_Animation == E_PLAYER_ANIMATION.MOVE_BACK)
                {
                    StopCoroutine(m_cProcess_Behaviour);
                    m_Transform_Spine.localRotation = Quaternion.Euler(m_Vector_Spine_Init);
                    m_Transform_RightArm.localRotation = Quaternion.Euler(m_Vector_RightArm_Init);
                    m_Transform_LeftArm.localRotation = Quaternion.Euler(m_Vector_LeftArm_Init);

                    m_cProcess_Behaviour = null;

                    m_ePlayer_Animation = E_PLAYER_ANIMATION.NULL;
                }
            }


            if (Input.GetKeyUp(KeyCode.Keypad8))
            {
                if (m_ePlayer_Animation != E_PLAYER_ANIMATION.THROWING)
                {
                    if (m_cProcess_Behaviour != null)
                    {
                        StopCoroutine(m_cProcess_Behaviour);
                        m_cProcess_Behaviour = null;
                    }

                    Behaviour_Throwing();
                }
                //m_Transform_RightArm.Rotate(Vector3.right * 5000 * Time.deltaTime);
                //m_Transform_LeftArm.Rotate(Vector3.right * 5000 * Time.deltaTime);
            }
            if (Input.GetKeyUp(KeyCode.Keypad2))
            {
                m_Transform_RightArm.Rotate(Vector3.left * 5000 * Time.deltaTime);
                m_Transform_LeftArm.Rotate(Vector3.left * 5000 * Time.deltaTime);
            }
        }
        else
        {
            // Debug.Log("게임 플레이 불가.");
        }
    }

    public void Behaviour_Move_Forward()
    {
        if (m_cProcess_Behaviour == null)
            m_cProcess_Behaviour = StartCoroutine(Process_Move_Forward());
        m_bMove = true;
        m_ePlayer_Animation = E_PLAYER_ANIMATION.MOVE_FORWARD;
    }
    public void Behaviour_Move_Back()
    {
        if (m_cProcess_Behaviour == null)
            m_cProcess_Behaviour = StartCoroutine(Process_Move_Back());
        m_bMove = true;
        m_ePlayer_Animation = E_PLAYER_ANIMATION.MOVE_BACK;
    }
    public void Behaviour_Throwing()
    {
        m_cProcess_Behaviour = StartCoroutine(Process_Throwing());
    }

    public Coroutine m_cProcess_Behaviour = null;
    IEnumerator Process_Throwing()
    {
        m_Transform_Spine.localRotation = Quaternion.Euler(m_Vector_Spine_Move_Start);
        m_Transform_RightArm.localRotation = Quaternion.Euler(m_Vector_RightArm_Throwing_Start);
        m_ePlayer_Animation = E_PLAYER_ANIMATION.THROWING;

        float time = 10;
        while (time > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            m_Transform_Spine.Rotate(Vector3.right * 25 * Time.deltaTime);
            m_Transform_RightArm.Rotate(Vector3.right * 680 * Time.deltaTime);
            time -= 1;
        }

        m_cProcess_Behaviour = null;

        m_Transform_Spine.localRotation = Quaternion.Euler(m_Vector_Spine_Init);
        m_Transform_RightArm.localRotation = Quaternion.Euler(m_Vector_RightArm_Init);
        m_ePlayer_Animation = E_PLAYER_ANIMATION.NULL;
    }
    IEnumerator Process_Move_Forward()
    {
        m_Transform_Spine.localRotation = Quaternion.Euler(m_Vector_Spine_Move_Start);
        m_Transform_RightArm.localRotation = Quaternion.Euler(m_Vector_RightArm_Move_Start);
        m_Transform_LeftArm.localRotation = Quaternion.Euler(m_Vector_LeftArm_Move_Start);

        float time = 7;
        while (time > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            m_Transform_Spine.Rotate(Vector3.right * 50 * Time.deltaTime);
            m_Transform_RightArm.Rotate(Vector3.right * 500 * Time.deltaTime);
            m_Transform_LeftArm.Rotate(Vector3.right * 500 * Time.deltaTime);
            time -= 1;
        }

        m_cProcess_Behaviour = null;

        m_Transform_Spine.localRotation = Quaternion.Euler(m_Vector_Spine_Init);
        m_Transform_RightArm.localRotation = Quaternion.Euler(m_Vector_RightArm_Init);
        m_Transform_LeftArm.localRotation = Quaternion.Euler(m_Vector_LeftArm_Init);
        m_ePlayer_Animation = E_PLAYER_ANIMATION.NULL;
    }
    IEnumerator Process_Move_Back()
    {
        m_Transform_Spine.localRotation = Quaternion.Euler(m_Vector_Spine_Move_Start);
        m_Transform_RightArm.localRotation = Quaternion.Euler(m_Vector_RightArm_Init);
        m_Transform_LeftArm.localRotation = Quaternion.Euler(m_Vector_LeftArm_Init);

        float time = 7;
        while (time > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            m_Transform_Spine.Rotate(Vector3.left * 25 * Time.deltaTime);
            m_Transform_RightArm.Rotate(Vector3.left * 250 * Time.deltaTime);
            m_Transform_LeftArm.Rotate(Vector3.left * 250 * Time.deltaTime);
            time -= 1;
        }

        m_cProcess_Behaviour = null;

        m_Transform_Spine.localRotation = Quaternion.Euler(m_Vector_Spine_Init);
        m_Transform_RightArm.localRotation = Quaternion.Euler(m_Vector_RightArm_Move_Start);
        m_Transform_LeftArm.localRotation = Quaternion.Euler(m_Vector_LeftArm_Move_Start);
        m_ePlayer_Animation = E_PLAYER_ANIMATION.NULL;
    }

}
