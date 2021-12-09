using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private string mapType;

    public TextMeshProUGUI OccupancyRateText_ForSchool;
    public TextMeshProUGUI OccupancyRateText_ForOutdoor;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region UI Callback Methods

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnEnterButtonClicked_Outdoor()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_OUTDOOR;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_Classroom()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_CLASSROOM;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType} };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties,0);
    }

    #endregion

    #region Photon Callback Methods

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateAndJoinRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Servers again");
        PhotonNetwork.JoinLobby();
    }


    public override void OnCreatedRoom()
    {
        Debug.Log("A room is created with the name: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("The local Player: "+ PhotonNetwork.NickName+" joined to "+ PhotonNetwork.CurrentRoom.Name + "Player Count "+ PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MultiplayerVRConstants.MAP_TYPE_KEY))
        {
            object mapType;
            if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MAP_TYPE_KEY, out mapType))
            {
                Debug.Log("Joined Room with the Map: " + (string)mapType);
                if((string)mapType == MultiplayerVRConstants.MAP_TYPE_CLASSROOM)
                {
                    PhotonNetwork.LoadLevel("Whiteboard");
                }
                if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_OUTDOOR)
                {
                    PhotonNetwork.LoadLevel("World_Outdoor");
                }
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to: "+"Player count: "+ PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count == 0)
        {
            OccupancyRateText_ForOutdoor.text = 0 + "/" + 20;
            OccupancyRateText_ForSchool.text = 0 + "/" + 20;
        }

        foreach(RoomInfo room in roomList)
        {
            Debug.Log(room.Name);
            if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_OUTDOOR))
            {
                Debug.Log("Room is Outdoor map. Player Count is: " + room.PlayerCount);
                OccupancyRateText_ForOutdoor.text = room.PlayerCount + " / " + 20;
            }
            else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_CLASSROOM))
            {
                Debug.Log("Room is School map. Player Count is: " + room.PlayerCount);
                OccupancyRateText_ForSchool.text = room.PlayerCount + " / " + 20;
            }
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the lobby");
    }

    #endregion

    #region Private Methods

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room_" + mapType + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        string[] roomPropsInLobby = { MultiplayerVRConstants.MAP_TYPE_KEY };

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { {MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };


        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;

        PhotonNetwork.CreateRoom(randomRoomName,  roomOptions);
    }

    #endregion
}
