using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] static Dictionary<string, RoomInfo> m_sd_Room_Current;

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
            Debug.Log("현재 만들어진 방 수: " + m_sd_Room_Current.Count + " / " + PhotonNetwork.CountOfRooms);
            foreach(KeyValuePair<string, RoomInfo> room in m_sd_Room_Current)
            {
                Debug.Log(room.Key + " / " + room.Value.Name);
            }
        }
    }

    // 콜백.
    public override void OnConnected()
    {
        Debug.Log("1. 네트워크에 접속중입니다. . .");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("2. 네트워크에 접속하였습니다.");

        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("3. 로비에 접속하였습니다.");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("현재 방 목록: " + PhotonNetwork.CountOfRooms.ToString());
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
        Debug.Log("4. 방에 접속하였습니다.: " + PhotonNetwork.CurrentRoom.Name);

        PhotonNetwork.LocalPlayer.NickName = "Player_" + Random.Range(1, 101).ToString();
        Debug.Log("플레이어 닉네임: " + PhotonNetwork.LocalPlayer.NickName);

        Debug.Log("현재 방에 참여중인 인원: " + PhotonNetwork.PlayerList.Length.ToString());
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            Debug.Log(i + 1 + ": " + PhotonNetwork.PlayerList[i].NickName);

        if (m_sd_Room_Current.ContainsKey(PhotonNetwork.CurrentRoom.Name) != true)
            m_sd_Room_Current.Add(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom);

        //PhotonNetwork.Instantiate("Cube", new Vector3(0, 0, 0), Quaternion.identity, 0);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("방에서 나왔습니다.");
    }

    public override void OnLeftLobby()
    {
        Debug.Log("로비에서 나왔습니다.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("네트워크가 연결 해제되었습니다.: " + cause);
    }
}
