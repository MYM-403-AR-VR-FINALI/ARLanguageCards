using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Lobi Arayüz Elemanlarý")]
    public InputField roomInputField; // Oda adýný yazacaðýn kutu
    public Text statusText;           // Ekranda "Lobiye Baðlanýldý" yazacak yer
    public Button joinButton;         // Odaya girmeni saðlayan o büyük buton

    void Start()
    {
        // Baþlangýçta butonu kapatýyoruz ki "JoinOrCreateRoom failed" hatasý alma
        if (joinButton != null) joinButton.interactable = false;

        // Eðer sunucuya baðlý deðilsek baðlanmayý dene
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            UpdateStatus("Sunucuya baðlanýlýyor...");
        }
    }

    // --- PHOTON GERÝ BÝLDÝRÝMLERÝ ---

    public override void OnConnectedToMaster()
    {
        UpdateStatus("Sunucu hazýr. Lobiye giriliyor...");
        PhotonNetwork.JoinLobby(); // Master'a baðlandýktan sonra lobiye girmek þarttýr
    }

    public override void OnJoinedLobby()
    {
        UpdateStatus("Lobiye baðlanýldý. Odaya girebilirsin.");
        // Baðlantý tam saðlandýðýnda butonu týklanabilir yapýyoruz
        if (joinButton != null) joinButton.interactable = true;
    }

    // --- BUTONUN ÇALIÞTIRACAÐI FONKSÝYON ---

    public void JoinOrCreateRoom()
    {
        // InputField boþsa otomatik isim veriyoruz
        string rName = roomInputField != null && !string.IsNullOrEmpty(roomInputField.text) ? roomInputField.text : "GenelOda";

        // Oda ayarlarý: Maksimum 2 kiþi
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };

        // Odaya gir veya yoksa kur
        PhotonNetwork.JoinOrCreateRoom(rName, options, TypedLobby.Default);
        UpdateStatus("Odaya giriliyor...");
    }

    public override void OnJoinedRoom()
    {
        UpdateStatus("Odaya girildi. Arkadaþýn bekleniyor...");
        CheckPlayers();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Yeni bir oyuncu geldiðinde kontrol et
        CheckPlayers();
    }

    void CheckPlayers()
    {
        // Odanýn dolmasýný bekler: Sadece odadaki oyuncu sayýsý tam 2 olduðunda 
        // ve iþlemi odayý kuran kiþi (Master) baþlattýðýnda sahne yüklenir.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            // Build Settings'deki "GameScene" sahnesini tüm oyuncular için yükler
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    void UpdateStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
        Debug.Log(msg);
    }
}