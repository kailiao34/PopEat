using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public Text nickNameUI;

	public static List<Details> resInfoList = new List<Details>();
	public static PlayerInfos myInfos = new PlayerInfos();
	public static List<PlayerInfos> InfosInRoom = new List<PlayerInfos>();
	public static TcpClient client;

	public string roomName = "ABC", nickName = "KK", foodSelected;
	public List<PlayerInfos> roomTest;

	//private void Start() {
	//	client = new TcpClient("127.0.0.1", 8000);
	//}

	#region ========= 給UI按鈕使用的函數 =========
	public void CreateRoom() {
		if (!ConnectWithRoomName()) return;

		if (roomName != myInfos.roomName) {         // 一樣的房名不再創建名間
			client.CreateRoom(roomName);
		}
	}

	public void JoinRoom() {
		if (!ConnectWithRoomName()) return;

		if (roomName != myInfos.roomName) {         // 一樣的房名不再加入
			client.JoinRoom(roomName);
		}
	}

	public void GoButton() {
		if (client == null || !client.isConnected || myInfos.roomName == "") {
			Debug.LogError("尚未加入房間或建立房間");
			return;
		}
		if (myInfos.nickName == "") {
			Debug.LogError("請輸入暱稱");
			return;
		}
		if (myInfos.foodSelected == "") {
			Debug.LogError("選擇一個餐廳");
			return;
		}

		myInfos.ready = false;
		client.SendPlayerInfos(myInfos);
	}

	public void NickNameButton() {
		myInfos.nickName = nickName;
	}

	public void ReadyButton() {
		if (client != null && client.isConnected) {
			client.SendReady();
		}
	}

	public void StartGameButton() {
	}
	#endregion ====================================

	#region ========== Server Call Back Functions ==========
	void JoinRoomCallback(TcpClient.RoomStatus status) {

		if (status == NetworkBehaviour.RoomStatus.Created) {                // 成功創建房間 (CreateRoom)
			Debug.Log("創建成功");
			myInfos.roomName = roomName;

		} else if (status == NetworkBehaviour.RoomStatus.RoomExists) {      // 房間已存在 (CreateRoom)
			Debug.LogError("房間已存在，請重新輸入");

		} else if (status == NetworkBehaviour.RoomStatus.Joined) {          // 成功加入房間 (JoinRoom)
			Debug.Log("加入成功");
			myInfos.roomName = roomName;

		} else if (status == NetworkBehaviour.RoomStatus.RoomNotExists) {   // 房間不存在 (JoinRoom)
			Debug.LogError("房間不存在，請重新輸入");
		} else {															// 未知錯誤
			Debug.LogError("錯誤");
		}

	}

	void ListChangedCallback() {
		roomTest = InfosInRoom;
	}

	void ReadyCallback(int playerIndex) {
		Debug.Log("Read: " + playerIndex);
	}
	#endregion =============================================
	/// <summary>
	/// 如果尚未連線則嘗試連線，若連線失敗返回 false
	/// </summary>
	bool ConnectWithRoomName() {
		if (roomName == "") {
			Debug.LogError("請輸入房名");
			return false;
		}

		if (client == null || !client.isConnected) {
			myInfos.roomName = "";
			try {
				client = new TcpClient();
				client.ConnectToServer("127.0.0.1", 8056);
				client.OnJoinedRoom = JoinRoomCallback;
				client.OnPlayerListChanged = ListChangedCallback;
				client.OnReadyCallback = ReadyCallback;
			} catch {
				Debug.LogError("伺服器未開啟");
				return false;
			}
		}
		return true;
	}

	private void OnApplicationQuit() {
		if (client != null) client.RealseAll();
	}

	void Update() {
		// 開房
		if (Input.GetKeyDown(KeyCode.D)) {
			CreateRoom();
		}
		// 入房
		if (Input.GetKeyDown(KeyCode.F)) {
			JoinRoom();
		}
		// Go
		if (Input.GetKeyDown(KeyCode.A)) {
			myInfos.nickName = nickName;
			myInfos.foodSelected = foodSelected;
			GoButton();
		}
		// Ready
		if (Input.GetKeyDown(KeyCode.S)) {
			ReadyButton();
		}
	}
}
