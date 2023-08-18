using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Collider : MonoBehaviour
{
    Player_Total m_Player_Total;

    [SerializeField] GameObject m_gPlayerCollider;
    [SerializeField] List<GameObject> m_gList_PlayerCollider;
    [SerializeField] List<BoxCollider> m_ColliderList_PlayerCollider;
    [SerializeField] List<Vector3> m_VectorList;
    [SerializeField] List<Vector3> m_VectorList_Size;
    [SerializeField] List<Vector3> m_VectorList_Rotation;
    [Space(20)]
    [SerializeField] GameObject m_gPlayer_RagDoll;

    Collider[] m_ColliderAry;
    [SerializeField] int m_nLayer;
    int m_nLayer_Goat;
    int m_nLayer_Wall;

    private void Awake()
    {
        m_Player_Total = this.gameObject.GetComponent<Player_Total>();

        m_gList_PlayerCollider = new List<GameObject>();
        m_ColliderList_PlayerCollider = new List<BoxCollider>();
        m_VectorList = new List<Vector3>();
        m_VectorList_Size = new List<Vector3>();
        m_VectorList_Rotation = new List<Vector3>();

        if (m_gPlayerCollider != null)
        {
            for (int i = 0; i < m_gPlayerCollider.transform.childCount; i++)
            {
                m_gList_PlayerCollider.Add(m_gPlayerCollider.transform.GetChild(i).gameObject);
                m_ColliderList_PlayerCollider.Add(m_gList_PlayerCollider[i].GetComponent<BoxCollider>());
                m_VectorList.Add(Vector3.zero);
                m_VectorList_Size.Add(Vector3.zero);
                m_VectorList_Rotation.Add(Vector3.zero);
            }
        }

        m_nLayer = 1 << LayerMask.NameToLayer("Goat") | 1 << LayerMask.NameToLayer("Wall");
        m_nLayer_Goat = 1 << LayerMask.NameToLayer("Goat");
        m_nLayer_Wall = 1 << LayerMask.NameToLayer("Wall");
    }

    private void Update()
    {
        //if (m_Player_Total.m_PhotonView.IsMine == true && PhotonNetwork.IsConnected == true && PhotonNetwork.InRoom == true)
        if (PhotonNetwork.IsConnected == true && PhotonNetwork.InRoom == true)
        {
            m_Player_Total.m_ePlayer_Move_State = E_PLAYER_MOVE_STATE.POSSIBLE;

            for (int i = 0; i < m_ColliderList_PlayerCollider.Count; i++)
            {
                m_VectorList[i] = (m_ColliderList_PlayerCollider[i].bounds.center);
                m_VectorList_Size[i] = (m_ColliderList_PlayerCollider[i].size * (float)(7f / 20f));
                m_VectorList_Rotation[i] = (m_ColliderList_PlayerCollider[i].transform.localRotation.eulerAngles + this.transform.localRotation.eulerAngles);

                m_ColliderAry = Physics.OverlapBox(m_VectorList[i],
                    (m_ColliderList_PlayerCollider[i].size) * (float)(7f / 20f) / 2,
                    Quaternion.Euler(m_ColliderList_PlayerCollider[i].transform.localRotation.eulerAngles + this.transform.localRotation.eulerAngles),
                    m_nLayer);

                //m_ColliderAry = Physics.OverlapBox(m_ColliderList_PlayerCollider[i].center + m_gList_PlayerCollider[i].transform.position,
                //    (m_ColliderList_PlayerCollider[i].size) * (float)(7f / 20f) / 2,
                //    Quaternion.Euler(m_ColliderList_PlayerCollider[i].transform.localRotation.eulerAngles + this.transform.localRotation.eulerAngles),
                //    m_nLayer);

                //m_ColliderAry = Physics.OverlapBox(m_ColliderList_PlayerCollider[i].center + m_gList_PlayerCollider[i].transform.position,
                //    (m_ColliderList_PlayerCollider[i].size) * (float)(7f / 20f) / 2,
                //    Quaternion.identity,
                //    m_nLayer);

                //Debug.Log(m_ColliderList_PlayerCollider[i].transform.localRotation.eulerAngles + this.transform.localRotation.eulerAngles);
                //Debug.Log(m_gList_PlayerCollider[i].transform.position + " / " + m_ColliderList_PlayerCollider[i].bounds.center);
                //Debug.Log("박스 콜라이더 크기 " + i + ": " + m_ColliderList_PlayerCollider[i].size);

                if (m_ColliderAry.Length > 0)
                {
                    for (int j = 0; j < m_ColliderAry.Length; j++)
                    {
                        //Debug.Log(m_ColliderList_PlayerCollider[i].name + " / " + m_ColliderAry[j].gameObject.name);
                        if (m_Player_Total.m_ePlayer_State == E_PLAYER_STATE.ALIVE)
                        {
                            if (m_ColliderAry[j].gameObject.tag == "Wall")
                            {
                                if (m_ColliderAry[j].gameObject.name == "Wall_Forward")
                                {
                                    m_Player_Total.m_ePlayer_Move_State = E_PLAYER_MOVE_STATE.POSSIBLE_BACK;
                                }
                                else if (m_ColliderAry[j].gameObject.name == "Wall_Back")
                                {
                                    m_Player_Total.m_ePlayer_Move_State = E_PLAYER_MOVE_STATE.POSSIBLE_FOWARD;
                                }
                            }
                            else
                            {
                                m_Player_Total.m_ePlayer_State = E_PLAYER_STATE.DEATH;
                                m_Player_Total.m_ePlayer_Move_State = E_PLAYER_MOVE_STATE.IMPOSSIBLE;
                                FlyAway((this.transform.position - m_ColliderList_PlayerCollider[i].gameObject.transform.position));
                            }
                        }
                    }
                }
            }
        }
        else
        {
            // Debug.Log("게임 플레이 불가.");
        }
        //0
        //m_VectorList[0] = (m_ColliderList_PlayerCollider[0].center + m_gList_PlayerCollider[0].transform.position);
        //m_ColliderAry = Physics.OverlapBox(m_ColliderList_PlayerCollider[0].center + m_gList_PlayerCollider[0].transform.position,
        //    m_ColliderList_PlayerCollider[0].transform.localScale / 2,
        //    m_ColliderList_PlayerCollider[0].transform.localRotation * m_gList_PlayerCollider[0].transform.localRotation * this.transform.localRotation,
        //    m_nLayer);

        //if (m_ColliderAry.Length > 0)
        //{
        //    for (int j = 0; j < m_ColliderAry.Length; j++)
        //    {
        //        Debug.Log(m_ColliderList_PlayerCollider[0].name + " / " + m_ColliderAry[j].gameObject.name);
        //    }
        //}
        //// 1
        //m_VectorList[1] = (m_ColliderList_PlayerCollider[1].center + m_gList_PlayerCollider[1].transform.position);
        //m_ColliderAry = Physics.OverlapBox(m_ColliderList_PlayerCollider[1].center + m_gList_PlayerCollider[1].transform.position,
        //    m_ColliderList_PlayerCollider[1].transform.localScale / 2,
        //    m_ColliderList_PlayerCollider[1].transform.localRotation * m_gList_PlayerCollider[1].transform.localRotation * this.transform.localRotation,
        //    m_nLayer);

        //if (m_ColliderAry.Length > 0)
        //{
        //    for (int j = 0; j < m_ColliderAry.Length; j++)
        //    {
        //        Debug.Log(m_ColliderList_PlayerCollider[1].name + " / " + m_ColliderAry[j].gameObject.name);
        //    }
        //}
    }

    void FlyAway(Vector3 dir)
    {
        GameObject gm = Instantiate(m_gPlayer_RagDoll);
        gm.transform.position = this.transform.position;
        
        gm.GetComponent<Player_RagDoll>().FlyAway(new Vector3(dir.x * 10, 0, dir.z * 10) + Vector3.up);

        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        if (m_VectorList != null)
        for (int i = 0; i <m_VectorList.Count; i++)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.identity;
            Gizmos.matrix = rotationMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_VectorList[i], 0.2f);
            ////Debug.Log(m_VectorList[i]);
            //rotationMatrix = Matrix4x4.TRS(m_VectorList[i], m_gList_PlayerCollider[i].transform.localRotation, Vector3.one);
            //Gizmos.matrix = rotationMatrix;
            ////Gizmos.matrix = m_gList_PlayerCollider[i].transform.localToWorldMatrix;

            //Gizmos.color = Color.blue;
            //Gizmos.DrawCube(m_VectorList[i], m_VectorList_Size[i]);
        }
    }
}
