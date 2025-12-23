using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Oda Kurma Paneli")]
    public InputField roomNameInput;
    public Button createButton;

    [Header("Oda Listesi")]
    public Transform contentObject;
    public GameObject roomItemPrefab;

    [Header("Durum")]
    public Text statusText;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        statusText.text = "Sunucuya baðlanýlýyor...";
        createButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        statusText.text = "Lobiye giriliyor...";
        UnityEngine.Debug.Log("Master Server'a baðlandý");
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Lobiye girildi. Oda kurabilirsin.";
        createButton.interactable = true;
        UnityEngine.Debug.Log("Lobiye girildi");
    }

    // --- ODA KURMA ---
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
            return;

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomNameInput.text, options);
        statusText.text = "Oda kuruluyor...";
        UnityEngine.Debug.Log("Oda kuruluyor: " + roomNameInput.text);
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Odaya girildi! Oyun baþlýyor...";
        UnityEngine.Debug.Log("Odaya girildi");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UnityEngine.Debug.Log("Oyuncu katýldý: " + newPlayer.NickName);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    // --- ODA LÝSTESÝ ---
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in contentObject)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList || !room.IsOpen || !room.IsVisible)
                continue;

            GameObject newRow = Instantiate(roomItemPrefab, contentObject);
            newRow.GetComponent<RoomItem>().SetRoomName(room.Name);
        }
    }
}
