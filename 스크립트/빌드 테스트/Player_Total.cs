using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_PLAYER_STATE { NULL, ALIVE, DEATH }
public enum E_PLAYER_MOVE_STATE { IMPOSSIBLE, POSSIBLE, POSSIBLE_FOWARD, POSSIBLE_BACK }
public enum E_PLAYER_POSITION { P1, P2 }

public class Player_Total : MonoBehaviour
{
    //private static Player_Total instance = null;
    //public static Player_Total Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            return null;
    //        }
    //        return instance;
    //    }
    //}

    public PhotonView m_PhotonView;
    [Space(20)] 
    public Player_Collider m_Player_Collider;
    public Player_Move m_Player_Move;
    [Space(20)]
    public E_PLAYER_STATE m_ePlayer_State;
    public E_PLAYER_MOVE_STATE m_ePlayer_Move_State;
    public E_PLAYER_POSITION m_ePlayer_Position;

    public string m_sPlayer_ID = string.Empty;
    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;

        //    DontDestroyOnLoad(this.gameObject);
        //}
        //else
        //{
        //    Destroy(this.gameObject);
        //}

        m_PhotonView = this.gameObject.GetComponent<PhotonView>();

        m_Player_Collider = this.gameObject.GetComponent<Player_Collider>();
        m_Player_Move = this.gameObject.GetComponent<Player_Move>();

        m_ePlayer_State = E_PLAYER_STATE.ALIVE;
        m_ePlayer_Move_State = E_PLAYER_MOVE_STATE.POSSIBLE;
    }

    public void InitialSet(E_PLAYER_POSITION epp)
    {
        m_ePlayer_Position = epp;
    }

    [PunRPC]
    public void RPC_GameStart_Player_Total()
    {
        m_Player_Move.m_Rigidbody.useGravity = true;
    }
}
