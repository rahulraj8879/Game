using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("login ui panel")]
    public InputField playerNameInput;
    [Header("connection ui panel")]
    public Text connectionStatus;

    public GameObject LoginUi;
    public GameObject GameOptionUi;
    public GameObject CreateRoomUi;
    public GameObject InsideRoomUi;
    public GameObject RoomListUi;
    public GameObject JoinRandomRoomUi;
    public InputField roomNameTextInput;
    public InputField maxPlayersTextInput;
    public Dictionary<string, RoomInfo> cachedroomList;
    public Dictionary<string, GameObject> roomListGameobjects;
    public Dictionary<int, GameObject> playerListGameobjects;
    public Text roomInfoText;
    public GameObject playerlist;
    public GameObject playerlistParent;
    public GameObject startBtn;


    public GameObject roomlistEntryPrefab;
    public GameObject roomlistEntryPrefabParent;
    
    
    // Start is called before the first frame update
    void Start()
    {
        ActivateUI(LoginUi.name);
        cachedroomList = new Dictionary<string, RoomInfo>();
        roomListGameobjects = new Dictionary<string, GameObject>();
        PhotonNetwork.AutomaticallySyncScene = true;
        
       
    }

    // Update is called once per frame
    void Update()
    {
        connectionStatus.text = " Connection Status :" + PhotonNetwork.NetworkClientState;
    }


    #region public method
    public void OnLoginBtnClicked()
    {
        string name = playerNameInput.text;
        if(!string.IsNullOrEmpty(name)){
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.ConnectUsingSettings();
        }
        
    }
 
    public void ActivateUI(string PanelToBeActivated)
    {
        LoginUi.SetActive(PanelToBeActivated.Equals(LoginUi.name));
        GameOptionUi.SetActive(PanelToBeActivated.Equals(GameOptionUi.name));
        CreateRoomUi.SetActive(PanelToBeActivated.Equals(CreateRoomUi.name));
        InsideRoomUi.SetActive(PanelToBeActivated.Equals(InsideRoomUi.name));
        RoomListUi.SetActive(PanelToBeActivated.Equals(RoomListUi.name));
        JoinRandomRoomUi.SetActive(PanelToBeActivated.Equals(JoinRandomRoomUi.name));
    }
    public void onCreateRoomClicked()
    {
        string roomname = roomNameTextInput.text;
        if (roomNameTextInput.text == null)
        {
            roomname = " Room " + Random.Range(1,20);
        }
        string maxplayer = maxPlayersTextInput.text;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(maxplayer);

        PhotonNetwork.CreateRoom(roomname,roomOptions);

       



    }

    void onRoomJoinClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
        ActivateUI(InsideRoomUi.name);
    }

    void clearRoomList()
    {
        foreach (var roomlistGameObject in roomListGameobjects.Values)
        {
            Destroy(roomlistGameObject);
        }
        roomListGameobjects.Clear();
    }
    public void onBackBtnClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivateUI(GameOptionUi.name);

    }
    public void OnShowListBtnClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivateUI(RoomListUi.name);
    }
    public void onCancelBtnClicked()
    {
        ActivateUI(GameOptionUi.name);
    }

    public void onLeaveBtn()
    {
        PhotonNetwork.LeaveRoom();
      
       // ActivateUI(GameOptionUi.name);
    }
    public void JoinRandomRoomClicked()
    {
        ActivateUI(JoinRandomRoomUi.name);
        PhotonNetwork.JoinRandomRoom();
    }
    public void startGameClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
    #endregion


    #region overriden Method
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name+" has been created");
        
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerlistObjects = Instantiate(playerlist);
        playerlistObjects.transform.SetParent(playerlistParent.transform);
        playerlistObjects.transform.localScale = Vector3.one;


        playerlistObjects.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerlistObjects.transform.Find("PlayerIndicator").gameObject.SetActive(true);
        }
        else
        {
            playerlistObjects.transform.Find("PlayerIndicator").gameObject.SetActive(false);
        }
        playerListGameobjects.Add(newPlayer.ActorNumber, playerlistObjects);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomInfoText.text = " Room Name : " + PhotonNetwork.CurrentRoom.Name + " "
             + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
             PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameobjects[otherPlayer.ActorNumber].gameObject);
        playerListGameobjects.Remove(otherPlayer.ActorNumber);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startBtn.SetActive(true);
        }
    }
    public override void OnLeftRoom()
    {
        ActivateUI(GameOptionUi.name);
        foreach (GameObject playerlistGameobjectt in playerListGameobjects.Values)
        {
            Destroy(playerlistGameobjectt);
        }
        playerListGameobjects.Clear();
        playerListGameobjects = null;
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startBtn.SetActive(true);
        }
        else
        {
            startBtn.SetActive(false);
        }
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined "+ PhotonNetwork.CurrentRoom.Name);
        ActivateUI(InsideRoomUi.name);
        roomInfoText.text = " Room Name : "+PhotonNetwork.CurrentRoom.Name + " "
            + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
            PhotonNetwork.CurrentRoom.MaxPlayers;

        if (playerListGameobjects==null)
        {
            playerListGameobjects = new Dictionary<int, GameObject>();
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerlistObjects = Instantiate(playerlist);
            playerlistObjects.transform.SetParent(playerlistParent.transform);
            playerlistObjects.transform.localScale = Vector3.one;


            playerlistObjects.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerlistObjects.transform.Find("PlayerIndicator").gameObject.SetActive(true);
            }
            else {
                playerlistObjects.transform.Find("PlayerIndicator").gameObject.SetActive(false);
            }
            playerListGameobjects.Add(player.ActorNumber, playerlistObjects);

        }

    }
  
    public override void OnLeftLobby()
    {
        clearRoomList();
        cachedroomList.Clear();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        clearRoomList();

        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room.Name);

            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (cachedroomList.ContainsKey(room.Name))
                {
                    cachedroomList.Remove(room.Name);
                }
            }
            else
            {
                if (cachedroomList.ContainsKey(room.Name))
                {
                    cachedroomList[room.Name] = room;
                }
                else
                {
                    cachedroomList.Add(room.Name, room);
                }
            }
            
        }


        foreach (RoomInfo room in cachedroomList.Values)
        {
            GameObject roomlistGameObject = Instantiate(roomlistEntryPrefab);
            roomlistGameObject.transform.SetParent(roomlistEntryPrefabParent.transform);
            roomlistGameObject.transform.localScale = Vector3.one;
            Debug.Log(room.Name + " Inintialized ");

            roomlistGameObject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
            roomlistGameObject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount+" / " + room.MaxPlayers;
            roomlistGameObject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(()=>onRoomJoinClicked(room.Name));
            roomListGameobjects.Add(room.Name,roomlistGameObject);
        }
    }
    public override void OnConnected()
    {
        Debug.Log(" connected to internet");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " connected to internet");
        ActivateUI(GameOptionUi.name);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        string roomName = "Room " + Random.Range(1,1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomName,roomOptions);
    }
    #endregion
}
