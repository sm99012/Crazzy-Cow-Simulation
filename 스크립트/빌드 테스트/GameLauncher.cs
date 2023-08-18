using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] static Dictionary<string, RoomInfo> m_sd_Room_Current;
    [SerializeField] GameObject m_gVector_PlayerPosition_P1;
    [SerializeField] GameObject m_gVector_PlayerPosition_P2;
    public TextMeshProUGUI m_TMP;

    Player_Total m_Player = null;

    private void Start()
    {
        Screen.SetResolution(600, 600, false);

        m_sd_Room_Current = new Dictionary<string, RoomInfo>();

        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Delete))
        {
            if (PhotonNetwork.InRoom == true)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                if (PhotonNetwork.InLobby == true)
                {
                    PhotonNetwork.LeaveLobby();
                }
                else
                {
                    PhotonNetwork.Disconnect();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            if (PhotonNetwork.InRoom == true)
            {

            }
            else
            {
                if (PhotonNetwork.InLobby == true)
                {
                    RoomOptions room = new RoomOptions();
                    room.MaxPlayers = 2;

                    bool b_Create_Room = true;
                    string s_Room_Name = string.Empty;
                    foreach (KeyValuePair<string, RoomInfo> roominfo in m_sd_Room_Current)
                    {
                        if (roominfo.Value.PlayerCount < roominfo.Value.MaxPlayers)
                        {
                            b_Create_Room = false;
                            s_Room_Name = roominfo.Key;
                        }

                        if (b_Create_Room == false)
                            break;
                    }

                    if (b_Create_Room == true)
                    {
                        int randomnum = Random.Range(1, 101);
                        while (m_sd_Room_Current.ContainsKey("Room_" + randomnum.ToString()) == true)
                        {
                            randomnum = Random.Range(1, 101);
                        }

                        PhotonNetwork.CreateRoom("Room_" + randomnum.ToString(), room, null);
                    }
                    else
                    {
                        PhotonNetwork.JoinRoom(s_Room_Name);
                    }

                    //PhotonNetwork.JoinOrCreateRoom("Room_" + randomnum.ToString(), room, null);
                }
                else
                {
                    if (PhotonNetwork.IsConnected == true)
                    {
                        PhotonNetwork.JoinLobby();
                    }
                    else
                    {
                        PhotonNetwork.ConnectUsingSettings();
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("���� ������� �� ��: " + m_sd_Room_Current.Count + " / " + PhotonNetwork.CountOfRooms);
            foreach (KeyValuePair<string, RoomInfo> room in m_sd_Room_Current)
            {
                Debug.Log(room.Key + " / " + room.Value.Name);
            }
        }
    }

    // �ݹ�.
    public override void OnConnected()
    {
        Debug.Log("1. ��Ʈ��ũ�� �������Դϴ�. . .");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("2. ��Ʈ��ũ�� �����Ͽ����ϴ�.");

        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("3. �κ� �����Ͽ����ϴ�.");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("���� �� ���: " + PhotonNetwork.CountOfRooms.ToString());
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList == true)
            {
                m_sd_Room_Current.Remove(roomList[i].Name);
            }
            else
            {
                if (m_sd_Room_Current.ContainsKey(roomList[i].Name) == false)
                {
                    m_sd_Room_Current.Add(roomList[i].Name, roomList[i]);
                }
            }
        }
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("4. �濡 �����Ͽ����ϴ�.: " + PhotonNetwork.CurrentRoom.Name);

        PhotonNetwork.LocalPlayer.NickName = "Player_" + Random.Range(1, 101).ToString();
        Debug.Log("�÷��̾� �г���: " + PhotonNetwork.LocalPlayer.NickName + PhotonNetwork.PhotonViews.Length);

        //Debug.Log("���� �濡 �������� �ο�: " + PhotonNetwork.PlayerList.Length.ToString());
        //for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        //    Debug.Log(i + 1 + ": " + PhotonNetwork.PlayerList[i].NickName);

        if (m_sd_Room_Current.ContainsKey(PhotonNetwork.CurrentRoom.Name) != true)
            m_sd_Room_Current.Add(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom);

        //if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        if (PhotonNetwork.IsMasterClient == true)
        {
            GameObject gm = PhotonNetwork.Instantiate("Player_P1", m_gVector_PlayerPosition_P1.transform.position + Vector3.down * 0.4f, Quaternion.Euler(new Vector3(0, 270, 0)));
            gm.GetComponent<Rigidbody>().useGravity = false;
            m_Player = gm.GetComponent<Player_Total>();
            m_Player.m_sPlayer_ID = PhotonNetwork.LocalPlayer.UserId;
            Debug.Log(gm.GetPhotonView().ViewID);
        }
        else
        {
            GameObject gm = PhotonNetwork.Instantiate("Player_P2", m_gVector_PlayerPosition_P2.transform.position + Vector3.down * 0.4f, Quaternion.Euler(new Vector3(0, 90, 0)));
            gm.GetComponent<Rigidbody>().useGravity = false;
            m_Player = gm.GetComponent<Player_Total>();
            m_Player.m_sPlayer_ID = PhotonNetwork.LocalPlayer.UserId;
            Debug.Log(gm.GetPhotonView().ViewID);
        }

        //PhotonNetwork.Instantiate("Cube", new Vector3(0, 0, 0), Quaternion.identity, 0);

        if (PhotonNetwork.IsMasterClient != true)
        {
            if (PhotonNetwork.CurrentRoom.Players.Count == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                m_TMP.text = "GameStart!";
                StartCoroutine(Process_GameStart());
            }
        }

        m_TMP.text += PhotonNetwork.CurrentRoom.Players.Count + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //m_TMP.text += "OnPlayerEnteredRoom :" + PhotonNetwork.CurrentRoom.Players.Count + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (PhotonNetwork.IsMasterClient == true)
        {
            if (PhotonNetwork.CurrentRoom.Players.Count == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                m_TMP.text = "GameStart!";
                StartCoroutine(Process_GameStart());
            }
        }
    }
    IEnumerator Process_GameStart()
    {
        yield return new WaitForSeconds(3f);
        m_Player.m_PhotonView.RPC("RPC_GameStart_Player_Total", RpcTarget.All);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("�濡�� ���Խ��ϴ�.");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftLobby()
    {
        Debug.Log("�κ񿡼� ���Խ��ϴ�.");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("��Ʈ��ũ�� ���� �����Ǿ����ϴ�.: " + cause);
    }
}
