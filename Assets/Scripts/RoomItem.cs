using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class RoomItem : MonoBehaviour
{
    public Text roomNameText;

    public void SetRoomName(string roomName)
    {
        roomNameText.text = roomName;
    }

    public void OnClick_Join()
    {
        UnityEngine.Debug.Log(roomNameText.text + " odasýna giriliyor...");
        PhotonNetwork.JoinRoom(roomNameText.text);
    }
}
