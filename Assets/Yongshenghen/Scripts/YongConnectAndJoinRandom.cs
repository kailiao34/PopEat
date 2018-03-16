using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script automatically connects to Photon (using the settings file),
/// tries to join a random room and creates one if none was found (which is ok).
/// </summary>
public class YongConnectAndJoinRandom : Photon.MonoBehaviour
{
    public InputField inp;
    /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
    public bool AutoConnect = true;

    public byte Version = 1;

    /// <summary>if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()</summary>
    private bool ConnectInUpdate = true;

    public virtual void Start()
    {

        PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
    }

    public virtual void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        //    {
        //        Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
        //        ConnectInUpdate = false;
        //        PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        //    {
        //        Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
        //        ConnectInUpdate = false;
        //        PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
        //    }
        //}
    }

    /// <summary>
    /// 創建房間
    /// </summary>
    public void Createroom()
    {
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
            ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
        }
    }
    /// <summary>
    /// 加入房間
    /// </summary>
    public void Joinroom()
    {
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
            {
                Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
                ConnectInUpdate = false;
                PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
            }
        }
    }
    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage


    public virtual void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        // RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 20 };
        // PhotonNetwork.JoinOrCreateRoom(inp.text, roomOptions, TypedLobby.Default);
        PhotonNetwork.JoinOrCreateRoom(inp.text, new RoomOptions() { MaxPlayers = 20 }, null);

    }
    public virtual void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.CreateRoom(inp.text, new RoomOptions() { MaxPlayers = 20 }, null);

    }

    public virtual void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        PhotonNetwork.CreateRoom(inp.text, new RoomOptions() { MaxPlayers = 20 }, null);

    }

    // the following methods are implemented to give you some context. re-implement them as needed.

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
    }
}
