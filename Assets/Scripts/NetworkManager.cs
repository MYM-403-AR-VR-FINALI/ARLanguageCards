using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Oda Kurma Paneli")]
    public InputField roomNameInput; // Oda ismini yazacaðýn kutu
    public Button createButton;      // Yanýndaki "Kur" butonu

    [Header("Oda Listesi")]
    public Transform contentObject;  // ScrollView içindeki Content
    public GameObject roomItemPrefab;// Project'teki o Mavi Küp (Prefab)

    [Header("Durum")]
    public Text statusText;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        if (statusText) statusText.text = "Sunucuya baðlanýlýyor...";
        if (createButton) createButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        if (statusText) statusText.text = "Lobiye giriliyor...";
    }

    public override void OnJoinedLobby()
    {
        if (statusText) statusText.text = "Lobiye Girildi.";
        if (createButton) createButton.interactable = true;
    }

    public void CreateRoom()
    {
        if (roomNameInput && !string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions { MaxPlayers = 2 };
            PhotonNetwork.CreateRoom(roomNameInput.text, options);
        }
    }

    public override void OnJoinedRoom()
    {
        // Odaya girince (kurunca veya katýlýnca) burasý çalýþýr
        if (PhotonNetwork.IsMasterClient)
        {
            // Sahne yükleme iþini sadece Master (Kuran) yapar
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    // Diðer oyuncu geldiðinde
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    // LÝSTE GÜNCELLEME SÝHRÝ
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Temizlik
        foreach (Transform child in contentObject) Destroy(child.gameObject);

        // Yeniden Dizme
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList || !room.IsOpen || !room.IsVisible) continue;

            // Prefabý yarat
            GameObject newRow = Instantiate(roomItemPrefab, contentObject);
            // Ýsmini yazdýr
            newRow.GetComponent<RoomItem>().SetRoomName(room.Name);
        }
    }
}